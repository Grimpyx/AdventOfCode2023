using Day3;

Schematic schematic = new Schematic(File.ReadAllLines("./data_complete.txt"));


// Program start
for (int qqq = 0; qqq < 500; qqq++)
{
    /*
    // loop through schematic and write it
    for (int x = 0; x < schematic.Dimension[0]; x++) // x-value from top left
    {
        for (int y = 0; y < schematic.Dimension[1]; y++) // y value from top left
        {
            Console.SetCursorPosition(x, y);
            Console.Write(schematic.Symbols[x, y]);
        }
    }*/

    Console.WriteLine("\n >> X: {0}   Y: {1}", schematic.Dimension[0], schematic.Dimension[1]);
    Console.WriteLine("\n >> [x,y]=[1,4]: {0}", schematic.Symbols[1, 4]);

    /*string? input = Console.ReadLine();
    if (int.TryParse(input, out int inputNumber))
    {
        Console.WriteLine(" Row: " + schematic.GetColumn(inputNumber));
        Console.WriteLine(" Col: " + schematic.GetRow(inputNumber));
    }*/
    List<Schematic.Map> maps = schematic.FindAllMaps();
    int sumOfAllPartNumbers = 0;
    foreach (Schematic.Map map in maps)
    {
        if (map.HasSymbolAdjacent()) // only a part number if it has adjacent symbol
            sumOfAllPartNumbers += map.Value;
    }
    Console.WriteLine(" >>>>> Sum of parts: " + sumOfAllPartNumbers);
    int sumOfRatios = 0;
    foreach (int ratio in Schematic.Map.FindGearRatios(maps))
        sumOfRatios += ratio;
    Console.WriteLine(" >>>>> Sum of ratio: " + sumOfRatios);



    Console.Write("\n\nPress anything to continue...");
    Console.Read();
    Console.Clear();





}