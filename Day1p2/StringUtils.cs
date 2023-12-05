using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day2
{
    public static class StringUtils
    {
        private static readonly char[] containList_integers = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
        private static readonly string[] containList_strings = new string[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "zero" };

        private static int FirstNumberOfString(string str, bool reversedDirection = false)
        {
            Console.WriteLine("  str: " + str);

            if (reversedDirection)
            {
                for (int i = str.Length - 1; i >= 0; i--)
                {
                    /*
                    // Substring 5 characters from place i
                    string substring = str.Substring(i, str.Length - i < 5 ? str.Length - i : 5);
                    //Console.WriteLine(str.Length - i < 5 ? str.Length - i : 5);

                    Console.Write("       - Substring " + (str.Length - i < 5 ? str.Length - i : 5).ToString() + ": " + substring);
                    for (int j = 0; j < containList_strings.Length; j++)
                    {
                        if (substring.Contains(containList_strings[j]))
                        {
                            Console.WriteLine("   Found number: " + containList_integers[j]);
                            return CharUnicodeInfo.GetDigitValue(containList_integers[j]);
                        }
                    }
                    Console.WriteLine();

                    // Compare c with all characters in the containList
                    for (int j = 0; j < containList_integers.Length; j++)
                    {
                        char comparedChar = containList_integers[j];
                        if (str[i] == comparedChar)
                        {
                            return CharUnicodeInfo.GetDigitValue(comparedChar); // c is the first number in this case
                        }
                    }*/

                    for (int j = 0; j < str.Length; j++)
                    {
                        string subb = str[(i-j)..(i+1)];
                        Console.WriteLine("       - Substring " + (j + 1) + ": " + subb);

                        // Combine number of contains
                        string[] totalContains = new string[containList_integers.Length + containList_strings.Length];
                        for (int o = 0; o < totalContains.Length; o++)
                        {
                            if (o >= containList_integers.Length)
                                totalContains[o] = containList_strings[o - containList_integers.Length];
                            else
                                totalContains[o] = containList_integers[o].ToString();
                        }

                        // Se if it anything is contained
                        for (int k = 0; k < totalContains.Length; k++)
                        {
                            if (subb.Contains(totalContains[k]))
                            {
                                Console.WriteLine("   Found number: " + totalContains[k]);
                                return CharUnicodeInfo.GetDigitValue(containList_integers[k % 10]);
                            }
                        }
                    }
                    Console.WriteLine();
                }
            }
            else
            {
                // i represents the current char we're looping over
                for (int i = 0; i < str.Length; i++)
                {

                    for (int j = 0; j < str.Length; j++)
                    {
                        string subb = str[..(j+1)];
                        Console.WriteLine("       - Substring " + (j+1) + ": " + subb);

                        // Combine number of contains
                        string[] totalContains = new string[containList_integers.Length + containList_strings.Length];
                        for (int o = 0; o < totalContains.Length; o++)
                        {
                            if (o >= containList_integers.Length)
                                totalContains[o] = containList_strings[o-containList_integers.Length];
                            else
                                totalContains[o] = containList_integers[o].ToString();
                        }

                        // Se if it anything is contained
                        for (int k = 0; k < totalContains.Length; k++)
                        {
                            if (subb.Contains(totalContains[k]))
                            {
                                Console.WriteLine("   Found number: " + totalContains[k]);
                                return CharUnicodeInfo.GetDigitValue(containList_integers[k%10]);
                            }
                        }
                    }
                    Console.WriteLine();
                }
            }
            

            return -1; // -1 means that it failed
        }

        private static int LastNumberOfString(string str)
        {
            // Reverse string
            /*char[] reversedString = new char[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                reversedString[i] = str[str.Length - 1 - i];
            }*/

            return FirstNumberOfString(new string(str), reversedDirection: true);
        }

        public static (int, int) FirstAndLastNumberOfString(string str)
        {
            int first = FirstNumberOfString(str);
            int last = LastNumberOfString(str);

            if (first == -1) first = last;
            if (last == -1) last = first;

            return (first, last);
        }
    }
}
