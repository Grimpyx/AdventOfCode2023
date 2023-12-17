using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Day10
{
    public partial class Map
    {
        public readonly Instruction[,] instructions; // [x,y]
        public List<Vector2> insideAreas;
        public List<Vector2> outsideAreas;
        public readonly Path loop;

        public readonly Vector2 start;

        public Map(string[] readAllLinesInput)
        {
            // Initialize the size
            instructions = new Instruction[readAllLinesInput[0].Length, readAllLinesInput.Length];

            // Pupulate the map
            for (int y = 0; y < readAllLinesInput.Length; y++)
            {
                for (int x = 0; x < readAllLinesInput[0].Length; x++)
                {
                    instructions[x, y] = new Instruction(readAllLinesInput[y][x], new Vector2(x,y));

                    if (instructions[x, y].instructionType == Instruction.InstructionType.Start)
                        start = new Vector2(x, y);
                }
            }

            loop = FindLoop();
            CalculateAreas();

        }

        // Get the map as a string in printable format
        public void WriteMap()
        {
            string toWrite = "";
            for (int y = 0; y < instructions.GetLength(1); y++)
            {
                for (int x = 0; x < instructions.GetLength(0); x++)
                {
                    toWrite += instructions[x, y].character;
                }
                toWrite += "\n";
            }
            Console.WriteLine(toWrite);
        }

        // Part 1 assignment
        // You wish to walk all paths, and find the furthest point from the start.
        public int Walk()
        {
            Instruction startInstruction = instructions[(int)start.x, (int)start.y];
            if (startInstruction.instructionType != Instruction.InstructionType.Start) return -1;

            // Evaluate the start
            // Not all connections to the starting coordinate (four, one per horizontal/vetical direction) are actually connected.
            // The method used below correctly selects all the connections that are valid.
            List<Instruction> startingInstructions = [.. GetStartInstructions()];

            // The continuously updating list of current instructions
            List<Path> paths = new List<Path>();
            foreach (var item in startingInstructions)
            {
                if (item.instructionType != Instruction.InstructionType.None && item.instructionType != Instruction.InstructionType.Start)
                    paths.Add(new Path(new List<Instruction>([instructions[(int)start.x,(int)start.y], item])));
            }

            // Keep looking for the next element in all paths...
            bool isAllPathsComplete = false;
            while (!isAllPathsComplete)
            {
                isAllPathsComplete = true;

                // Because we modify the paths during runtime, we need a copy of the unchanged
                // state of it for some logic regarding when to stop adding new instructions
                List<Path> snapshotOfAllPaths = new List<Path>();
                foreach (Path p in paths)
                {
                    snapshotOfAllPaths.Add(new Path(p.steps.ToArray().ToList()));
                }

                // Foreach path... (there should be two paths)
                foreach (Path path in paths)
                { 
                    // This is used to check if all paths have ended (to end the while loop)
                    isAllPathsComplete &= path.hasFoundEnd;

                    // Do not advance the path if it has ended
                    if (path.hasFoundEnd) continue;


                    // Loop though all the connections
                    bool allConnectionsFailed = true;
                    for (int connectionIndex = 0; connectionIndex < path.CurrentInstruction.connections.Length; connectionIndex++)
                    {
                        Vector2 connectionCoord = path.CurrentInstruction.connections[connectionIndex];
                        Instruction connectionInstruction = instructions[(int)connectionCoord.x, (int)connectionCoord.y];

                        // We try to add the connection.
                        // It will return FALSE if it is already part of the path, meaning it returns FALSE if we're moving backward
                        bool ifSucceeded = path.Add(connectionInstruction, snapshotOfAllPaths);

                        // If an instruction was added, we know we made one step.
                        if (ifSucceeded)
                        {
                            allConnectionsFailed = false;
                            break;
                        }
                    }

                    // If we didn't find a next instruction in the path, it has found an end.
                    if (allConnectionsFailed)
                    {
                        path.hasFoundEnd = true;
                        continue;
                    }


                }

            }
            Console.WriteLine("\nFound end for all paths.");

            // Write output
            int min, max;
            min = paths.Select(p => p.steps.Count).Min() - 1;
            max = paths.Select(p => p.steps.Count).Max() - 1;
            bool writeOutput = true;
            if (writeOutput)
            {
                Console.WriteLine("\n > Min steps: " + min);
                Console.WriteLine(" > Max steps: " + max);
                (int, int) previousCursorPos = Console.GetCursorPosition();
    
    
                int offset = 50;

                // Draw the end
                foreach (Path p in paths)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.SetCursorPosition(offset + (int)p.CurrentInstruction.coordinate.x, (int)p.CurrentInstruction.coordinate.y);
                    Console.Write(p.CurrentInstruction.character);
                }
                Thread.Sleep(1000);
                Console.ForegroundColor = ConsoleColor.Yellow;
                foreach (Path p in paths)
                {
                    foreach (Instruction s in p.steps)
                    {
                        if (s == startInstruction) continue;
                        Console.SetCursorPosition(offset + (int)s.coordinate.x, (int)s.coordinate.y);
                        Console.Write(s.character);
                    }
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                foreach (Path p in paths)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.SetCursorPosition(offset + (int)p.CurrentInstruction.coordinate.x, (int)p.CurrentInstruction.coordinate.y);
                    Console.Write(p.CurrentInstruction.character);
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(offset + (int)paths[0].steps[0].coordinate.x, (int)paths[0].steps[0].coordinate.y);
                Console.Write(paths[0].steps[0].character);

                Console.SetCursorPosition(previousCursorPos.Item1, previousCursorPos.Item2);
            }
            

            return min;
        }

        
        private void CalculateAreas()
        {
            // This does three things.
            // First, we walk the path to get the boundaries of the inside area.
            // Secondly, we fill that hole.
            // Thirdly, we define the outside area to be anywhere that is not the inside area or the path.

            int offset = 50;
            bool writeOutput = true;

            // Find all points outside of the path loop
            insideAreas = new List<Vector2>();
            outsideAreas = new List<Vector2>();

            // We walk the path by looping through its coordinates
            Vector2[] allLoopCoordinates = loop.steps.Select(x => x.coordinate).ToArray();
            for (int i = 0; i < allLoopCoordinates.Length - 1; i++) // length-1 because we access allLoopCoordinates[i+1]
            {
                Instruction instAtLoopCoord = instructions[(int)allLoopCoordinates[i].x, (int)allLoopCoordinates[i].y];
                Vector2 localForward = allLoopCoordinates[i + 1] - allLoopCoordinates[i];
                Vector2 localDirectionRight = localForward.PerpendicularClockwise();
                Vector2 localDirectionLeft = localForward.PerpendicularCounterClockwise();


                // For each coordinate in the loop, we check twice
                for (int j = 0; j < 2; j++)
                {
                    Vector2 currentMidpoint = allLoopCoordinates[i + j];
                    Vector2 nextPoint = currentMidpoint;
                    
                    // Depending what way we walk the path, the inside will be to the left or right.
                    // After testing, my loop was counter clockwise, meaning we assign all spaces to the left of
                    // the path as "inside".
                    nextPoint += localDirectionLeft;

                    // If nextPoint is outside of the bounds, we dont do anything
                    if (nextPoint.x < 0 || nextPoint.x >= instructions.GetLength(0) || nextPoint.y < 0 || nextPoint.y >= instructions.GetLength(1)) continue;

                    // If the coordinate to the left is not already part of the inside area, and also not part of the loop, add to insideAreas
                    if (!allLoopCoordinates.Contains(nextPoint) && !insideAreas.Contains(nextPoint))
                    {
                        insideAreas.Add(nextPoint);
                        if (writeOutput)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            Console.SetCursorPosition(offset + (int)nextPoint.x, (int)nextPoint.y);
                            Console.Write('+');
                        }
                    }
                }

            }

            // We loop through all the inside area coordinates. For each coordinate, we compare the surrounding points to see if it is part of the path or not.
            // To compare the surrounding points, we rotate an offset vector four times. If we never find 
            Vector2 dir = new Vector2(0, -1);
            bool didNotFindAny = false;
            while (!didNotFindAny)
            {
                didNotFindAny = true;

                Vector2[] snapshot = new Vector2[insideAreas.Count];
                insideAreas.CopyTo(snapshot);

                foreach (Vector2 v in snapshot)
                {
                    for (int vdircounter = 0; vdircounter < 4; vdircounter++)
                    {
                        Vector2 newVec = v + dir;

                        // If the point is not already part of the inside area and it's not part of the loop, we add it to the inside area.
                        if (!insideAreas.Contains(newVec) && !allLoopCoordinates.Contains(newVec))
                        {
                            // If we never find any new points, we break the loop.
                            didNotFindAny = false;
                            insideAreas.Add(newVec);

                            if (writeOutput)
                            {
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.SetCursorPosition(offset + (int)newVec.x, (int)newVec.y);
                                Console.Write('+');
                            }
                        }
                        dir = dir.PerpendicularCounterClockwise();
                    }
                }
            }

            // Create the outside area
            // Any coordinate not included in the path, or the inside area, is part of the outside area.
            for (int y = 0; y < instructions.GetLength(1); y++)
            {
                for (int x = 0; x < instructions.GetLength(0); x++)
                {
                    if (!insideAreas.Contains(instructions[x, y].coordinate) && !allLoopCoordinates.Contains(instructions[x, y].coordinate))
                    {
                        outsideAreas.Add(instructions[x, y].coordinate);

                        if (writeOutput)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.SetCursorPosition(offset + (int)instructions[x, y].coordinate.x, (int)instructions[x, y].coordinate.y);
                            Console.Write('+');
                        }
                    }
                }
            }

            Console.SetCursorPosition(0, 70);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Outside areas: " + outsideAreas.Count + "   ");
            Console.WriteLine(" Inside areas: " + insideAreas.Count + "   ");
        }


        private Path FindLoop()
        {
            Instruction startInstruction = instructions[(int)start.x, (int)start.y];

            // Evaluate the start
            // Not all connections to the starting coordinate (four, one per horizontal/vetical direction) are actually connected.
            // The method used below correctly selects all the connections that are valid.
            Instruction firstInstruction = instructions[(int)start.x, (int)start.y];

            Instruction secondInstruction = GetStartInstructions()[0];

            // The continuously updating list of current instructions
            Path path = new Path();
            path.steps.AddRange([firstInstruction, secondInstruction]);

            while(!path.hasFoundEnd)
            {
                // Loop though all the connections
                bool allConnectionsFailed = true;
                for (int connectionIndex = 0; connectionIndex < path.CurrentInstruction.connections.Length; connectionIndex++)
                {
                    Vector2 connectionCoord = path.CurrentInstruction.connections[connectionIndex];
                    Instruction connectionInstruction = instructions[(int)connectionCoord.x, (int)connectionCoord.y];

                    // We try to add the connection.
                    // It will return FALSE if it is already part of the path, meaning it returns FALSE if we're moving backward
                    bool ifSucceeded = path.Add(connectionInstruction);

                    // If an instruction was added, we know we made one step.
                    if (ifSucceeded)
                    {
                        allConnectionsFailed = false;
                        break;
                    }
                }

                // If we didn't find a next instruction in the path, it has found an end.
                if (allConnectionsFailed)
                {
                    path.hasFoundEnd = true;
                    continue;
                }
            }


            // Write output
            // This writes the whole map
            bool writeOutput = true;
            if (writeOutput)
            {
                (int, int) previousCursorPos = Console.GetCursorPosition();


                int offset = 50;

                Console.SetCursorPosition(offset + (int)path.CurrentInstruction.coordinate.x, (int)path.CurrentInstruction.coordinate.y);

                Thread.Sleep(200);

                // All loop steps are yellow
                Console.ForegroundColor = ConsoleColor.Yellow;
                foreach (Instruction s in path.steps)
                {
                    if (s == startInstruction) continue;
                    Console.SetCursorPosition(offset + (int)s.coordinate.x, (int)s.coordinate.y);
                    Console.Write(s.character);
                }

                // The last point in the path is magenta
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.SetCursorPosition(offset + (int)path.CurrentInstruction.coordinate.x, (int)path.CurrentInstruction.coordinate.y);
                Console.Write(path.CurrentInstruction.character);

                // The starting point is green
                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(offset + (int)path.steps[0].coordinate.x, (int)path.steps[0].coordinate.y);
                Console.Write(path.steps[0].character);

                Console.SetCursorPosition(previousCursorPos.Item1, previousCursorPos.Item2);
            }


            return path;
        }

        // Selects only the valid instruction leading from the start position
        private List<Instruction> GetStartInstructions()
        {
            // Get all connections
            List<Instruction> instrus = new List<Instruction>();
            for (int i = 0; i < instructions[(int)start.x, (int)start.y].connections.Length; i++)
            {
                Vector2 v = instructions[(int)start.x, (int)start.y].connections[i];
                instrus.Add(instructions[(int)v.x,(int)v.y]);
            }

            Instruction startInstruction = instructions[(int)start.x, (int)start.y];
            // Remove those that are illegal (that arent actually connected)
            foreach (var inst in instrus.ToArray())
            {
                if (!startInstruction.IsConnectedTo(inst))
                    instrus.Remove(inst);
            }

            return instrus;

        }
    }
}
