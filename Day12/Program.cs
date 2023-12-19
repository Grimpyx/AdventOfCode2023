


//GalaxyMap galaxyMap = new GalaxyMap(File.ReadAllLines("./data_test_p1.txt"));
using Day12;
using System.Diagnostics;

Stopwatch sw1 = Stopwatch.StartNew();

//string[] data = File.ReadAllLines("./data_test_p1.txt");
string[] data = File.ReadAllLines("./data_complete.txt");
long totalNumberOfWays = 0;
foreach (var item in data)
{
    Stopwatch sw2 = Stopwatch.StartNew();
    SpringRow row = new SpringRow(item);
    int nrOfWays = row.ResolveUnknownSprings();
    totalNumberOfWays += nrOfWays;
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write(" > " + row.ToString());
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("   nrOfWays: " + nrOfWays + "   Took " + sw2.ElapsedMilliseconds + "ms");
    sw2.Stop();
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("\n >>> Total nr of ways: " + totalNumberOfWays + "\nTook time " + sw1.ElapsedMilliseconds + "ms\n\n");
Console.ForegroundColor = ConsoleColor.White;
//SpringRow row = new SpringRow(data[0]);