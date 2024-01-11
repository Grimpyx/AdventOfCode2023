using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V2 = (int x, int y);

namespace Day23
{
    class Intersection
    {
        V2 pos;
        HashSet<(V2 pos, char symbol)> allAdjacent;
        HashSet<(V2 pos, char symbol)> pointsLeadingIn;
        HashSet<(V2 pos, char symbol)> pointsLeadingOut;

        public List<int> StepsToReachFromLeadingIn { get; set; }

        public Intersection((int x, int y) pos, char[][] map)
        {
            this.pos = pos;

            allAdjacent = new HashSet<(V2 pos, char symbol)>();
            pointsLeadingIn = new HashSet<(V2 pos, char symbol)>();
            pointsLeadingOut = new HashSet<(V2 pos, char symbol)>();

            V2[] dir = [(-1, 0), (1, 0), (0, -1), (0, 1)];
            for (int i = 0; i < dir.Length; i++)
            {
                V2 globalPos = (pos.x + dir[i].x, pos.y + dir[i].y);
                char c = map[globalPos.y][globalPos.x];
                var addObject = (globalPos, c);

                allAdjacent.Add(addObject);
                if (CharToDirFROM(c) == dir[i]) pointsLeadingIn.Add(addObject);
                else if (CharToDirTO(c) == dir[i]) pointsLeadingOut.Add(addObject);

            }

            StepsToReachFromLeadingIn = new List<int>();
        }

        public (int x, int y) Pos { get => pos; }
        public HashSet<((int x, int y) pos, char symbol)> AllAdjacent { get => allAdjacent; }
        public HashSet<((int x, int y) pos, char symbol)> PointsLeadingIn { get => pointsLeadingIn; }
        public HashSet<((int x, int y) pos, char symbol)> PointsLeadingOut { get => pointsLeadingOut; }

        public override bool Equals(object? obj)
        {
            return obj is Intersection intersection &&
                   pos.Equals(intersection.pos);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(pos);
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

        public override string? ToString()
        {
            return $"{pos.x}, {pos.y}";
        }
    }
    class Node
    {
        //static HashSet<Node> allNodes;
        public static Dictionary<V2, Node> allNodesDict;
        static int idTracker;
        private int id;

        private V2 position;
        public Dictionary<Node, int> connectedNodes;
        V2[] connectedPoints;

        public Node((int x, int y) position, V2[] connectedPoints) //, List<(Node, int)> connectedNodes
        {
            this.position = position;
            //this.connectedNodes = connectedNodes;
            this.connectedPoints = connectedPoints;
            connectedNodes = new Dictionary<Node, int>();

            id = idTracker;
            idTracker++;

            //if (allNodes == null) allNodes = new HashSet<Node>();
            //allNodes.Add(this);
            if (allNodesDict == null) allNodesDict = new Dictionary<V2, Node>();
            allNodesDict.Add(position, this);
        }

        public void Populate(char[][] map)
        {
            int stepCounter = 1;

            Dictionary<V2, int> visited = new Dictionary<V2, int>() { { position, 0 } };
            Queue<List<V2>> queue = new Queue<List<V2>>();
            List<V2> surroundingFirst = [..connectedPoints]; //Surrounding(position, [(-1, 0), (1, 0), (0, -1), (0, 1)]);
            queue.Enqueue(surroundingFirst);
            //surroundingFirst.ForEach(x => visited.Add(x, stepCounter));

            Console.SetCursorPosition(position.x, position.y);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write('O');

            while (queue.Count > 0)
            {
                List<V2> v2s = queue.Dequeue();

                List<V2> nextQueue = new List<V2>();
                foreach (V2 v in v2s)
                {
                    if (visited.ContainsKey(v))
                        continue;

                    if (allNodesDict.Keys.Contains(v)) // then is a node/intersection
                    {
                        Node otherNode = allNodesDict[v];
                        connectedNodes.Add(otherNode, stepCounter);
                        continue;
                    }

                    Console.SetCursorPosition(v.x, v.y);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write('O');

                    List<V2> surrounding = Surrounding(v, [(-1, 0), (1, 0), (0, -1), (0, 1)]);

                    nextQueue.AddRange(surrounding);
                    visited.Add(v, stepCounter);
                }
                if (nextQueue.Count == 0)
                    continue;

                queue.Enqueue(nextQueue);
                //nextQueue.ForEach(x => visited.Add(x, stepCounter));
                stepCounter++;
            }

            Console.SetCursorPosition(0, map.Length + 1);
            Console.ForegroundColor = ConsoleColor.White;
            

            List<V2> Surrounding(V2 v, V2[] directions) // [(-1, 0), (1, 0), (0, -1)]
            {
                List<V2> surrounding = new List<V2>();
                for (int i = 0; i < directions.Length; i++)
                {
                    V2 globalPos = (v.x + directions[i].x, v.y + directions[i].y);
                    if (map[globalPos.y][globalPos.x] != '#')
                        surrounding.Add(globalPos);
                }
                return surrounding;
            }
        }


        public override bool Equals(object? obj)
        {
            return obj is Node node &&
                   id == node.id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(position, connectedNodes);
        }

        public override string? ToString()
        {
            return $"ID:{id} ({position.x}, {position.y})";
        }
    }
}
