using static Day8.CamelNetwork;

namespace Day8
{
    public class CamelNetwork
    {
        public enum Instruction
        {
            Left = 'L',
            Right = 'R'
        }
        private Instruction[] instructions;

        private Dictionary<string, CamelNext> nodes = new Dictionary<string, CamelNext>();

        public CamelNetwork(Instruction[] instructions, Dictionary<string, CamelNext> nodes)
        {
            this.instructions = instructions;
            this.nodes = nodes;
        }

        public CamelNetwork(string[] readAllLinesInput)
        {
            // Interpret data
            // The first row is all the instructions
            string instructionsAsString = readAllLinesInput[0];
            instructions = new Instruction[instructionsAsString.Length];
            for (int i = 0; i < instructionsAsString.Length; i++)
                instructions[i] = (Instruction)instructionsAsString[i];


            // The first row is all the instructions
            string[] nodesAsStrings = readAllLinesInput[2..];
            for (int row = 0; row < nodesAsStrings.Length; row++)
            {
                string from = nodesAsStrings[row].Split(" = ")[0];
                string[] rightHand = nodesAsStrings[row].Split(", ");
                string toLeft = rightHand[0][(rightHand[0].Length-3)..];
                string toRight = rightHand[1][..3];

                nodes.Add(from, new CamelNext(toLeft, toRight));
            }
        }

        public int FindStepsToReach(string start, string destination)
        {
            int steps = 0;
            int currentInstructionIndex = 0;

            string current = start;

            // loop through instructions
            while (current != destination)
            {
                if (currentInstructionIndex >= instructions.Length) currentInstructionIndex = 0;

                switch (instructions[currentInstructionIndex])
                {
                    case Instruction.Left:
                        current = nodes[current].Left;
                        break;
                    case Instruction.Right:
                        current = nodes[current].Right;
                        break;
                    default:
                        break;
                }

                currentInstructionIndex++;
                steps++;
            }

            return steps;
        }

        public long FindStepsToReachGhostMode()
        {
            string[] allNodesEndingWithA = nodes.Keys.Where(x => x.EndsWith('A')).ToArray();

            long steps = 0;
            int printCounter = 0;
            int currentInstructionIndex = 0;

            string[] current = allNodesEndingWithA;

            // loop through instructions
            bool continueCondition = true;
            while (continueCondition)
            {
                if (currentInstructionIndex >= instructions.Length) currentInstructionIndex = 0;
                if (printCounter >= 2500000)
                {
                    Console.WriteLine("steps: " + steps);
                    printCounter = 0;
                }

                bool allEndsWithZ = true;
                // Every index
                for (int i = 0; i < current.Length; i++)
                {
                    switch (instructions[currentInstructionIndex])
                    {
                        case Instruction.Left:
                            current[i] = nodes[current[i]].Left;
                            break;
                        case Instruction.Right:
                            current[i] = nodes[current[i]].Right;
                            break;
                        default:
                            break;
                    }

                    if (current[i].EndsWith('Z')) allEndsWithZ &= true;
                    else
                    {
                        allEndsWithZ &= false;
                        break; // if this index fails on 1, all fail, so we exit the loop
                    }
                }

                continueCondition = !allEndsWithZ;

                currentInstructionIndex++;
                steps++;
                printCounter++;
            }

            Console.WriteLine($" - After {steps} steps, found the following values ending with Z: ");
            Console.Write("     ");
            foreach (string c in current)
            {
                Console.Write(c + "  ");
            }
            Console.WriteLine("\n");

            return steps;
        }

        public struct CamelNext
        {
            public CamelNext(string left, string right)
            {
                Left = left;
                Right = right;
            }

            public string Left { get; init; }
            public string Right { get; init; }
        }

    }
}