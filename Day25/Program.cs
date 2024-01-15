

using Day25;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using System.Xml.Linq;

bool useCompleteData = false;

string testDataPath = "./data_test.txt";
string completeDataPath = "./data_complete.txt";

Part1();
//Part2();


void Part2()
{
    // There was no part 2!
}
void Part1()
{
    // The input will produce a graph.
    // The graph will be undirected:

    // The minimum cut problem in undirected, weighted graphs
    // limited to non-negative weights can be solved in polynomial
    // time by the Stoer-Wagner algorithm. In the special case
    // when the graph is unweighted, Karger's algorithm provides
    // an efficient randomized method for finding the cut. In this
    // case, the minimum cut equals the edge connectivity of the graph.
    // https://en.wikipedia.org/wiki/Minimum_cut

    // It seems the Karger's will work best for an undirected graph.
    // But there is a better version of it: Karger–Stein algorithm
    // https://en.wikipedia.org/wiki/Karger%27s_algorithm#Karger%E2%80%93Stein_algorithm

    // I have a pretty hard time understanding all the intricacies of at all
    // but I will try to make an implementation of something similar.

    Graph graph;
    if (useCompleteData) graph = InterpretData(completeDataPath);
    else graph = InterpretData(testDataPath);

    if (!useCompleteData) graph.WriteAdjacencyMap(0,0);

    int largestValue = -1;
    int nodesInGroup1 = -1;
    int nodesInGroup2 = -1;

    Stopwatch sw = Stopwatch.StartNew();

    int timesContracted = 0;
    while (true)
    {
        if (!useCompleteData) graph.WriteAdjacencyMap(0, 0);

        // Reset the graph each time we failed finding 3 cuts
        graph.Reset();

        // Contract graph at random until only 2 vertices remain (returns true then)
        while (!graph.ContractRandom()) ;

        largestValue = graph.GetLargestValueInMatrix(); // represents the number of connections between the last two nodes after contracting

        if (largestValue <= 3)
        {
            sw.Stop();

            nodesInGroup1 = graph.ContractedHistory.First().Value.Count; // only two values remain after contracting in ContractedHistory, index 0 and 1.
            nodesInGroup2 = graph.ContractedHistory.Last().Value.Count;  // here I get index 0 and 1 with First() and Last()
            break;
        }
        timesContracted++;
        if (useCompleteData)
        {
            Console.WriteLine($"Completed cycle {timesContracted}.\n" +
                              "      Number of connections: " + largestValue + "\n");
        }
    }

    if (!useCompleteData) graph.WriteAdjacencyMap(0, 0);

    Console.WriteLine($"Took {sw.ElapsedMilliseconds} ms.\n");

    Console.WriteLine(" > Largest value in matrix: " + largestValue);
    Console.WriteLine("         group 1 all nodes: " + string.Join(' ', graph.ContractedHistory.First().Value.Order().Select(graph.IDtoString)));
    Console.WriteLine("         group 2 all nodes: " + string.Join(' ', graph.ContractedHistory.Last().Value.Order().Select(graph.IDtoString)));
    Console.WriteLine("\n          total in group 1: " + nodesInGroup1);
    Console.WriteLine("          total in group 2: " + nodesInGroup2);
    Console.WriteLine("\n > Multiplied result is " + (nodesInGroup1 * nodesInGroup2));

    Console.WriteLine();
}

Graph InterpretData(string path)
{
    string[] data = File.ReadAllLines(path);

    Graph g = new(data);
    return g;
}