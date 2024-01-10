using Day23;
using System.Linq;
using V2 = (int x, int y);


bool useCompleteData = true;

string testDataPath = "./data_test.txt";
string completeDataPath = "./data_complete.txt";


//Part2();
Part1();

void Part2()
{

}

void Part1()
{
    bool writeMap = true;

    char[][] map;
    HashSet<Intersection> intersections;            // Ways to obtain data from
    HashSet<V2> intersectionPositions;              // intersections quickly.
    Dictionary<V2, Intersection> intersectionDict;  //

    if (useCompleteData) (map, intersections) = InterpretDataP1(completeDataPath);
    else (map, intersections) = InterpretDataP1(testDataPath);

    intersectionPositions = intersections.Select(x => x.Pos).ToHashSet();
    intersectionDict = intersections.Select(x => (x.Pos, x)).ToDictionary();

    // Start point is always (1,0)
    //   End point is always (max_X - 1, max_Y)
    V2 startCoordinate = (1, 0);
    V2 endCoordinate = (map[0].Length - 2, map.Length - 1);

    Dictionary<V2, int> visited = new Dictionary<V2, int>();
    PriorityQueue<List<V2>, int> queue = new PriorityQueue<List<V2>, int>();

    // Queue the start coordinate
    queue.Enqueue([startCoordinate], 0);

    int stepsTaken = 0;
    while (queue.Count > 0)
    {
        var vs = queue.Dequeue();

        List<V2> nextToQueue = new List<V2>();

        for (int i = 0; i < vs.Count; i++)
        {
            var v = vs[i];

            // If this point is in the middle of an intersection we might want to do something different.
            // For the intersections that have multiple ways to reach it, we know that last path to reach
            // the intersection is the longest path there. So if we reach the intersection any of the
            // first times, we know for sure that this path should stop right.
            // (This works because we dont have any dead ends)
            if (intersectionPositions.Contains(v))
            {
                Intersection inter = intersectionDict[v];
                if (inter.PointsLeadingIn.Count > 1 &&                                   // Contains more than one way to reach the intersection
                    inter.StepsToReachFromLeadingIn.Count < inter.PointsLeadingIn.Count) // the times we have reached it before
                {
                    // If more than one way to reach this intersection
                    // AND not all paths has reached it yet (meaning this path is the shortest)
                    inter.StepsToReachFromLeadingIn.Add(stepsTaken);
                    if (inter.StepsToReachFromLeadingIn.Count != inter.PointsLeadingIn.Count)
                    {
                        if (writeMap)
                        {
                            V2 previous = (-1, -1);
                            V2[] surrounding = [(v.x + 1, v.y), (v.x + -1, v.y), (v.x, v.y + 1), (v.x, v.y - 1)];
                            for (int j = 0; j < surrounding.Length; j++)
                            {
                                if (visited.ContainsKey(surrounding[j])) previous = surrounding[j];
                            }

                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.SetCursorPosition(previous.x, previous.y);
                            Console.Write('X');
                        }

                        // By continuing, we don't add anything new to the queue.
                        // i.e. we stop walking this path until the longest path catches up.
                        continue;
                    }
                }
            }

            // Add the position to the visited nodes. If already visited, return
            // (one way to stop the path from walking backwards)
            if (visited.ContainsKey(v))
                continue;
            visited.Add(v, stepsTaken);

            // If we want to visualize the paths in
            if (writeMap)
            {
                if (intersectionPositions.Contains(v)) Console.ForegroundColor = ConsoleColor.Red;
                else if (v == startCoordinate) Console.ForegroundColor = ConsoleColor.Magenta;
                else if (v == endCoordinate) Console.ForegroundColor = ConsoleColor.Yellow;
                else if (map[v.y][v.x] == '.') Console.ForegroundColor = ConsoleColor.DarkGray;
                else Console.ForegroundColor = ConsoleColor.DarkGreen;

                Console.SetCursorPosition(v.x, v.y);
                if (map[v.y][v.x] == '.') Console.Write('O');
                else Console.Write(map[v.y][v.x]);
            }


            // Find and queue neighbours
            V2[] adjacentDirToCheck = [(1, 0), (0, 1), (-1, 0), (0, -1)]; //[direction, L90(direction), R90(direction)];
            List<(V2, char)> adjacentPosAndChar = new List<(V2, char)>();
            for (int dirIndex = 0; dirIndex < adjacentDirToCheck.Length; dirIndex++)
            {
                V2 adjacentGlobalPos = (v.x + adjacentDirToCheck[dirIndex].x, v.y + adjacentDirToCheck[dirIndex].y);
                if (IsWithinBoundsOfMap(adjacentGlobalPos, map))
                {
                    char adjacentChar = map[adjacentGlobalPos.y][adjacentGlobalPos.x];
                    if (adjacentChar == '#') continue;
                    if (adjacentChar == '.' || (adjacentChar != '#' && DirToCharOUT(adjacentDirToCheck[dirIndex]) == adjacentChar))
                    {
                        nextToQueue.Add(adjacentGlobalPos);
                    }
                }
            }
        }
        stepsTaken++;

        if (nextToQueue.Any())
            queue.Enqueue(nextToQueue, stepsTaken);
    }


    // Actually construct the longest path by walking it backward
    HashSet<V2> completePath = new HashSet<V2>();
    V2 currentCoord = endCoordinate;
    while (currentCoord != startCoordinate)
    {
        completePath.Add(currentCoord);

        if (writeMap)
        {
            if (currentCoord == endCoordinate) Console.ForegroundColor = ConsoleColor.Yellow;
            else Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(currentCoord.x, currentCoord.y);
            Console.Write('O');
        }

        List<V2> surrounding = [(currentCoord.x + 1, currentCoord.y), (currentCoord.x + -1, currentCoord.y), (currentCoord.x, currentCoord.y + 1), (currentCoord.x, currentCoord.y - 1)];

        V2 next = (-1, -1);
        int longestLength = -1;

        // Remove surrounding points that dont go in the "reverse" direction
        if (intersectionPositions.Contains(currentCoord))
        {
            Intersection inter = intersectionDict[currentCoord];
            foreach (var sur in surrounding.ToArray())
            {
                if (!inter.PointsLeadingIn.Select(x => x.pos).Contains(sur)) surrounding.Remove(sur);
            }
        }

        // Loop through all surrounding points and choose the one with longest path
        for (int j = 0; j < surrounding.Count; j++)
        {
            if (visited.ContainsKey(surrounding[j]) && !completePath.Contains(surrounding[j]))
            {
                int l = visited[surrounding[j]];
                if (longestLength < l)
                {
                    longestLength = l;
                    next = surrounding[j];
                }
            };
        }
        currentCoord = next;
    }

    // Reset color and move cursor to end of map
    if (writeMap)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(0, map.Length);
    }

    Console.WriteLine(" > Steps: " + visited[endCoordinate]);
    Console.WriteLine(" > Actual path length: " + completePath.Count);
}

bool IsWithinBoundsOfMap(V2 v, char[][] map)
{
    if (v.x < 0 || v.y < 0)
        return false;
    if (v.x >= map[0].Length || v.y >= map.Length)
        return false;
    return true;
}

(char[][] map, HashSet<Intersection> intersections) InterpretDataP1(string path)
{
    string[] data = File.ReadAllLines(path);

    List<V2> intersectionParts = new List<V2>();
    HashSet<Intersection> intersections = new HashSet<Intersection>();

    char[][] map = new char[data.Length][];
    for (int j = 0; j < data.Length; j++)
    {
        char[] row = new char[data[0].Length];
        for (int i = 0; i < data[0].Length; i++)
        {
            char d = data[j][i];
            row[i] = d;

            if (d == '>' || d == '<' || d == 'v' || d == '^') intersectionParts.Add((i, j));
        }
        map[j] = row;
    }

    for (int i = 0; i < intersectionParts.Count; i++)
    {
        V2 beforeIntersection = CharToDirFROM(map[intersectionParts[i].y][intersectionParts[i].x]);
        beforeIntersection.x += intersectionParts[i].x;
        beforeIntersection.y += intersectionParts[i].y;

        if (map[beforeIntersection.y][beforeIntersection.x] == '#') continue;

        List<V2> neighbours = GetValidNeighbours(beforeIntersection);
        if (neighbours.Count >= 2) // then it is an intersection
        {
            if (!intersections.Select(x => x.Pos).Contains(beforeIntersection))
                intersections.Add(new Intersection(beforeIntersection, map));
        }


        /*if (NumberOfNeighborsLeadingAway(beforeIntersection) > 1)
            intersections.Add(beforeIntersection);


        int NumberOfNeighborsLeadingAway(V2 startCoord)
        {
            if (startCoord.x < 0 || startCoord.y < 0)
                return 0;
            if (startCoord.x >= map[0].Length || startCoord.y >= map.Length)
                return 0;

            int tracker = 0;
            //V2[] dir = [(-1, 0), (1, 0), (0, -1), (0, 1)];
            char[] dirChar = ['<', '>', '^', 'v'];
            for (int i = 0; i < dirChar.Length; i++)
            {
                V2 nCoord = (startCoord.x + CharToDirTO(dirChar[i]).x, startCoord.y + CharToDirTO(dirChar[i]).y);
                if (dirChar[i] == map[nCoord.y][nCoord.x])
                {
                    tracker++;
                }
            }
            return tracker;
        }*/

        List<V2> GetValidNeighbours(V2 v)
        {
            List<V2> validNeighbours = new List<V2>();

            V2[] dir = [(-1, 0), (1, 0), (0, -1), (0, 1)];
            for (int i = 0; i < dir.Length; i++)
            {
                V2 globalPos = (v.x + dir[i].x, v.y + dir[i].y);
                if (map[globalPos.y][globalPos.x] != '#' && map[globalPos.y][globalPos.x] != '.') validNeighbours.Add(globalPos);
            }
            return validNeighbours;
        }
    }

    return (map, intersections);
}
V2 CharToDirTO(char c)
{
    return c switch
    {
        '<' => (-1, 0),
        '>' => (1, 0),
        'v' => (0, 1),
        '^' => (0, -1),
        _ => (0, 0)
    };
}
V2 CharToDirFROM(char c)
{
    return c switch
    {
        '<' => (1, 0),
        '>' => (-1, 0),
        'v' => (0, -1),
        '^' => (0, 1),
        _ => (0, 0)
    };
}
char DirToCharOUT(V2 v)
{
    return v switch
    {
        (-1, 0) => '<',
        (1, 0) => '>',
        (0, 1) => 'v',
        (0, -1) => '^',
        _ => 'X'
    };
}

V2 L90(V2 v)
{
    v = (v.x, -v.y); // because up and down is flipped we have to do this
    V2 vL90 = (-v.y, v.x);
    return (vL90.x, -vL90.y);
}
V2 R90(V2 v)
{
    v = (v.x, -v.y); // because up and down is flipped we have to do this
    V2 vR90 = (v.y, -v.x);
    return (vR90.x, -vR90.y);
}