using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day12
{
    public partial class SpringRow
    {
        public int[] continuousGroups;

        public enum Condition
        {
            Unknown = '?',
            Operational = '.',
            Damaged = '#'
        }
        public Condition[] springs;

        public SpringRow(string row, bool isPart2 = false)
        {
            string[] a = row.Split(' ');
            string[] nrAsString = a[1].Split(',');
            char[] springsAsCharray = a[0].ToCharArray();

            springs = new Condition[springsAsCharray.Length];
            for (int i = 0; i < springs.Length; i++)
            {
                springs[i] = (Condition)springsAsCharray[i];
            }

            continuousGroups = new int[nrAsString.Length];
            for (int i = 0; i < continuousGroups.Length; i++)
            {
                continuousGroups[i] = int.Parse(nrAsString[i]);
            }


            // Part 2 has a modified version
            if (isPart2)
            {
                int[] newContinuousGroups = new int[continuousGroups.Length * 5];
                List<Condition> newSprings = new List<Condition>(); //new Condition[continuousGroups.Length * 5];

                for (int i = 0; i < newContinuousGroups.Length; i++)
                {
                    int j = i % continuousGroups.Length;
                    newContinuousGroups[i] = continuousGroups[j];
                }

                for (int i = 0; i < 5; i++)
                {
                    if (i != 0) newSprings.Add((Condition)'?');
                    foreach (var item in springs) newSprings.Add(item);
                }

                continuousGroups = newContinuousGroups;
                springs = newSprings.ToArray();
            }
        }


        public int ResolveUnknownSprings()
        {
            int validSets = 0;
            bool writeOutput = false;

            string springsAsString = SpringsAsString();
            string[] groups = RemoveConsecutiveCharacter('.', springsAsString.Trim('.')).Split('.');

            if (writeOutput)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" > " + ToString());
            }

            //int indicesWhereThereAreQuestionmarks = springsAsString.Where(x => x == '?').ToArray().Length;
            List<int> indices = new List<int>();
            for (int i = 0; i < springsAsString.Length; i++)
            {
                if (springsAsString[i] == '?')
                {
                    if (writeOutput) Console.WriteLine(" Found ? at index " + i);
                    indices.Add(i);
                }
            }

            // represent in binary i = 0000 -> 0001 -> 0010 -> 0011 -> 0100 -> 0101 -> 0110 and on and on
            char[] questionmarks = new char[indices.Count];
            if (writeOutput) Console.WriteLine("Binary counter: " + ((1 << questionmarks.Length) - 1));

            // For three questionmarks the length is 8 (from 0 to 7), meaning in binary from 000 to 111
            for (int i = 0; i < (1 << questionmarks.Length); i++)
            {
                //int indexOfThis = indices[i];

                // Convert i to series of "." and "#".
                char[] springConversion = Convert.ToString(i, 2).ToCharArray();

                // Insert zeroes in front (i = 0 would give string "0", but we want the string "0000" for example)
                if (springConversion.Length < questionmarks.Length)
                {
                    int l = (questionmarks.Length - springConversion.Length);
                    List<char> newValues = [.. new string('0', l)];
                    newValues.AddRange(springConversion);
                    springConversion = newValues.ToArray();
                }
                if (writeOutput)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("  " + new string(springConversion)); // write binary string
                }


                // Convert binary string to "#" or "."
                for (int sc = 0; sc < springConversion.Length; sc++)
                {
                    if (springConversion[sc] == '1') springConversion[sc] = '#';
                    else springConversion[sc] = '.';
                }
                if (writeOutput)
                {
                    Console.WriteLine("  " + new string(springConversion)); // write binary string
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }

                // Construct a new string of all springs
                string completeSpringsString = "";
                int springConversionCounter = 0;
                if (writeOutput) Console.Write("  ");
                for (int sas = 0; sas < springsAsString.Length; sas++)
                {
                    // If a '?' we replace it with one of the generates springs from the binary number
                    if (springsAsString[sas] == '?')
                    {
                        if (writeOutput) Console.ForegroundColor = ConsoleColor.Green;
                        completeSpringsString += springConversion[springConversionCounter];
                        springConversionCounter++;
                    }
                    else
                    {
                        if (writeOutput) Console.ForegroundColor = ConsoleColor.Yellow;
                        completeSpringsString += springsAsString[sas];
                    }
                    if (writeOutput) Console.Write(springsAsString[sas]);
                }
                if (writeOutput)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine();
                }

                // See if it's a valid solution to the continuousGroup
                if (StringIsValid(completeSpringsString))
                {
                    if (writeOutput)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("  " + completeSpringsString + " is valid!\n\n");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    validSets++;
                }
            }

            // Write number of valid sets
            if (writeOutput)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(" > Valid sets: " + validSets);
                Console.ForegroundColor = ConsoleColor.Yellow;
            }

            return validSets;
        }

        bool StringIsValid(string s)
        {
            string[] groups = RemoveConsecutiveCharacter('.', s.Trim('.')).Split('.');
            //Console.WriteLine("  " + s + new string(' ', 25 - s.Length) + RemoveConsequtiveCharacter('.', s.Trim('.')) + "\n");

            if (groups.Length == continuousGroups.Length)
            {
                for (int i = 0; i < groups.Length; i++)
                {
                    if (groups[i].Length != continuousGroups[i])
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        // For example, if given ..#.#...###. and the consecutiveCharacter is '.', it will return .#.#.###.
        string RemoveConsecutiveCharacter(char consecutiveCharacter, string fromString)
        {
            List<char> temp = new List<char>();
            bool isConsequtive = false;
            foreach (char c in fromString)
            {
                if (isConsequtive && c == consecutiveCharacter) continue;
                else isConsequtive = false;

                if (c == consecutiveCharacter) isConsequtive = true;

                temp.Add(c);
            }
            return new string(temp.ToArray());
        }

        public string SpringsAsString()
        {
            string completeSeries = "";
            foreach (var item in springs)
            {
                completeSeries += (char)item;
            }
            return completeSeries;
        }

        public override string ToString()
        {
            string s = SpringsAsString() + " ";
            bool first = true;
            foreach (var item in continuousGroups)
            {
                if (!first) s += ',';
                s += item;
                first = false;
            }
            return s;
        }


    }
}
