




using Day14;
using Day10;

string[] rowData = File.ReadAllLines("./data_complete.txt");
Rock[,] rocks = new Rock[rowData[0].Length, rowData.Length];
for (int i = 0; i < rocks.GetLength(1); i++)
{
	for (int j = 0; j < rocks.GetLength(0); j++)
	{
        char c = rowData[i][j];
        rocks[j, i] = new Rock((RockType)c);
    }
}
Map2D<Rock> map = new Map2D<Rock>(rocks);

Console.WriteLine(map);
Console.WriteLine();
TiltMap(map, Vector2.down);
Console.WriteLine();
//TiltMap(map, Vector2.up);
//Console.WriteLine();
//TiltMap(map, Vector2.left);
//Console.WriteLine();
//TiltMap(map, Vector2.right);



(Map2D<Rock>? map, long load) TiltMap(Map2D<Rock> mapToTilt, Vector2 direction) // north is (0,-1), because -1 is upward
{
    if (direction.x != 0 && direction.y != 0) return default;
    direction = direction.Normalized;

    Map2D<Rock> map2d = new Map2D<Rock>(mapToTilt.values);
    long load = 0;

    // Determine first coordinate
    Vector2 startCoordinate;
    int i_max, j_max;
    switch ((direction.x, direction.y))
    {
        case (0, 1):
            startCoordinate = new Vector2(map2d.ColumnLength - 1, map2d.RowLength - 1);
            i_max = map2d.RowLength;
            j_max = map2d.ColumnLength;
            break;
        case (0, -1):
            startCoordinate = Vector2.zero;
            i_max = map2d.RowLength;
            j_max = map2d.ColumnLength;
            break;
        case (1, 0):
            startCoordinate = new Vector2(map2d.ColumnLength - 1, 0);
            i_max = map2d.ColumnLength;
            j_max = map2d.RowLength;
            break;
        case (-1, 0):
            startCoordinate = new Vector2(0, map2d.RowLength - 1);
            i_max = map2d.ColumnLength;
            j_max = map2d.RowLength;
            break;
        default:
            startCoordinate = Vector2.zero;
            i_max = -1;
            j_max = -1;
            break;
    }



    Vector2 nextStep = direction.PerpendicularCounterClockwise();
    Vector2 nextRow = -direction;

    Vector2 currentCoordinate = startCoordinate;

    // Index represents column, value in that column is the row
    // So vector wise, j*nextStep is the j-location x value
    int[] rockFallRow = Enumerable.Repeat(0, j_max).ToArray();

    for (int i = 0; i < i_max; i++) // row
    {
        for (int j = 0; j < j_max; j++)
        {
            Rock r = map2d.values[(int)currentCoordinate.x, (int)currentCoordinate.y];
            if (r.type != RockType.None)
            {
                if (r.type == RockType.Rolling)
                {
                    Vector2 v = startCoordinate;    // Where the ij loop starts
                    v += j * nextStep;              // j steps to "the right" (to the left of direction vector)
                    v += rockFallRow[j] * nextRow;  // rockFallRow[j] steps "below" direction vector

                    map2d.values[(int)currentCoordinate.x, (int)currentCoordinate.y] = new Rock(RockType.None);
                    map2d.values[(int)v.x, (int)v.y] = new Rock(RockType.Rolling);
                    rockFallRow[j] = rockFallRow[j] + 1;
                    load += i_max - (long)v.y; // v.y represents how far away from the max value we are.
                }
                else if (r.type == RockType.Stationary)
                    rockFallRow[j] = i + 1;

            }

            currentCoordinate += nextStep;
        }
        ////Console.WriteLine(i + ":\n" + map2d.ToString());
        currentCoordinate = startCoordinate + (i+1) * nextRow;
    }

    Console.WriteLine(map2d.ToString());
    Console.WriteLine("Load: " + load);
    return (map2d, load);
}
