using System.Runtime.InteropServices;
using System.Security.AccessControl;
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
            // I looked up some help online, and they talk about a least common multiple?
            // There's apparently a pattern, that for each place you start, only ONCE do you stop by a node ending with Z.

            // After thinking about it I think you could geometrically look at it this way.
            // First we assume 
            // Assume for any Start the iterations required to reach the end looks something like this
            // -----O----   <-- The whole loop. "O" is the destinatione, "-" is any other node.
            // For different starts, they have different lengths.
            // For example:
            // -----O       (length 6)
            // ---O         (length 4)
            // ----O        (length 5)
            //
            // If we loop through every single one you get something like this
            // -----O-----O-----O-----O-----O-----O-----O-----O-----O-----O           ---O--   nr of loops = 4*5 = 20         total steps = 20 * 6 = 120
            // ---O---O---O---O---O---O---O---O---O---O---O---O---O---O---O           -O--     nr of loops = 6*5 = 30         total steps = 30 * 4 = 120
            // ----O----O----O----O----O----O----O----O----O----O----O----O           --O--    nr of loops = 6*4 = 24         total steps = 24 * 5 = 120
            //                                                          | <-- HERE is where they coincide. The distance until this point is 57
            //                                                                which happens to be the least common multiple
            // 
            // LCM method:
            // 6 = 2*3  => NEEDS a 2 and 3
            // 4 = 2*2  => NEEDS two 2
            // 5 = 5    => NEEDS a 5        => 2*2*3*5 = 60

            string[] allNodesEndingWithA = nodes.Keys.Where(x => x.EndsWith('A')).ToArray();
            string[] current = allNodesEndingWithA;
            
            List<long> listOfLoops = new List<long>();
            foreach (string s in current)
            {
                (long, long) lengths = GetLoopLength(s);
                listOfLoops.Add(lengths.Item1);
                Console.WriteLine("Loop length of " + lengths.Item1 + " remainder " + lengths.Item2 + "\n");
            }
            long lcm = LeastCommonMultiple(listOfLoops.ToArray());
            Console.WriteLine("\n > Least common multiple for all items: " + lcm);


            return lcm;
        }

        // My own algorithm
        private long LeastCommonMultipleNumeric(long[] list)
        {
            long leastCommonMultiple = -1;
            long[] incrementedList = [..list];
            while (true)
            {
                long maxVal = incrementedList.Max();

                // Increase all except the largest number with 1 step
                for (int i = 0; i < incrementedList.Length; i++)
                {
                    if (incrementedList[i] == maxVal) continue;
                    else incrementedList[i] += list[i];
                }
                /*foreach (var item in incrementedList)
                {
                    Console.WriteLine(" - " + item);
                }*/

                // if item1 == item2 == ... == itemN, we have reached a common multiple
                bool shouldIterate = false;
                foreach (var item in incrementedList)
                {
                    shouldIterate |= (item != maxVal);
                }

                if (shouldIterate) continue;
                else return maxVal;
            }
        }

        // There is a faster method of calculating the least common multiple, by dividing the product of the list of values with the greatest common divisor
        // One way to calc the GCD: https://en.wikipedia.org/wiki/Euclidean_algorithm#Worked_example <-- see the worked example
        // Apparently, assuming X>Y, the GCD of X and Y is the same as for X and X-Y.
        // Essentially you can recursively make X and Y smaller and smaller until you get a remainder of 0, meaning you found the least common divisor.
        // If you have many values, the least common divisor for all values can be calculated doing  GCD(A,B ... Z) = GCD(A,GCD(B,GCD(C,D))) and on and on and on until you hit Z
        // Below is my implementation of that.
        public static long GreatestCommonDenominator(long[] list)
        {
            // can't find GCD for a list of length 1
            if (list.Length < 2) return -1;

            // If we have more than 2 values, we have to recursively call this until it is only 2 values.
            if (list.Length > 2)
            {
                long gcdForTheFirstTwo = GreatestCommonDenominator(list[1..]);
                list = [list[0], gcdForTheFirstTwo];
            }

            // If we have only 2 values we can proceed
            if (list.Length == 2)
            {
                long greater = Math.Max(list[0], list[1]);
                long lesser = Math.Min(list[0], list[1]);

                while (true)
                {
                    //Console.WriteLine("OLD - Greater: " + greater + "   Lesser: " + lesser);
                    long remainder = greater;

                    // instead of doing -lesser over and over again we can just use modulus to get the remainder
                    remainder = greater % lesser;
                    if (remainder == 0) return lesser; // if true, it is the GCD

                    // When out of that loop, assign new Greater and Lesser
                    greater = lesser;
                    lesser = remainder;
                }
            }

            Console.WriteLine("How did you get here?");
            return -1;
        }
        public static long LeastCommonMultiple(long[] list)
        {
            // can't find LCM for a list of length 1
            if (list.Length < 2) return -1;

            // If we have more than 2 values, we have to recursively call this until it is only 2 values.
            if (list.Length > 2)
            {
                long lcmForTheFirstTwo = LeastCommonMultiple(list[1..]);
                list = [list[0], lcmForTheFirstTwo];
            }

            // If we have only 2 values we can proceed
            if (list.Length == 2)
            {
                // We follow the definition of lcm(A,B) = (A*B)/gcd(A,B) from https://en.wikipedia.org/wiki/Least_common_multiple#Using_the_greatest_common_divisor
                ulong product = 1;
                foreach (long item in list) product *= (ulong)item;
                ulong gcd = (ulong)GreatestCommonDenominator(list);
                return (long)(product/gcd);
            }

            Console.WriteLine("How did you get here?");
            return -1;
        }


        // if iterates over the camelnetwork to find a loop. If it does, it sends the length of it.
        // The first long returned is the length, and the second is the offset between the loop end and the goal "reach anything ending with a Z"
        // for example: ----Z--- has length 8 and an offset of -3 because the three spaces after the goal.
        private (long, long) GetLoopLength(string nodeSource)
        {
            long currentInstructionIndex = 0;
            long stepsUntilZ = 0;
            long zOffsetFromLoopEnd = 0;

            string currentSource = nodeSource;
            string nextSource = "";
            string zSource = "";

            while (true)
            {
                if (currentInstructionIndex >= instructions.Length) currentInstructionIndex = 0;
                stepsUntilZ++;

                // After following the instruction we store the node in nextSource
                Instruction nextInstruction = instructions[currentInstructionIndex];
                switch (nextInstruction)
                {
                    case Instruction.Left:
                        nextSource = nodes[currentSource].Left;
                        break;
                    case Instruction.Right:
                        nextSource = nodes[currentSource].Right;
                        break;
                    default:
                        break;
                }

                // if the next source is the same as the previous node ending in Z the we know for certain we've done a loop
                if (nextSource == zSource)
                {
                    break;
                }

                // If the source ends with Z we start counting the loop length
                // The value in counter stepsUntilZ was used to track the z offset.
                if (currentSource.EndsWith('Z'))
                {
                    //Console.WriteLine("Steps until " + nextSource + ": " + stepsUntilZ);
                    zSource = nextSource;
                    zOffsetFromLoopEnd = stepsUntilZ;
                    stepsUntilZ = 0;
                }
                currentSource = nextSource;
                currentInstructionIndex++;
            }

            // this is essentially this: zOffsetFromLoopEnd = firstLoopWithOffsetLength - completeLoopLength
            zOffsetFromLoopEnd -= stepsUntilZ;
            return (stepsUntilZ, zOffsetFromLoopEnd);
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