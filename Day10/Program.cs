

using Day10;

Map map = new Map(File.ReadAllLines("./data_complete.txt"));

int walk = map.Walk();
Console.WriteLine("Walk: " + walk);
Console.Read();
