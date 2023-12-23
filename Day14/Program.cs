


// 104733 too low

using Day14;
using Day10;
using Memoization;

//string[] rowData = File.ReadAllLines("./data_test_p1.txt");
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

//Part1();
Part2();

void Part1()
{
    Console.WriteLine(map.ToString());
    Console.WriteLine("\n----------->\n");
    Map2D<Rock> outputMap = TiltMap(map, Vector2.down); // tilt north
    int load = GetLoad(outputMap, Vector2.down);
    Console.WriteLine(outputMap.ToString());
    Console.WriteLine("Load: " + load);

}

void Part2()
{
    Map2D<Rock>? output = Map2D<Rock>.MakeCopyFrom(map);
    long times = 1000000000;

    List<int> visitedStatesHashcodes = new List<int>();
    List<long> visitedStatesLoad = new List<long>();

    int startOfLoopIndex = -1;
    int loopLength = -1;

    bool countLoop = false;

    // Loop perform the cycle of tilting
    for (int i = 0; i < times; i++)
    {
        // Pattern is north, west, south, east.
        // The memoization is completely unnecessary
        output = TiltMap(output, Vector2.down);     // tilt north
        output = TiltMap(output, Vector2.left);     // tilt west
        output = TiltMap(output, Vector2.up);       // tilt south
        output = TiltMap(output, Vector2.right);    // tilt east

        int hc = output.GetHashCode();

        // If we don't encounter a loop
        if (i == 0 || !visitedStatesHashcodes.Contains(hc)) 
        {
            visitedStatesHashcodes.Add(hc);
            int l = GetLoad(output, Vector2.down);
            visitedStatesLoad.Add(l);
        }
        // If we find a loop.
        // This will run two times in total, to confirm that we found a loop.
        else
        {
            Console.WriteLine("Found loop point at i = " + i + ".  Old HC at " + visitedStatesHashcodes.IndexOf(hc));

            // First loop is found
            if (!countLoop)
            {
                countLoop = true;
                visitedStatesHashcodes.Clear();
                visitedStatesLoad.Clear();
                continue;
            }
            // If we find another loop we have all information we need to skip all unnecessary computations:
            // - start of the loop
            // - length of loop
            else
            {
                startOfLoopIndex = i;
                loopLength = visitedStatesHashcodes.Count;
                break;
            }
        }

    }

    int visitedIndex = ((int)times - 1 - startOfLoopIndex) % loopLength;
    long load = visitedStatesLoad[visitedIndex];
    // Illustration of the above index:
    //                              v                        v                      v
    //   |-------------|--x---------------------|--x--------------...--|--x---------|
    //   --beforeLoop---                        ------(loops)-----...---
    //                 --------loopLength--------                      ---postLoop---
    //
    //   01234...---------------------allTime----------------------------------------
    //
    //   v = the index of the loop where it ends, counting from the beginning of the loop.
    //   Once we find the loop we can calculate v:
    //   Firstly, everytime we use (alltime) we have to use (alltime - 1) becuase our loop goes from 0->alltime-1, not 1->alltime.
    //   ((allTime - 1) - beforeLoop) gives us the total number of indices until the end.
    //   ((alltime - 1) - beforeloop) % loopLength) "removes" all loopLength and leaves us with the remainder.
    //   That is the index to v!


    Console.WriteLine("\n   Load: " + load);
    Console.WriteLine();
}


// Get the load in a given direction of a given map2d
// Only direction (0,-1) has been confirmed to work (north)
int GetLoad(Map2D<Rock> map, Vector2 direction)
{
    if (direction.x != 0 && direction.y != 0) return default;
    direction = direction.Normalized;

    int result = 0;
    switch ((direction.x, direction.y))
    {
        case (0, 1):
            for (int i = 0; i < map.RowLength; i++)
            {
                int add = map.GetRow(map.RowLength - (i+1)).Where(x => x.type == RockType.Rolling).Count() * (i+1);
                result += add;
            }
            break;
        case (0, -1): // only this is confirmed to work
            for (int i = 0; i < map.RowLength; i++)
            {
                int add = map.GetRow(i).Where(x => x.type == RockType.Rolling).Count() * (map.RowLength - i);
                result += add;
            }
            break;
        case (1, 0):
            for (int i = 0; i < map.RowLength; i++)
            {
                int add = map.GetColumn(map.ColumnLength - (i + 1)).Where(x => x.type == RockType.Rolling).Count() * (i + 1);
                result += add;
            }
            break;
        case (-1, 0):
            for (int i = 0; i < map.RowLength; i++)
            {
                int add = map.GetColumn(i).Where(x => x.type == RockType.Rolling).Count() * (map.RowLength - i);
                result += add;
            }
            break;
        default:

            break;
    }

    return result;
}

Map2D<Rock> TiltMap(Map2D<Rock> mapToTilt, Vector2 direction) // north is (0,-1), because -1 is upward
{
    if (direction.x != 0 && direction.y != 0) return new Map2D<Rock>(new Rock[0,0]);
    direction = direction.Normalized;

    Map2D<Rock> map2d = Map2D<Rock>.MakeCopyFrom(mapToTilt);

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



    Vector2 currentCoordinate = startCoordinate;

    // Index represents column, value in that column is the distance away from the
    // furthest point to the next space in front of a stone
    // See this example:
    //               . . # . O O . O
    //               # . . . O . . #
    // rockFallRow = 2 0 1 0 2 1 0 2
    //
    // So vector wise, j*nextStep is the j-location x value
    int[] rockFallRow = Enumerable.Repeat(0, j_max).ToArray();

    Vector2 nextStep = direction.PerpendicularCounterClockwise();   // to the left of direction
    Vector2 nextRow = -direction;                                   // opposite of direction

    // The way this loop works is that j represents the column, and i is the row.
    // However, I use vectors in a manner that lets i represent the row relative to the direction we specify.
    // If we specify North, we want to move right (j) and down (i) from the coordinate (0,0) (top left corner).
    // If we specify East,  we want to move down (j) and left (i) from the coordinate (x_max, 0) (top right corner).
    // If we specify South,  we want to move left (j) and up (i) from the coordinate (x_max, y_max) (bottom right corner).
    // If we specify West,  we want to move up (j) and right (i) from the coordinate (0, y_max) (bottom left corner).
    //
    // This way, we always count i to the right of the direction, and j to the opposite of direction
    // (see nextStep and nextRow above)
    for (int i = 0; i < i_max; i++)
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
                }
                else if (r.type == RockType.Stationary)
                    rockFallRow[j] = i + 1;

            }

            currentCoordinate += nextStep;
        }
        currentCoordinate = startCoordinate + (i + 1) * nextRow;
    }

    return map2d;
}