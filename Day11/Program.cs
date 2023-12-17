
using Day10;
using Day11;

GalaxyMap galaxyMap = new GalaxyMap(File.ReadAllLines("./data_test_p1.txt"));

// Make map of galaxies
//Console.WriteLine(galaxyMap.GetWritableString() + "\n");
galaxyMap.WriteFormatted();
Console.WriteLine("\nExpanded once:");
galaxyMap.Expand();
galaxyMap.WriteFormatted();

//Console.WriteLine("\nExpanded twice:");
//galaxyMap.Expand();
//galaxyMap.WriteFormatted();

Console.WriteLine(galaxyMap.GetTotalDistances());

//Console.WriteLine(galaxyMap.GetTotalDistances());

// Insert empty spaces (cosmic expansion)