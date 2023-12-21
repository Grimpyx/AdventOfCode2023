
// Interpret data
using Day13;

List<MirrorMap> mirrorMaps = new List<MirrorMap>();

string[] rowData = File.ReadAllLines("./data_complete.txt");
List<string> currentMapAsString = new List<string>();
int rowCounter = 0;
int mapNr = 1;
long totalValue = 0;
while (rowCounter < rowData.Length)
{
    if (rowData[rowCounter].Length > 1)
        currentMapAsString.Add(rowData[rowCounter++]);
    
    if (rowData[rowCounter].Length == 0 || rowCounter == rowData.Length - 1)
    {
        MirrorMap mm = new([.. currentMapAsString]);
        mirrorMaps.Add(mm);
        currentMapAsString.Clear();
        Console.WriteLine(mm.ToString());
        rowCounter++;

        long value = mm.Solve();
        totalValue += value;

        Console.WriteLine("Map " + mapNr);
        Console.WriteLine("| Value: " + value);
        Console.WriteLine("| Total: " + totalValue + "\n\n");
        mapNr++;

        //MirrorMap.Terrain[] row = mm.GetRow(0);
        //MirrorMap.Terrain[] column = mm.GetColumn(0);
        //Console.WriteLine("Row: " + string.Join(' ', row.Select(x => x == MirrorMap.Terrain.Rocks ? '#' : '.')));
        //Console.WriteLine("Col: " + string.Join(' ', column.Select(x => x == MirrorMap.Terrain.Rocks ? '#' : '.')));
    }
}



/*string[] colData = new string[rowData[0].Length];
for (int i = 0; i < colData.Length; i++)
{
    string col = "";
    for (int j = 0; j < rowData[i].Length; j++)
    {
        col += rowData[i][j];
    }
    colData[i] = col;
    Console.WriteLine(col);
}*/


