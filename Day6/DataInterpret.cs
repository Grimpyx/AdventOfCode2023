using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day6
{
    public static class DataInterpret
    {
        public static int[] GetNumbersArrayFrom(string sample, char separationChar)
        {
            List<int> numbers = new List<int>();

            // Prune all characters except numbers and the separationChar
            // String should not contain anything except numbers and separationChar after this
            List<char> sampleAsList = new List<char>();
            for (int i = 0; i < sample.Length; i++)
            {
                char currentChar = sample[i];
                //Console.WriteLine("CurrentChar: " + currentChar);
                if (currentChar == separationChar || char.IsDigit(currentChar))
                    sampleAsList.Add(currentChar);
            }
            string prunedSample = new string(sampleAsList.ToArray());


            // Separate into distinct strings with only numbers, and then parse them into integers
            string[] numbersAsString = prunedSample.Split(separationChar);
            for (int i = 0; i < numbersAsString.Length; i++)
            {
                if (numbersAsString[i] == string.Empty) continue; // skips empty strings
                int nr = int.Parse(numbersAsString[i]);
                numbers.Add(nr);
            }

            // This is a so called collection expression.
            // Creates array from all "IEnumerable entries" in numbers
            return [.. numbers]; 
        }

        public static string GetNumbersCharsFrom(string sample)
        {
            string workedSample = "";
            foreach (char c in sample)
            {
                if (char.IsDigit(c)) workedSample += c;
            }
            return workedSample;
        }
    }
}
