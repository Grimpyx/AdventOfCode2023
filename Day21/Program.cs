// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");


using Day21;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

//Part1(); // Answer is 38XX
Part2(); // Answer is 636350496972143
void Part2()
{
    // Had to look at a bunch of visualizations to know what to do here. Thanks AOC reddit!

    // The steps we need to go is 26501365.

    // First we create a Stepping algorithm that advances the map, just like in part 1.
    // But we have to make sure the map it is stepping through is a multiple of the file input.

    // Then we ensure that it works by matching the results (number of gardens we can reach in X steps) to the given answer for the test input.
    // Steps 6, 10, 50, and 100 gives the correct answers as in the example
    //
    //     Step     Gardens reached
    //      6              16
    //      10             50
    //      50            1594
    //     100            6536

    // Then we have to notice a pattern. The width of each base map is 131 in the complete input.
    // Essentially, after 65 steps (width/2 - 1 because we start in the MIDDLE of the map) we reach the edges of the first base map, creating a diamond shape.
    // Then, for every 131st step after that the diamond's edges will have moved to the edges of the next 
    //  
    //                                         Step 64+2*131
    //                      Step 65+131            ..X..
    //    Step 65               .X.                .XOX.
    //       X                  XOX                XOOOX
    //                          .X.                .XOX.
    //                                             ..X..
    //
    // Step 65:       we have reached the edges of the middle base map.
    // Step 65+131:   we have filled the first middle part, and reached the edge of the next adjacent base maps above/below/left/right.
    // Step 65+2*131: we have filled all previous base maps and have reached the edges of the next adjacent base maps
    // and on and on and on and on
    // 
    // Because the pattern expands in all directions in two dimensions, this growth is quadratic in nature.
    // This means that we can make a quadratic regression of three datapoints (required for a second degree polynomial)
    //
    // To select the datapoints, we step through the map and save the result of stepping 65+131, 65+2*131, and 65+3*131.
    // Then we use a regression calculator to calculate the regression, and in that resulting quadratic function we insert 26501365.
    // !! Make sure the regression calculator has enough precision to display 10^14 numbers !!
    // (also, if R^2 is not 1, then not all points perfectly align with the regression. Then it will not produce the correct result)


    // Create the map.
    // The input data from the file is called the base map. The complete map is many repeated base maps in all directions.
    // The assignment says it should repeat to infinity, but for us it is sufficient with a large one, f0r example 111x111
    //   parameter explanation: width number doesn't really matter that much, as long as it's big enough not to hit any edges
    (MapTile[][] map, int baseMapWidth) = InterpretInputP2(File.ReadAllLines("./data_complete.txt"), multipliedWidth_MustBeOddNr: 111);

    int maxSteps = 99000; // a large number, doesn't really matter how much

    Map mapObject = new Map(map);
    List<(int step, int distinct)> distinctList = new(); // This will be used to create a regression
    for (int i = 1; i <= maxSteps; i++)
    {
        mapObject.Step(); //

        Console.SetCursorPosition(0, 0);
        Console.WriteLine("Step " + i);

        if (i > baseMapWidth) // every 20th step, record the data
        {
            if ((i + (baseMapWidth / 2)) % baseMapWidth == 0) // every 131st step from 65    (65+131, 65+262, 65+393)
                distinctList.Add((i, mapObject.Distinct));
        }
        if (distinctList.Count >= 3) break; // If we find our three values, break

        //if (i == maxSteps) PrintMap(map, 0, 3);
    }
    Console.WriteLine("\nDistinct: " + mapObject.Distinct + "\n");

    Console.WriteLine("Coordinates found for the quadratic regression:");

    for (int i = 0; i < distinctList.Count; i++)
    {
        var coord = distinctList[i];
        Console.WriteLine(string.Join(';', [coord.step.ToString(), coord.distinct.ToString()]));
    }
    Console.WriteLine();

    /*
    // Find maxtiles for a filled map
    int maxTiles = 0;
    for (int j = 0; j < map.Length; j += 1)
    {
        int even = (maxSteps % 2);

        int off = 0;
        if (even == 1) off = j % 2; 
        else off = 1 - (j % 2);

        for (int i = off; i < map[0].Length; i+=2)
        {
            (int x, int y) c = (i, j);
            if (map[j][i] != MapTile.Rock)
                maxTiles++;
        }
    }


    // 1 -> 8 -> 16
    int remainder = maxSteps;
    int nrOfFilledMaps = 1;
    int rememberi = 0;
    for (int i = 0; ; i++)
    {
        rememberi = i;
        if (i == 0)
        {
            remainder -= maxTiles;
            continue;
        }

        int newRemainder = remainder - maxTiles * 4 * i;
        if (newRemainder >= 0)
        {
            nrOfFilledMaps += 4 * i;
            remainder = newRemainder;
        }
        else break;
    }
    //Console.WriteLine("Remainder: " + remainder);

    //          ^
    //          X
    //        X X X
    //      X X X X X
    // <- X X X S X X X ->
    //      X X X X X
    //        X X X
    //          X
    //          v
    //
    //          |-----|
    //         rememberi


    // Calculate how many completed maps we will get

    // Create maps around it, and loop through them
    */



    /*
    PriorityQueue<(int x, int y)[], int> queue = new PriorityQueue<(int x, int y)[], int>();
    queue.Enqueue([start], 0);

    Console.SetCursorPosition(0, 0);
    Console.WriteLine("Step 0\n");
    PrintMap(map, 0, 2);

    for (int step = 1; step <= 26501365; step++)
    {
        Console.SetCursorPosition(0, 0);
        Console.WriteLine("Step " + step + "\n");

        // step
        var nextCoords = queue.Dequeue();
        List<(int x, int y)> toAdd = new List<(int x, int y)>();
        foreach (var coord in nextCoords)
        {
            toAdd.AddRange(GetNeighbours(coord, map));
        }

        // Update map
        (int x, int y)[] distinct = toAdd.Distinct().ToArray();
        queue.EnqueueRange([distinct], step);
        foreach (var item in nextCoords)
        {
            map[item.y][item.x] = MapTile.Garden;
        }
        foreach (var item in distinct)
        {
            map[item.y][item.x] = MapTile.Step;
        }
        if (map[start.y][start.x] != MapTile.Step) map[start.y][start.x] = MapTile.Start;


        PrintMap(map, 0, 2);

        Console.WriteLine($"Total plots available for {step} step(s) is {distinct.Count()}. \n > Press enter to step.");
    }


    void AdvanceMaps()
    {

    }*/
}



void Part1()
{
    //(MapTile[][] map, (int x, int y) start) = InterpretInput(File.ReadAllLines("./data_test.txt"));
    (MapTile[][] map, (int x, int y) start) = InterpretInputP1(File.ReadAllLines("./data_complete.txt"));
    
    PriorityQueue<(int x, int y)[], int> queue = new PriorityQueue<(int x, int y)[], int>();
    queue.Enqueue([start], 0);

    Console.SetCursorPosition(0, 0);
    Console.WriteLine("Step 0\n");
    PrintMap(map, 0, 2);

    for (int step = 1; step <= 5; step++)
    {
        //Console.ReadLine();
        Thread.Sleep(100);
        Console.SetCursorPosition(0, 0);
        Console.WriteLine("Step " + step + "\n");

        // step
        var nextCoords = queue.Dequeue();
        List<(int x, int y)> toAdd = new List<(int x, int y)>();
        foreach (var coord in nextCoords)
        {
            toAdd.AddRange(GetNeighbours(coord, map));
        }
        //List<(int x, int y)> notDistinct = toAdd.ToList();
        (int x, int y)[] distinct = toAdd.Distinct().ToArray();
        queue.EnqueueRange([distinct], step);
        foreach (var item in nextCoords)
        {
            map[item.y][item.x] = MapTile.Garden;
        }
        foreach (var item in distinct)
        {
            map[item.y][item.x] = MapTile.Step;
        }
        if (map[start.y][start.x] != MapTile.Step) map[start.y][start.x] = MapTile.Start;


        PrintMap(map, 0, 2);
        
        Console.WriteLine($"Total plots available for {step} step(s) is {distinct.Count()}. \n > Press enter to step.");
    }
}

(int x, int y)[] GetNeighbours((int x, int y) coord, MapTile[][] map)
{
    List<(int x, int y)> allNeighbours = new List<(int x, int y)>();

    (int x, int y) nextCoord;
    nextCoord = (coord.x + 1, coord.y);
    if (WithinBounds(nextCoord)) allNeighbours.Add(nextCoord);
    nextCoord = (coord.x, coord.y + 1);
    if (WithinBounds(nextCoord)) allNeighbours.Add(nextCoord);
    nextCoord = (coord.x - 1, coord.y);
    if (WithinBounds(nextCoord)) allNeighbours.Add(nextCoord);
    nextCoord = (coord.x, coord.y - 1);
    if (WithinBounds(nextCoord)) allNeighbours.Add(nextCoord);

    return allNeighbours.Where(c => map[c.y][c.x] != MapTile.Rock).ToArray();

    bool WithinBounds((int x, int y) cc)
    {
        if (cc.x < 0 || cc.y < 0 || cc.x >= map[0].Length || cc.y >= map.Length) return false;
        else return true;
    }
}

// Returns FALSE if any of the neighbours is out of bounds
bool GetNeighboursTF((int x, int y) coord, MapTile[][] map, out (int x, int y)[] neighbours)
{
    List<(int x, int y)> allNeighbours = new List<(int x, int y)>();

    (int x, int y) nextCoord;

    nextCoord = (coord.x + 1, coord.y);
    if (WithinBounds(nextCoord)) allNeighbours.Add(nextCoord);
    else return false;

    nextCoord = (coord.x, coord.y + 1);
    if (WithinBounds(nextCoord)) allNeighbours.Add(nextCoord);
    else return false;

    nextCoord = (coord.x - 1, coord.y);
    if (WithinBounds(nextCoord)) allNeighbours.Add(nextCoord);
    else return false;

    nextCoord = (coord.x, coord.y - 1);
    if (WithinBounds(nextCoord)) allNeighbours.Add(nextCoord);
    else return false;

    neighbours = allNeighbours.Where(c => map[c.y][c.x] != MapTile.Rock).ToArray();
    return true;

    bool WithinBounds((int x, int y) cc)
    {
        if (cc.x < 0 || cc.y < 0 || cc.x >= map[0].Length || cc.y >= map.Length) return false;
        else return true;
    }
}

(MapTile[][] map, (int x, int y) start) InterpretInputP1(string[] readAllLines)
{
    MapTile[][] map = new MapTile[readAllLines.Length][];
    (int, int) start = (0, 0);
    for (int j = 0; j < readAllLines.Length; j++)
    {
        MapTile[] mapRow = new MapTile[readAllLines[j].Length];
        for (int i = 0; i < readAllLines[j].Length; i++)
        {
            mapRow[i] = readAllLines[j][i] switch
            {
                '.' => MapTile.Garden,
                '#' => MapTile.Rock,
                'S' => MapTile.Start,
                _ => MapTile.Step
            };
            if (mapRow[i] == MapTile.Start) start = (i, j);
        }
        map[j] = mapRow;
    }
    return (map, start);
}

(MapTile[][], int) InterpretInputP2(string[] readAllLines, int multipliedWidth_MustBeOddNr)
{
    if (multipliedWidth_MustBeOddNr % 2 != 1)
    {
        Console.WriteLine("ERROR! Map width was an even number.");
        return (new MapTile[0][], -1);
    }

    (MapTile[][] baseMap, (int x, int y) startCoord) = InterpretInputP1(readAllLines);
    MapTile[][] bigmap = new MapTile[baseMap.Length * multipliedWidth_MustBeOddNr][];


    //(int, int) start = (startCoord.x * multipliedWidth, startCoord.y * multipliedWidth);
    for (int j = 0; j < baseMap.Length * multipliedWidth_MustBeOddNr; j++)
    {
        int baseIndexY = j % baseMap.Length;
        MapTile[] mapRow = new MapTile[baseMap[baseIndexY].Length * multipliedWidth_MustBeOddNr];
        for (int i = 0; i < baseMap[baseIndexY].Length * multipliedWidth_MustBeOddNr; i++)
        {
            int baseIndexX = i % baseMap[baseIndexY].Length;

            mapRow[i] = baseMap[baseIndexY][baseIndexX];
            if (mapRow[i] == MapTile.Start) mapRow[i] = MapTile.Garden; //start = (i, j);
        }
        bigmap[j] = mapRow;
    }

    return (bigmap, baseMap.Length);
}

void PrintMap(MapTile[][] map, int x, int y)
{
    Console.SetCursorPosition(x, y);
    string s = "";
    for (int j = 0; j < map.Length; j++)
    {
        if (j > 0) s += '\n';
        for (int i = 0; i < map[0].Length; i++)
        {
            s += map[j][i] switch
            {
                MapTile.Garden => '.',
                MapTile.Rock => '#',
                MapTile.Start => 'S',
                _ => 'O'
            };
        }
    }
    Console.WriteLine(s);
}

(int x, int y) MapStartFromDirection(Direction d, MapTile[][] mapTiles)
{
    (int x, int y) midPoint = ((mapTiles[0].Length - 1) / 2, (mapTiles.Length - 1) / 2);

    return d switch
    {
        Direction.N => (midPoint.x, mapTiles.Length - 1),
        Direction.NE => (0, mapTiles.Length - 1),
        Direction.E => (0, midPoint.y),
        Direction.SE => (0, 0),
        Direction.S => (midPoint.x, 0),
        Direction.SW => (mapTiles[0].Length - 1, 0),
        Direction.W => (mapTiles[0].Length - 1, midPoint.y),
        Direction.NW => (mapTiles[0].Length - 1, mapTiles.Length - 1),
        _ => (0, 0)
    };
}

Direction OppositeDir(Direction d)
{
    switch (d)
    {
        case Direction.N:
            return Direction.S;
        case Direction.NE:
            return Direction.SW;
        case Direction.E:
            return Direction.W;
        case Direction.SE:
            return Direction.NW;
        case Direction.S:
            return Direction.N;
        case Direction.SW:
            return Direction.NE;
        case Direction.W:
            return Direction.E;
        case Direction.NW:
            return Direction.SE;
        default:
            return d;
    }
}

enum Direction
{
    N,
    NE,
    E,
    SE,
    S,
    SW,
    W,
    NW
}

enum MapTile
{
    Start,
    Garden,
    Rock,
    Step
}