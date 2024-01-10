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
}
