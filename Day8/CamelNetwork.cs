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