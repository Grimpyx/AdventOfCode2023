

using Day9;

OasisHistory oh = new OasisHistory(File.ReadAllLines("./data_complete.txt"));

long sumOfAllExtrapolatedTopValues = 0;
foreach (var item in oh.entries)
{
    Console.WriteLine("---------------------------------\n" + item.ToPrintedString());
    item.Extrapolate(1);
    Console.WriteLine(item.ToPrintedString());
    sumOfAllExtrapolatedTopValues += item.subEntries[0].values.Last();
}


// 1868368343 was right! First try feels good
Console.WriteLine("\n > Done! Total was " + sumOfAllExtrapolatedTopValues);