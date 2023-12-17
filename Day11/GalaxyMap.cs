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

            maxGalaxy_x = (int)galaxyDict.Values.Select(g => g.position.x).Max() + (int)galaxyDict.Values.Select(g => g.expansions.x).Max() + 1;
            maxGalaxy_y = (int)galaxyDict.Values.Select(g => g.position.y).Max() + (int)galaxyDict.Values.Select(g => g.expansions.y).Max() + 1;
        }

        public void Expand()
        {
            Galaxy[] galaxies = [.. galaxyDict.Values];
            Vector2[] galaxyPositions = galaxyDict.Values.Select(g => g.position).ToArray();
            float[] allX = galaxyPositions.Select(x => x.x).ToArray();
            float[] allY = galaxyPositions.Select(x => x.y).ToArray();

            maxGalaxy_x = (int)galaxyDict.Values.Select(g => g.position.x).Max() + (int)galaxyDict.Values.Select(g => g.expansions.x).Max() + 1;
            maxGalaxy_y = (int)galaxyDict.Values.Select(g => g.position.y).Max() + (int)galaxyDict.Values.Select(g => g.expansions.y).Max() + 1;


            for (int y = 0; y < maxGalaxy_y; y++)
            {
                if (!allY.Contains(y))
                {
                    foreach (Galaxy g in galaxies)
                    {
                        if (g.position.y >= y) galaxyDict[g.id].expansions.y++; 
                    }
                }
            }
            for (int x = 0; x < maxGalaxy_x; x++)
            {
                if (!allX.Contains(x))
                {
                    foreach (Galaxy g in galaxies)
                    {
                        if (g.position.x >= x) galaxyDict[g.id].expansions.x++;
                    }
                }
            }
        }


        public long GetTotalDistances()
        {
            long totalDistances = 0;

            Galaxy[] galaxies = galaxyDict.Values.ToArray();
            for (int i = 0; i < galaxies.Length; i++)
            {
                Galaxy first = galaxies[i];
                for (int j = i + 1; j < galaxies.Length; j++)
                {
                    Galaxy second = galaxies[j];

                    long dist = (long)(Math.Abs(first.GetScaledPosition.y - second.GetScaledPosition.y) + Math.Abs(first.GetScaledPosition.x - second.GetScaledPosition.x));
                    totalDistances += dist;
                }
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

            Galaxy[] galaxies = galaxyDict.Values.ToArray();

            int maxx = (int)galaxyDict.Values.Select(g => g.position.x).Max() + (int)galaxyDict.Values.Select(g => g.expansions.x).Max() + 1;
            int maxy = (int)galaxyDict.Values.Select(g => g.position.y).Max() + (int)galaxyDict.Values.Select(g => g.expansions.y).Max() + 1;
            
            string background = "";
            for (int i = 0; i < maxy; i++)
            {
                background += new string('.', maxx) + "\n";
            }
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write(background);

            Console.ForegroundColor = ConsoleColor.Magenta;
            char counter = '1';
            for (int i = 0; i < galaxies.Length; i++)
            {
                Console.SetCursorPosition(
                    (int)(baseCursorPos.Item1 + galaxies[i].expansions.x + galaxies[i].position.x),
                    (int)(baseCursorPos.Item2 + galaxies[i].expansions.y + galaxies[i].position.y));
                Console.Write(counter);
                counter++;
            }


            Console.SetCursorPosition(0, baseCursorPos.Item2 + maxy + 1);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
        }

        public class Galaxy
        {
            public int id;
            public Vector2 position;
            public Vector2 expansions;
            private static int ExpansionLength = 1000000;

            public Galaxy(int id, Vector2 position)
            {
                this.id = id;
                this.position = position;
            }

            // When one expansion should represent 1 million steps. All the expansions need to build on one another.
            // This means that if we would have four expansions, the last expansion would have a distance around (10^6)^4.
            // Instead we scale it here for when we do calculations, and just use the variable "expansions" to draw correctly.
            public Vector2 GetScaledPosition => Math.Max(ExpansionLength-1, 1) * expansions + position; // Doing only ExpansionLength is a bit off. but "Math.Max(ExpansionLength-1, 1)" works




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
