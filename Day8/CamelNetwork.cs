using static Day8.CamelNetwork;

namespace Day8
{
    public class CamelNetwork
    {
        public enum Instruction;
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


            // The first row is all the instructions
            string[] nodesAsStrings = readAllLinesInput[2..];
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

        public int FindEnd()
        {
            int steps = 0;
            return steps;
        }
    }
}