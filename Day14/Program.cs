




using Day14;

string[] rowData = File.ReadAllLines("./data_test_p1.txt");
Rock[,] rocks = new Rock[rowData[0].Length, rowData.Length];
for (int i = 0; i < rocks.GetLength(1); i++)
{
	for (int j = 0; j < rocks.GetLength(0); j++)
	{
        char c = rowData[j][i];
        rocks[j, i] = new Rock((RockType)c);
    }
}
Map2D<Rock> map = new Map2D<Rock>(rocks);

Console.WriteLine(map);



