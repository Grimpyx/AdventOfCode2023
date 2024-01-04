
using System.Collections.Generic;
using System.Diagnostics;

// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

// Part 1 12XX was correct
// Part 2 13XX was correct

//string[] data = File.ReadAllLines("./data_test.txt");
string[] data = File.ReadAllLines("./data_complete.txt");
int[][] heatMap = new int[data.Length][];
Console.ForegroundColor = ConsoleColor.Gray;
for (int y = 0; y < data.Length; y++)
{
    int[] row = new int[data[0].Length];
    for (int x = 0; x < data[0].Length; x++)
    {
        row[x] = data[y][x] - '0';
        Console.Write(row[x]);
    }
    heatMap[y] = row;
    Console.WriteLine();
}
Console.ForegroundColor = ConsoleColor.Green;

T(heatMap, 0, 0, 1, 3); // P1
T(heatMap, 0, 0, 4, 10); // P2

void T(int[][] heatMap, int startX, int startY, int minimumSteps, int maximumSteps)
{
    //Dictionary<(int x, int y), int> distances = new(); // <(node), distance>
    //Dictionary<(int x, int y, Direction dir, int stepsAlreadyWalked), int> visitedNodes = new(); // <(node), heat>
    
    
    PriorityQueue<(int x, int y, Direction dir, int stepsAlreadyWalked), int> queue = new();

    // For each coordinate in the map, there will be a dictionary over directions leading there, consequtive steps, and heat
    Dictionary<(Direction, int), int>[][] visitedNodes = new Dictionary<(Direction, int), int>[heatMap.Length][];
    for (var y = 0; y < heatMap.Length; y++)
    {
        visitedNodes[y] = new Dictionary<(Direction, int), int>[heatMap[0].Length];
        for (var x = 0; x < heatMap[0].Length; x++)
            visitedNodes[y][x] = [];
    } // we just made all [y][x] accessible

    // Queue the start
    queue.Enqueue((0, 0, Direction.S, 0), 0);
    queue.Enqueue((0, 0, Direction.E, 0), 0);


    while (queue.Count > 0)
    {
        // Pick next from queue
        var fromQueue = queue.Dequeue();

        // Skip if visited
        //if (visitedNodes[fromQueue.x][fromQueue.y].Keys.Contains(fromQueue)) continue;
        //visitedNodes.Add(fromQueue);

        // Check neighbors if they need to be updated
        int heat = visitedNodes[fromQueue.y][fromQueue.x].GetValueOrDefault((fromQueue.dir, fromQueue.stepsAlreadyWalked));


        if (fromQueue.stepsAlreadyWalked < maximumSteps) // keep walking forward
            Move(fromQueue.y, fromQueue.x, fromQueue.dir, heat, fromQueue.stepsAlreadyWalked);
        if (fromQueue.stepsAlreadyWalked >= minimumSteps) // If exceeding maxSteps we try turning
        {
            Move(fromQueue.y, fromQueue.x, L90(fromQueue.dir), heat, 0);
            Move(fromQueue.y, fromQueue.x, R90(fromQueue.dir), heat, 0);
        }
    }


    var maxY = heatMap.Length - 1;
    var maxX = heatMap[0].Length - 1;

    Console.SetCursorPosition(0, maxY + 2);

    Console.WriteLine(" > Result: " + visitedNodes[maxY][maxX].Min(x => x.Value));
    Console.WriteLine(" > Result: " + visitedNodes[maxY][maxX].Values.Min());



    void Move(int y, int x, Direction direction, int heat, int directionMoves)
    {
        //if (d == DirectionOpposite(direction)) return;

        int dx = direction switch
        {
            Direction.E => 1,
            Direction.W => -1,
            _ => 0
        };
        int dy = direction switch
        {
            Direction.N => -1,
            Direction.S => 1,
            _ => 0
        };

        int newX = x + dx;
        int newY = y + dy;
        int newDirectionMoves = directionMoves + 1; //isConsequtiveMove ? fromQueue.stepsAlreadyWalked + 1 : 0;
        if (newY < 0 || newY >= heatMap.Length || newX < 0 || newX >= heatMap[0].Length || newDirectionMoves > maximumSteps) return;
        heat += heatMap[newY][newX];

        var nodeVisits = visitedNodes[newY][newX];
        //if (heat < allWaysWeVisitedNewNode.Values.Min())

        if (nodeVisits.TryGetValue((direction, newDirectionMoves), out int visitedHeat))
        {
            if (visitedHeat <= heat) return;
        }

        Console.SetCursorPosition(newX, newY);
        Console.Write("X");
        queue.Enqueue((newX, newY, direction, newDirectionMoves), heat);
        nodeVisits[(direction, newDirectionMoves)] = heat;
    }
}





void Algo(int[,] graph, int src)
{
    int V = (int)graph.LongLength;

    int[] dist = new int[V];        // distances
    bool[] sptSet = new bool[V];    // if step is included in spt

    for (int i = 0; i < V; i++)
    {
        dist[i] = int.MaxValue;
        sptSet[i] = false;
    }

    dist[src] = 0;

    for (int count = 0; count < V - 1; count++)
    {
        int u = MinDistance(dist, sptSet);

        sptSet[u] = true;

        for (int v = 0; v < V; v++)
        {
            if (!sptSet[v] && graph[u, v] != 0
                    && dist[u] != int.MaxValue
                    && dist[u] + graph[u, v] < dist[v])
                dist[v] = dist[u] + graph[u, v];
        }
    }

    printSolution(dist);



    int MinDistance(int[] dist, bool[] sptSet)
    {
        // Initialize min value
        int min = int.MaxValue, min_index = -1;

        for (int v = 0; v < V; v++)
            if (sptSet[v] == false && dist[v] <= min)
            {
                min = dist[v];
                min_index = v;
            }

        return min_index;
    }

    void printSolution(int[] dist)
    {
        Console.Write("Vertex \t\t Distance "
                      + "from Source\n");
        for (int i = 0; i < V; i++)
            Console.Write(i + " \t\t " + dist[i] + "\n");
    }
}
Direction DirectionOpposite(Direction input)
{
    switch (input)
    {
        case Direction.N:
            return Direction.S;
        case Direction.E:
            return Direction.W;
        case Direction.S:
            return Direction.N;
        case Direction.W:
            return Direction.E;
        default:
            break;
    }
    return default;
}


Direction L90(Direction direction) => direction switch
{
    Direction.N => Direction.W,
    Direction.W => Direction.S,
    Direction.S => Direction.E,
    Direction.E => Direction.N,
    _ => throw new UnreachableException()
};
Direction R90(Direction direction) => direction switch
{
    Direction.N => Direction.E,
    Direction.E => Direction.S,
    Direction.S => Direction.W,
    Direction.W => Direction.N,
    _ => throw new UnreachableException()
};

enum Direction
{
    N = 0,
    E = 2,
    S = 1,
    W = 3
}
