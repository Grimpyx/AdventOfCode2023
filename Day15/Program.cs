
using Day15;


string data = File.ReadAllText("./data_complete.txt");
//string data = File.ReadAllText("./data_test.txt");
string[] splitData = data.Split(',');
long sumOfResults = 0;


//Part1();
Part2();

void Part1()
{
    foreach (string s in splitData)
    {
        if (s == string.Empty) continue;
        if (s == "\r\n") continue;
        PrintHashFor(s.Trim(['\r', '\n']));
    }
    Console.WriteLine(" > Result: " + sumOfResults);

    void PrintHashFor(string s)
    {
        int hashFromString = HashAlgorithm(s);
        Console.WriteLine($"\"{s}\" has hash {hashFromString}.");
        sumOfResults += hashFromString;
    }
}

void Part2()
{
    // Create boxes
    Dictionary<byte, Box> boxes = new();
    for (int i = 0; i < byte.MaxValue + 1; i++)
    {
        boxes.Add((byte)i, new Box());
    }

    // Add/remove lenses depending on the input
    foreach (string s in splitData)
    {
        string lensLabel;
        byte boxNr;

        // in case we have an '='
        if (char.IsDigit(s[^1]))
        {
            byte focalLength = byte.Parse(s[^1..]);
            lensLabel = s[..^2];
            boxNr = HashAlgorithm(lensLabel);

            boxes[boxNr].AddLens(lensLabel, focalLength);
        }
        else // If operation is '-'
        {
            lensLabel = s[..^1];
            boxNr = HashAlgorithm(lensLabel);

            boxes[boxNr].RemoveLens(lensLabel);
        }
    }

    long focusingPower = 0;
    // Calculate focusing power
    for (int boxIndex = 0; boxIndex < 256; boxIndex++)
    {
        Box box = boxes[(byte)boxIndex];
        // For each lens
        // + focal length
        int lensCounter = 0;
        foreach (Lens l in box.Lenses)
        {
            if (l.IsEmpty) continue;
            lensCounter++; // start counting from 1, not 0

            long fpBox = 1 + boxIndex; // boxIndex is 0-255, but we need it to be 1-256
            fpBox *= lensCounter * l.focalLength; // as per the instruction

            Console.WriteLine("fp for lens " + l.label + ": " + fpBox);
            focusingPower += fpBox;
        }

    }
    Console.WriteLine(" > Tot: " + focusingPower);
}


// Per instruction
byte HashAlgorithm(string s)
{
    if (s == string.Empty) return 0;
    if (s == "\r\n") return 0;

    int number = 0;
    foreach (char c in s)
    {
        if (c == '\n' || c == '\0' || c == '\r') continue;
        number += c;
        number *= 17;
        number %= 256;
    }

    return (byte)number;
}
