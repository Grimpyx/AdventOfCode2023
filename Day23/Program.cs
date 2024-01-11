using Day23;
using System.Collections;
using System.Linq;
using System.Xml.Linq;
using V2 = (int x, int y);


bool useCompleteData = true;

string testDataPath = "./data_test.txt";
string completeDataPath = "./data_complete.txt";


Part2();
//Part1();

void Part2()
{
    HashSet<Node> nodes;            // Ways to obtain data from
    char[][] map;
    if (useCompleteData) (map, nodes) = InterpretDataP2(completeDataPath);
    else (map, nodes) = InterpretDataP2(testDataPath);

    V2 startCoordinate = (1, 0);
    V2 endCoordinate = (map[0].Length - 2, map.Length - 1);
    FindLongestPath(startCoordinate, endCoordinate);

    void FindLongestPath(V2 start, V2 end)
    {
        Node startNode = Node.allNodesDict[start];
        Node endNode = Node.allNodesDict[end];

        List<(List<Node> nodes, int length)> completedPaths = new List<(List<Node> nodes, int length)>();

        // All paths currently being explored is stored here.
        Queue<(Node currentNode, HashSet<Node> visitedNodes, int length)> paths = new Queue<(Node currentNode, HashSet<Node> visitedNodes, int length)>();
        paths.Enqueue((startNode, [], 0)); // Queue start for our first path.

        while (paths.Count > 0)
        {
            var currentPath = paths.Dequeue();

            var unvisitedNeighbours = currentPath.currentNode.connectedNodes.Keys.Except(currentPath.visitedNodes);
            if (unvisitedNeighbours.Any()) // If we have any unvisited neighbours
            {
                // For each unvisited neighbour we create a copy of the current path and advances it to that neighbour
                foreach (var n in unvisitedNeighbours)
                {
                    HashSet<Node> newVisitedNodes = [.. currentPath.visitedNodes];
                    newVisitedNodes.Add(currentPath.currentNode);
                    paths.Enqueue((n, newVisitedNodes, currentPath.length + currentPath.currentNode.connectedNodes[n]));
                }
            }
            else // Reached dead end
            {
                // Check if at end
                if (currentPath.currentNode == endNode)
                {
                    List<Node> completedPathNodes = [.. currentPath.visitedNodes];
                    completedPathNodes.Add(currentPath.currentNode);
                    completedPaths.Add((completedPathNodes, currentPath.length));
                }

                continue;
            }
        }
        /*foreach (var p in completedPaths) // Takes way too long
        {
            Console.WriteLine("Path length: " + p.length);
        }*/
        Console.WriteLine("Longest path length: " + completedPaths.Select(x => x.length).Max());
    }
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

(char[][] map, HashSet<Node> nodes) InterpretDataP2(string path)
{
    string[] data = File.ReadAllLines(path);

    HashSet<Node> nodes = new HashSet<Node>();

    char[][] map = new char[data.Length][];
    for (int j = 0; j < data.Length; j++)
    {
        char[] row = new char[data[0].Length];
        for (int i = 0; i < data[0].Length; i++)
        {
            char d;
            d = data[j][i];
            row[i] = d;

            // In part 2 we dont care about these 'v' '^' '<' '>'
            // Also, instead of "Intersections" I created a list of Nodes instead that I intend to use more like a graph
            if (j > 1 && i > 1 && j < map.Length - 1 && i < map[0].Length - 1) // If we're NOT on the edge of the map, at least one step in
            {
                // How this looks during iteration:
                // Completed map -> #.#####################
                // Completed map -> #.................#####  <- y=(j-1)
                //   Current row -> #########.
                //                           ^ (i, j)
                // This becomes:
                //                  #.#####################
                //                  #.......XIX.......#####  I is the intersection position
                //                  #########X               X are the connected points (List<V2> surrounding)
                List<V2> surrounding = Surrounding(v: (i,j-1), excludedChar: '#', below: data[j][i]); // j-1 because we look at the above tile
                if (surrounding.Count > 2)
                    nodes.Add(new Node((i, j - 1), surrounding.ToArray()));
            }
        }
        map[j] = row;
    }

    // All intersections in the map will be nodes, but we
    // also need to count the start and the end as nodes too.
    V2 startCoordinate = (1, 0);
    V2 endCoordinate = (map[0].Length - 2, map.Length - 1);
    nodes.Add(new Node(startCoordinate, [(1, 1)]));
    nodes.Add(new Node(endCoordinate, [(endCoordinate.x, endCoordinate.y - 1)]));

    foreach (var node in nodes)
    {
        // Find all connections between the nodes
        node.Populate(map);
    }

    return (map, nodes);

    List<V2> Surrounding(V2 v, char excludedChar, char below)
    {
        if (map[v.y][v.x] == excludedChar) return new List<V2>();

        List<V2> surrounding = new List<V2>();

        V2[] dir = [(-1, 0), (1, 0), (0, -1)];
        for (int i = 0; i < dir.Length; i++)
        {
            V2 globalPos = (v.x + dir[i].x, v.y + dir[i].y);
            if (map[globalPos.y][globalPos.x] != excludedChar)
                surrounding.Add(globalPos);
        }
        if (below != excludedChar)
            surrounding.Add((v.x, v.y + 1));

        return surrounding;
    }
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
            char d;
            d = data[j][i];
            row[i] = d;

            if (data[j][i] == '>' || data[j][i] == '<' || data[j][i] == 'v' || data[j][i] == '^') intersectionParts.Add((i, j));
        }
        map[j] = row;
    }

    for (int i = 0; i < intersectionParts.Count; i++)
    {
        V2 beforeIntersection = CharToDirFROM(map[intersectionParts[i].y][intersectionParts[i].x]);
        beforeIntersection.x += intersectionParts[i].x;
        beforeIntersection.y += intersectionParts[i].y;

        if (map[beforeIntersection.y][beforeIntersection.x] == '#') continue;

        // Find all valid neighbours (not # or .). Having more than 1 of these symbols <>^v it means we have found an intersection
        List<V2> neighbours = GetValidNeighbours(beforeIntersection);
        if (neighbours.Count >= 2) // then it is an intersection
        {
            if (!intersections.Select(inters => inters.Pos).Contains(beforeIntersection))
                intersections.Add(new Intersection(beforeIntersection, map));
        }

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