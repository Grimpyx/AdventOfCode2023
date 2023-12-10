using System;
using System.Globalization;

internal class Program
{
    public static readonly char[] containList = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };

    public static readonly string[] list = File.ReadAllLines("./data_complete.txt");


    static void Main(string[] args)
    {
        Console.WriteLine("In the list, these are the numbers:");

        List<int> allIntegers = new List<int>();
        int sumOfAll;

        // Loop through each entry
        foreach (string s in list)
        {
            (int first, int last) = FirstAndLastNumberOfString(s);

            if (first == -1 || last == -1)
            {
                Console.WriteLine($"Failed to determine for \"{s}\".");
            }
            else
            {
                Console.WriteLine($"   > First: {first}  Last: {last}  Total: {first}{last}");
                allIntegers.Add(int.Parse($"{first}{last}"));
            }
        }

        sumOfAll = allIntegers.Sum(x => x); // weird ass extension method
        Console.WriteLine("\n >>> Sum of all: " + sumOfAll.ToString());


        Console.WriteLine("\n\n\n");
    }

    private static int FirstNumberOfString(string str)
    {
        // Loop through all characters in the string
        foreach (char c in str)
        {
            // Compare c with all characters in the containList
            for (int i = 0; i < containList.Length; i++)
            {
                if (c == containList[i])
                {
                    return CharUnicodeInfo.GetDigitValue(c); // c is the first number in this case
                }
            }
        }
        return -1; // -1 means that it failed
    }

    private static int LastNumberOfString(string str)
    {
        char[] reversedString = new char[str.Length];
        for (int i = 0; i < str.Length; i++)
        {
            reversedString[i] = str[str.Length - 1 - i];
        }

        return FirstNumberOfString(new string(reversedString));
    }

    private static (int, int) FirstAndLastNumberOfString(string str)
    {
        int first = FirstNumberOfString(str);
        int last = LastNumberOfString(str);

        if (first == -1) first = last;
        if (last == -1) last = first;

        return (first, last);
    }
}