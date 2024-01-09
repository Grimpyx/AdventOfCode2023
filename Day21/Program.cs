// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");


using Day21;
using System.Collections.Generic;
using System.Diagnostics;

//Part1(); // Answer is 38XX
Part2();
void Part2()
{
    //int maxSteps = 26501365;
    int maxSteps = 100;

    // Create first map.
    MapTile[][] map = InterpretInputP2(File.ReadAllLines("./data_test.txt"));
    //PrintMap(map, 1, 1);
    Console.WriteLine();

    Map mapObject = new Map(map, MapStartFromDirection(Direction.SW, map));
    //Map mapObject = new Map(map);

    for (int i = 1; i <= maxSteps; i++)
    {
        mapObject.Step();
        if (i == maxSteps)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Step " + i);
            PrintMap(map, 0, 3);
        }
    }
    Console.WriteLine("\nDistinct: " + mapObject.Distinct);
    Console.ReadLine();

    //(MapTile[][] map, (int x, int y) start) = InterpretInput(File.ReadAllLines("./data_complete.txt"));
    /*
    Dictionary<Direction, int> filledDirections = new Dictionary<Direction, int>();
    Direction[] allDirections = [Direction.N, Direction.NE, Direction.E, Direction.SE, Direction.S, Direction.SW, Direction.W, Direction.NW];

    for (int i = 0; i < allDirections.Length; i++)
    {
        Direction d = allDirections[i];
        MapTile[][] tempMap = map;
        (int x, int y) startCoord = MapCoordFromDirection(d, start, map);


        PriorityQueue<(int x, int y)[], int> q1 = new PriorityQueue<(int x, int y)[], int>();
        q1.Enqueue([startCoord], 0);
        int currentDistinct = 0;
        for (int step = 1; ; step++)
        {
            // step
            var nextCoords = q1.Dequeue();
            List<(int x, int y)> toAdd = new List<(int x, int y)>();
            
            bool hitEdge = false;
            foreach (var coord in nextCoords)
            {
                if (GetNeighboursTF(coord, map, out (int x, int y)[] neighs))
                    toAdd.AddRange(neighs);
                else
                {
                    hitEdge = true;
                    //PrintMap(map, 0, 2);
                    break;
                }
            }
            if (hitEdge)
            {
                filledDirections.Add(d, currentDistinct);
                break;
            }

            // Update map
            (int x, int y)[] distinct = toAdd.Distinct().ToArray();
            q1.EnqueueRange([distinct], step);
            foreach (var item in nextCoords)
            {
                tempMap[item.y][item.x] = MapTile.Garden;
            }
            foreach (var item in distinct)
            {
                tempMap[item.y][item.x] = MapTile.Step;
            }
            if (tempMap[start.y][start.x] != MapTile.Step) tempMap[start.y][start.x] = MapTile.Start;
            
            currentDistinct = distinct.Length;
        }
    }

    */



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
    Console.WriteLine("Remainder: " + remainder);

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
    }*/


    void AdvanceMaps()
    {

    }
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

MapTile[][] InterpretInputP2(string[] readAllLines)
{

    (MapTile[][] baseMap, (int x, int y) startCoord) = InterpretInputP1(readAllLines);
    MapTile[][] bigmap = new MapTile[baseMap.Length * 3][];


    //(int, int) start = (startCoord.x * 3, startCoord.y * 3);
    for (int j = 0; j < baseMap.Length * 3; j++)
    {
        int baseIndexY = j % baseMap.Length;
        MapTile[] mapRow = new MapTile[baseMap[baseIndexY].Length * 3];
        for (int i = 0; i < baseMap[baseIndexY].Length * 3; i++)
        {
            int baseIndexX = i % baseMap[baseIndexY].Length;

            mapRow[i] = baseMap[baseIndexY][baseIndexX];
            if (mapRow[i] == MapTile.Start) mapRow[i] = MapTile.Garden; //start = (i, j);
        }
        bigmap[j] = mapRow;
    }

    return bigmap;
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