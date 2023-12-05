using Day3;

Schematic schematic = new Schematic(File.ReadAllLines("./data_complete.txt"));

/*
// loop through schematic and write it
// The large schematic is way too big for this
for (int x = 0; x < schematic.Dimension[0]; x++) // x-value from top left
{
    for (int y = 0; y < schematic.Dimension[1]; y++) // y value from top left
    {
        Console.SetCursorPosition(x, y);
        Console.Write(schematic.Symbols[x, y]);
    }
}*/

List<Schematic.Map> maps = schematic.FindAllMaps();
int sumOfAllPartNumbers = 0;
foreach (Schematic.Map map in maps)
{
    // All part numbers has a symbol adjacent.
    if (map.HasSymbolAdjacent())
        sumOfAllPartNumbers += map.Value;
}
Console.WriteLine(" >  Sum of parts: " + sumOfAllPartNumbers);
int sumOfRatios = 0;
foreach (int ratio in Schematic.Map.FindGearRatios(maps))
    sumOfRatios += ratio;
Console.WriteLine(" > Sum of ratios: " + sumOfRatios);



Console.Write("\n\nPress anything to continue...");
Console.Read();
Console.Clear();