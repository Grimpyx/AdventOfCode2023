using Day2;

int[] maxNumberOfColoredCubes = new int[3] { 12, 13, 14 };

string[] data = File.ReadAllLines("./data_complete.txt");
//data.ToList().ForEach(x => Console.WriteLine(x));

Game[] games = new Game[data.Length];
for (int i = 0; i < data.Length; i++)
{
    games[i] = new Game(data[i]);
    Console.WriteLine("\t" + games[i].ToString());
}

Console.WriteLine("\nLegality:");
int totalSumOfIDs = 0;
int totalSumOfPowers = 0;
foreach (Game game in games)
{
    bool legality = game.IsGameLegal(maxNumberOfColoredCubes[0], maxNumberOfColoredCubes[1], maxNumberOfColoredCubes[2]);
    Console.WriteLine(" > Game {0}: {1}", game.id, legality ? "Yes" : "No");
    if (legality) totalSumOfIDs += game.id;

    totalSumOfPowers += game.GetPower();
}
Console.WriteLine(" >>> Total ID sum: " + totalSumOfIDs);
Console.WriteLine(" >>> Total Power sum: " + totalSumOfPowers);
