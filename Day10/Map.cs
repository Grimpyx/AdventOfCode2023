using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Day10
{
    public partial class Map
    {
        public readonly Instruction[,] instructions; // [x,y]
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

                // Foreach path...
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
