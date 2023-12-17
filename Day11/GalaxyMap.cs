using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Day10;

namespace Day11
{
    public class GalaxyMap
    {
        //int[,] galaxy;
        //List<Galaxy> galaxyList;
        Dictionary<int, Galaxy> galaxyDict;
        int maxGalaxy_x;
        int maxGalaxy_y;



        public GalaxyMap(string[] readAllLines)
        {
            int idCounter = 0;
            galaxyDict = new Dictionary<int, Galaxy>();

            for (int y = 0; y < readAllLines.Length; y++)
            {
                for (int x = 0; x < readAllLines[0].Length; x++)
                {
                    if (readAllLines[y][x] == '#')
                    {
                        galaxyDict.Add(idCounter, new Galaxy(idCounter, new Vector2(x, y)));
                        idCounter++;
                    }
                }
            }

            maxGalaxy_x = (int)galaxyDict.Values.Select(g => g.position.x).Max() + 1;
            maxGalaxy_y = (int)galaxyDict.Values.Select(g => g.position.y).Max() + 1;
        }

        public void Expand()
        {
            Galaxy[] galaxies = [.. galaxyDict.Values];
            Vector2[] galaxyPositions = galaxyDict.Values.Select(g => g.position).ToArray();
            float[] allX = galaxyPositions.Select(x => x.x).ToArray();
            float[] allY = galaxyPositions.Select(x => x.y).ToArray();
            //bool[] columnContainsGalaxy = new bool[maxGalaxy_x];
            //bool[] rowContainsGalaxy = new bool[maxGalaxy_y];
            List<Galaxy> galaxiesToIncrement_x = new List<Galaxy>();
            List<Galaxy> galaxiesToIncrement_y = new List<Galaxy>();


            for (int y = 0; y < maxGalaxy_y; y++)
            {
                if (!allY.Contains(y))
                {
                    //Console.WriteLine("Does not contain galaxy in y = " + y);
                    foreach (Galaxy g in galaxies)
                    {
                        if (g.position.y >= y) galaxiesToIncrement_y.Add(galaxyDict[g.id]);
                    }
                }
            }
            for (int x = 0; x < maxGalaxy_x; x++)
            {
                if (!allX.Contains(x))
                {
                    //Console.WriteLine("Does not contain galaxy in x = " + x);
                    foreach (Galaxy g in galaxies)
                    {
                        if (g.position.x >= x) galaxiesToIncrement_x.Add(galaxyDict[g.id]);
                    }
                }
            }

            // Update the galaxy list 
            foreach (var item in galaxiesToIncrement_x)
            {
                item.position.x++;
            }
            foreach (var item in galaxiesToIncrement_y)
            {
                item.position.y++;
            }

            // Correct the depth values
            maxGalaxy_x = (int)galaxyDict.Values.Select(g => g.position.x).Max() + 1;
            maxGalaxy_y = (int)galaxyDict.Values.Select(g => g.position.y).Max() + 1;
        }


        public int GetTotalDistances()
        {
            int totalDistances = 0;

            Galaxy[] galaxies = galaxyDict.Values.ToArray();
            for (int i = 0; i < galaxies.Length; i++)
            {
                //int shortestDistance = -1;

                Galaxy first = galaxies[i];
                Galaxy? currentShortestGalaxy = null;
                for (int j = i + 1; j < galaxies.Length; j++)
                {
                    Galaxy second = galaxies[j];

                    int dist = (int)(Math.Abs(first.position.y - second.position.y) + Math.Abs(first.position.x - second.position.x));
                    totalDistances += dist;
                    /*if (dist < shortestDistance || shortestDistance < 0)
                    {
                        currentShortestGalaxy = second;
                        shortestDistance = dist;
                    }
                    */
                }
                //if (currentShortestGalaxy != null) Console.WriteLine("Shortest distance found between " + (char)(first.id + '1') + " and " + (char)(currentShortestGalaxy.id + '1') + "    (length: " + shortestDistance + ")");
                //totalDistances += shortestDistance;
            }
            return totalDistances;
        }

        public string GetWritableString()
        {
            string s = "";

            int maxX, maxY;
            Vector2[] galaxyPositions = galaxyDict.Values.Select(g => g.position).ToArray();
            maxX = (int)galaxyPositions.Select(v => v.x).Max();
            maxY = (int)galaxyPositions.Select(v => v.y).Max();

            for (int y = 0; y < maxY + 1; y++)
            {
                for (int x = 0; x < maxX + 1; x++)
                {
                    if (galaxyPositions.Contains(new Vector2(x, y)))
                        s += '#';
                    else
                        s += '.';
                }
                s += "\n";
            }
            return s;
        }

        public void WriteFormatted()
        {
            (int, int) baseCursorPos = Console.GetCursorPosition();

            Vector2[] galaxyPositions = galaxyDict.Values.Select(g => g.position).ToArray();

            string background = "";
            for (int i = 0; i < maxGalaxy_y; i++)
            {
                background += new string('.', maxGalaxy_x) + "\n";
            }
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write(background);

            Console.ForegroundColor = ConsoleColor.Magenta;
            char counter = '1';
            foreach (Vector2 gv in galaxyPositions)
            {
                Console.SetCursorPosition((int)(baseCursorPos.Item1 + gv.x), (int)(baseCursorPos.Item2 + gv.y));
                Console.Write(counter);
                counter++;
            }

            Console.SetCursorPosition(0, baseCursorPos.Item2 + maxGalaxy_y + 1);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
        }

        public class Galaxy
        {
            public int id;
            public Vector2 position;

            public Galaxy(int id, Vector2 position)
            {
                this.id = id;
                this.position = position;
            }

            public override bool Equals(object? obj)
            {
                if (obj is not Galaxy other) return false;
                return position == other.position && id == other.id;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(id, position);
            }

            public override string? ToString()
            {
                return $"ID:{id} POS:{position}";
            }
        }
    }
}
