using Day12;

//string[] data = File.ReadAllLines("./data_test_p1.txt");
string[] data = File.ReadAllLines("./data_complete.txt");
AllSprings springs = new AllSprings(data, true);

long totalNumberOfWays = 0;

foreach (var row in springs.springRows)
{
    string nrstring = "";
    foreach (var item in row.continuousGroups)
    {
        nrstring += item.ToString() + ",";
    }

    Console.WriteLine(" | " + row.SpringsAsString() + " " + nrstring[..(nrstring.Length-1)]);
    long v = row.CreatePermutations();
    totalNumberOfWays += v;
    Console.WriteLine(" | Value: " + v);
    Console.WriteLine(" | Total: " + totalNumberOfWays + "\n");
}