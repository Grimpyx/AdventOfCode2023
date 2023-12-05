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

                        while (char.IsDigit(row[x+stepsToJump]))//(char.IsDigit(row[Math.Clamp(x + stepsToJump, 0, Dimension[0]-1)]))
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

                        // Create submap of symbols
                        
                        //if (map.HasSymbolAdjacent()) Console.WriteLine(map.GetPrintableString(Symbols, Dimension) + "    > Val: " + map.Value + "    steps: " + map.ValueCharacterLength);
                        //Console.WriteLine("Has adjacent. >>>>>>>>>>>>>>>>\n");
                        //else Console.WriteLine("\n");

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

                //Console.WriteLine("Chars: " + allCharsAdjacent);
                allCharsAdjacent = allCharsAdjacent.Trim(".0123456789".ToCharArray());
                //Console.WriteLine("Chars: " + allCharsAdjacent);

                if (allCharsAdjacent.Length > 0) return true;
                else return false;
            }

            public string GetPrintableString(char[,] sym, int[] dim)
            {
                string returnString = "";

                // We essentially grab all symbols around the number,
                // but clamp the range so we dont go outside the schematic
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

            private char[,] CreateSymbolMap(char[,] sym, int[] dim)
            {
                // We essentially grab all symbols around the number,
                // but clamp the range so we dont go outside the schematic
                int xLim_low = Math.Clamp(X_coordinate - 1, 0, dim[0]);
                int xLim_high = Math.Clamp(X_coordinate + ValueCharacterLength+1, 0, dim[0]);
                int yLim_low = Math.Clamp(RowNr - 1, 0, dim[1]);
                int yLim_high = Math.Clamp(RowNr + 2, 0, dim[1]);

                char[,] returnMatrix = new char[xLim_high - xLim_low, yLim_high - yLim_low];
                int symbols_x = 0; // indexes for the local field "symbols"
                int symbols_y = 0;

                // Row loop
                for (int y = yLim_low; y < yLim_high; y++)
                {
                    symbols_x = 0;
                    // x coordinate loop
                    for (int x = xLim_low; x < xLim_high; x++)
                    {
                        returnMatrix[symbols_x, symbols_y] = sym[x, y];
                        symbols_x++;
                    }
                    symbols_y++;
                }
                return returnMatrix;
            }

            public static int[] FindGearRatios(List<Map> mapList)
            {
                List<int> gearRatios = new List<int>();

                List<Map> gearContenders = new List<Map>();
                List<int> gear_x = new List<int>();
                List<int> gear_y = new List<int>();
                foreach (Map map in mapList)
                {
                    string allChars = "";
                    for (int i = 0; i < map.symbols.GetLength(0); i++)
                    {
                        for (int j = 0; j < map.symbols.GetLength(1); j++)
                        {
                            allChars += map.symbols[i, j];
                            if (map.symbols[i, j] == '*')
                            {
                                gearContenders.Add(map);

                                // check first row and first column for numbers. If numbers is found, +1
                                int global_x = map.X_coordinate; // start at global x-value of the first number
                                int global_y = map.RowNr;        // start at global y-value of the first number
                                global_x += -1; // we move 1 step to the left of the first number offset for our local coordinate
                                global_y += -1; // we move 1 step above the first number offset for our local coordinate
                                global_x += i;
                                global_y += j;
                                for (int k = 0; k < map.symbols.GetLength(1); k++)
                                {
                                    if (char.IsDigit(map.symbols[0, k]))
                                    {
                                        Console.WriteLine(map.Value + " X:" + map.symbols[0, k]);
                                        global_x++; // adjust coordinate system right one step if we an edge case scenario
                                        break;
                                    }
                                }
                                for (int k = 0; k < map.symbols.GetLength(0); k++)
                                {
                                    if (char.IsDigit(map.symbols[k, 0]))
                                    {
                                        Console.WriteLine(map.Value + " Y:" + map.symbols[k, 0]);
                                        global_y++; // adjust coordinate system down one step if we an edge case scenario
                                        break;
                                    }
                                }

                                gear_x.Add(global_x); gear_y.Add(global_y);
                                Console.WriteLine("{0} [{1},{2}]", map.Value, global_x, global_y);
                                //Console.WriteLine(" - " + map.Value + " might be part of a gear at [{0},{1}]", global_x, global_y);
                            }
                        }
                    }
                }

                for (int i = 0; i < gearContenders.Count; i++)      // gearContenders[i] is the current one
                {
                    for (int j = i+1; j < gearContenders.Count; j++)  // gearContenders[j] comparing   // we set j = i because we dont want to compare to what came before
                    {
                        if (gear_x[i] == gear_x[j] && gear_y[i] == gear_y[j])
                        {
                            gearRatios.Add(gearContenders[i].Value * gearContenders[j].Value);
                            Console.WriteLine("Found ratio between {0} and {1}: {2}", gearContenders[i].Value, gearContenders[j].Value, gearContenders[i].Value * gearContenders[j].Value);
                        }
                    }
                }
                return gearRatios.ToArray();
            }
        }
    }
}
