using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day3
{
    public class Schematic
    {
        public char[,] Symbols { get; private set; }
        public int[] Dimension { get; private set; }

        public Schematic(string[] readAllLinesOutput)
        {
            Dimension = new int[2] { readAllLinesOutput[0].Length, readAllLinesOutput.Length };
            char[,] sym = new char[Dimension[0], Dimension[1]];

            for (int x = 0; x < readAllLinesOutput[0].Length; x++) // x-value from top left
            {
                for (int y = 0; y < readAllLinesOutput.Length; y++) // y value from top left
                {
                    sym[x, y] = readAllLinesOutput[y][x];
                }
            }

            Symbols = sym;
        }

        public List<Map> FindAllMaps()
        {
            List<Map> returnMap = new List<Map>();

            // Loop over each ROW to find numbers
            for (int y = 0; y < Dimension[1]; y++)
            {
                string row = GetRow(y);

                for (int x = 0; x < Dimension[0]; x++)
                {
                    if (char.IsDigit(row[x]))
                    {
                        int stepsToJump = 0;

                        // if current char is a digit
                        while (char.IsDigit(row[x+stepsToJump]))
                        {
                            stepsToJump++;
                            if (x + stepsToJump >= Dimension[0]) break;
                        }

                        int val = int.Parse(row[x..(x + stepsToJump)]);
                        Map map = new Map(
                            rowNr: y,
                            x_coordinate: x,
                            valueCharacterLength: stepsToJump,
                            value: val,
                            Symbols,
                            Dimension);
                        returnMap.Add(map);

                        // Skip a few iterations of the loop by stepping past the digits
                        x += stepsToJump;
                    }
                }
            }
            return returnMap;
        }

        public string GetColumn(int column)
        {
            return new string(Enumerable.Range(0, Dimension[0])
                .Select(y => Symbols[column, y])
                .ToArray());
        }
        public string GetRow(int row)
        {
            return new string(Enumerable.Range(0, Dimension[0]) // not really sure how this works
                .Select(x => Symbols[x, row]) // select all x values from a specific row
                .ToArray());
        }

        public class Map
        {
            public char[,] symbols;
            //public char[,] Symbols { get; private set; }
            public int RowNr { get; private set; }
            public int X_coordinate { get; private set; }
            public int ValueCharacterLength { get; private set; }
            public int Value { get; private set; }

            public Map(int rowNr, int x_coordinate, int valueCharacterLength, int value, char[,] sym, int[] dim)
            {
                this.RowNr = rowNr;
                this.X_coordinate = x_coordinate;
                this.ValueCharacterLength = valueCharacterLength;
                this.Value = value;

                symbols = CreateSymbolMap(sym, dim);
            }

            public bool HasSymbolAdjacent()
            {
                string allCharsAdjacent = "";

                for (int y = 0; y < symbols.GetLength(1); y++)
                {
                    for (int x = 0; x < symbols.GetLength(0); x++)
                    {
                        allCharsAdjacent += symbols[x, y];
                    }
                }

                allCharsAdjacent = allCharsAdjacent.Trim(".0123456789".ToCharArray());

                if (allCharsAdjacent.Length > 0) return true;
                else return false;
            }

            public string GetPrintableString(char[,] sym, int[] dim)
            {
                string returnString = "";

                // We essentially grab the number and all symbols around it,
                // but clamp the range so we dont go outside the intended map (or the whole schematic)
                int xLim_low = Math.Clamp(X_coordinate - 1, 0, dim[0]); 
                int xLim_high = Math.Clamp(X_coordinate + ValueCharacterLength + 1, 0, dim[0]);
                int yLim_low = Math.Clamp(RowNr - 1, 0, dim[1]);
                int yLim_high = Math.Clamp(RowNr + 2, 0, dim[1]);

                for (int y = yLim_low; y < yLim_high; y++)
                {
                    for (int x = xLim_low; x < xLim_high; x++)
                    {
                        returnString += sym[x, y];
                    }
                    returnString += "\n";
                }
                return returnString;
            }

            /// <summary>
            /// Creates the 2D array for all symbols around a the number
            /// </summary>
            /// <param name="sym">The whole schematic</param>
            /// <param name="dim">dimensions of the schematic</param>
            /// <returns></returns>
            private char[,] CreateSymbolMap(char[,] sym, int[] dim)
            {
                // We essentially grab the number and all symbols around it,
                // but clamp the range so we dont go outside the intended map (or the whole schematic)
                int xLim_low = Math.Clamp(X_coordinate - 1, 0, dim[0]);
                int xLim_high = Math.Clamp(X_coordinate + ValueCharacterLength+1, 0, dim[0]);
                int yLim_low = Math.Clamp(RowNr - 1, 0, dim[1]);
                int yLim_high = Math.Clamp(RowNr + 2, 0, dim[1]);

                char[,] returnMatrix = new char[xLim_high - xLim_low, yLim_high - yLim_low];
                int symbols_x = 0; // indexes for the local field "symbols"
                int symbols_y = 0;

                // Row loop (y)
                for (int y = yLim_low; y < yLim_high; y++)
                {
                    symbols_x = 0;
                    // x coordinate loop (x)
                    for (int x = xLim_low; x < xLim_high; x++)
                    {
                        returnMatrix[symbols_x, symbols_y] = sym[x, y];
                        symbols_x++;
                    }
                    symbols_y++;
                }

                // loop through map and write it. Put breakpoint on this line to step through all maps.
                /*for (int x = 0; x < returnMatrix.GetLength(0); x++) // x-value from top left
                {
                    for (int y = 0; y < returnMatrix.GetLength(1); y++) // y value from top left
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(returnMatrix[x, y]);
                    }
                }*/

                return returnMatrix;
            }

            public static int[] FindGearRatios(List<Map> mapList)
            {
                List<int> gearRatios = new List<int>();

                List<Map> gearContenders = new List<Map>();
                List<int> gear_x = new List<int>();
                List<int> gear_y = new List<int>();

                // Loop through all maps
                foreach (Map map in mapList)
                {
                    // Loop through x-coordinate
                    for (int i = 0; i < map.symbols.GetLength(0); i++)
                    {
                        // Loop through y-coordinate
                        for (int j = 0; j < map.symbols.GetLength(1); j++)
                        {
                            // If it contains the symbol '*', it is a "gear contender".
                            // Still needs to be adjacent to another gear contender. So we check that below:
                            if (map.symbols[i, j] == '*')
                            {
                                gearContenders.Add(map);

                                // We will check if the first row and/or column contains a number.
                                // If it does, it means we need to shift the coordinate reference down and/or right,
                                // because we assume most numbers start in the middle of the map, not the beginning
                                //
                                //  For example: .644. needs to be shifted downward one step (y++)
                                //               *....
                                //
                                //  and this      53.  needs to be shifted down and right (x++, y++)
                                //                .*.
                                //
                                int global_x = map.X_coordinate; // start at global x-value of the first number
                                int global_y = map.RowNr;        // start at global y-value of the first number
                                global_x += -1; // we move 1 step to the left of the first number offset for our local coordinate
                                global_y += -1; // we move 1 step above the first number offset for our local coordinate
                                global_x += i;  // add our local coordinate (coordinate of the loop [i,j] where we found a '*'
                                global_y += j;  // add our local coordinate (coordinate of the loop [i,j] where we found a '*'
                                for (int k = 0; k < map.symbols.GetLength(1); k++) // first column
                                {
                                    // Counterintuitively, the value of x is actually the column number.
                                    // In map.symbols[0, k] we loop through the first col
                                    if (char.IsDigit(map.symbols[0, k]))
                                    {
                                        global_x++; // adjust coordinate system right one step if we an edge case scenario
                                        break;
                                    }
                                }
                                for (int k = 0; k < map.symbols.GetLength(0); k++) // first row
                                {
                                    // Counterintuitively, the value of y is actually the row number.
                                    // In map.symbols[k, 0] we loop through the first row
                                    if (char.IsDigit(map.symbols[k, 0]))
                                    {
                                        global_y++; // adjust coordinate system down one step if we an edge case scenario
                                        break;
                                    }
                                }

                                gear_x.Add(global_x); gear_y.Add(global_y);
                                //Console.WriteLine("{0} [{1},{2}]", map.Value, global_x, global_y);
                            }
                        }
                    }
                }

                // Compare all gear contenders with eachother, to see if they share '*'
                for (int i = 0; i < gearContenders.Count; i++)
                {

                    // gearContenders[i] is the current one
                    // gearContenders[j] the one we're comparing to comparing.
                    // We set j = i because we dont want to compare to what came before.
                    // This makes it such that we don't compare two numbers twice (for example map1 == map2,  map2 == map1).
                    // we also add +1 cause we don't want to compare with itself (cause they will always return true: map1 == map1)
                    for (int j = i+1; j < gearContenders.Count; j++)
                    {
                        // If their '*' respective coordinates match
                        if (gear_x[i] == gear_x[j] && gear_y[i] == gear_y[j])
                        {
                            gearRatios.Add(gearContenders[i].Value * gearContenders[j].Value);
                            //Console.WriteLine("Found ratio between {0} and {1}: {2}", gearContenders[i].Value, gearContenders[j].Value, gearContenders[i].Value * gearContenders[j].Value);
                        }
                    }
                }
                return gearRatios.ToArray();
            }
        }
    }
}
