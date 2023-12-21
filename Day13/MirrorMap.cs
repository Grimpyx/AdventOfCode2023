using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Day13.MirrorMap;

namespace Day13
{

    public class MirrorMap
    {
        public readonly Terrain[,] mirrors; // true == rocks (#), false = ash (.)
        public readonly Terrain[][] rows;
        public readonly Terrain[][] columns;

        public enum Terrain
        {
            Ash = 0,
            Rocks = 1
        }


        public MirrorMap(Terrain[,] mirrors)
        {
            this.mirrors = mirrors;
        }
        public MirrorMap(string[] rowInput)
        {
            mirrors = new Terrain[rowInput[0].Length, rowInput.Length];
            rows = new Terrain[mirrors.GetLength(1)][];    // getlength(1) is the length of all nr of x values
            columns = new Terrain[mirrors.GetLength(0)][]; // getlength(0) is the length of all nr of y values

            // Populate the x,y map
            for (int i = 0; i < mirrors.GetLength(1); i++)
            {
                for (int j = 0; j < mirrors.GetLength(0); j++)
                {
                    mirrors[j, i] = (rowInput[i][j] == '#') ? Terrain.Rocks : Terrain.Ash;
                }
            }

            // All rows in order
            for (int i = 0; i < mirrors.GetLength(1); i++)
            {
                Terrain[] tarray = new Terrain[mirrors.GetLength(0)];
                for (int j = 0; j < mirrors.GetLength(0); j++)
                {
                    tarray[j] = mirrors[j, i];
                }
                rows[i] = tarray;
            }

            // All columns in order
            for (int i = 0; i < mirrors.GetLength(0); i++)
            {
                Terrain[] tarray = new Terrain[mirrors.GetLength(1)];
                for (int j = 0; j < mirrors.GetLength(1); j++)
                {
                    tarray[j] = mirrors[i, j];
                }
                columns[i] = tarray;
            }


        }

        // Returns the value defined by the task (part 1):
        // 100 times rows to above the mirrored axis
        // OR the number of columns to the left of the mirrored axis
        public long Solve()
        {
            long value = 0;

            // If mirrored along y (rows)
            long rowValue = 0;
            for (int i = 0; i < rows.Length - 1; i++) // Compares pairs of two
            {
                if (rows[i].SequenceEqual(rows[i+1])) // If pairs are equal
                {
                    // This clamps the end of the loop such that we don't accidentally step out of range
                    int length = Math.Min(i + 1, rows.Length - (i + 1));

                    bool wasBroken = false;
                    // If a pair of two is found, compare the columns to the above and below the pair
                    for (int j = 1; j < length; j++)
                    {
                        // i   -1 is the element to the above the first of the pair.                        Start   ******||****
                        // i+1 +1 is the element to the below the second of the pair.                       j=1     *****|**|***
                        // replacing -1/+1 with j leads means we keep comparing until we reach the end      j=2     ****|****|**
                        if (!rows[i - j].SequenceEqual(rows[i + 1 + j]))
                        {
                            wasBroken = true;
                            break;
                        }
                    }

                    // If broken, it means it wasn't a perfect mirror.
                    if (wasBroken) continue;
                    else
                    {
                        rowValue = 100 * (i + 1);
                        break;
                    }

                }
            }
            value += rowValue;

            // If mirrored along y (rows)
            long colValue = 0;
            for (int i = 0; i < columns.Length - 1; i++) // Compares pairs of two
            {
                if (columns[i].SequenceEqual(columns[i + 1])) // If pairs are equal
                {
                    // This clamps the end of the loop such that we don't accidentally step out of range
                    int length = Math.Min(i + 1, columns.Length - (i + 1));
                    
                    bool wasBroken = false;
                    // If a pair of two is found, compare the columns to the right and left of the pair
                    for (int j = 1; j < length; j++)
                    {
                        // i   -1 is the element to the left of the first in the pair.                      Start   ******||****
                        // i+1 +1 is the element to the right of the second in the pair.                    j=1     *****|**|***
                        // replacing -1/+1 with j leads means we keep comparing until we reach the end      j=2     ****|****|**
                        if (!columns[i - j].SequenceEqual(columns[i + 1 + j]))
                        {
                            wasBroken = true;
                            break;
                        }
                    }

                    // If broken, it means it wasn't a perfect mirror.
                    if (wasBroken) continue;
                    else
                    {
                        colValue = (i + 1);
                        break;
                    }
                }
            }
            value += colValue;

            return value;
        }

        public Terrain[] GetRow(int row)
        {
            return [.. Enumerable.Range(0, mirrors.GetLength(0))
                .Select(x => mirrors[x, row])];

            // Explanation:
            // Enumerable.Range(x,y) generate a list of integers from x to y. (x, x+1, x+2, ..., y-2, y-1)
            // in .Select() we use each integer from the generated list to select all mirrors[LISTVALUE, row]
            // We dont need .ToArray() because we already have it in a collection expression (ie [.. collectionOfValues])
        }
        public Terrain[] GetColumn(int column)
        {
            return [.. Enumerable.Range(0, mirrors.GetLength(1))
                .Select(y => mirrors[column, y])];

            // Explanation:
            // Enumerable.Range(x,y) generate a list of integers from x to y. (x, x+1, x+2, ..., y-2, y-1)
            // in .Select() we use each integer from the generated list to select all mirrors[LISTVALUE, row]
            // We dont need .ToArray() because we already have it in a collection expression (ie [.. collectionOfValues])
        }

        public override string? ToString()
        {
            // Without numbering
            string s = "";
            for (int i = 0; i < mirrors.GetLength(1); i++)
            {
                for (int j = 0; j < mirrors.GetLength(0); j++)
                {
                    s += mirrors[j, i] == Terrain.Rocks ? '#' : '.';
                }
                s += "\n";
            }
            return s;

            /*
            // With numbering. Falls apart when numbering is more than one character long,
            // for example if you have 10 or more columns
            string s = "  ";
            for (int i = 0; i < mirrors.GetLength(0); i++)
            {
                s += i;
            }

            for (int i = 0; i < mirrors.GetLength(1); i++)
            {
                s += "\n" + i + " ";
                for (int j = 0; j < mirrors.GetLength(0); j++)
                {
                    s += mirrors[j, i] == Terrain.Rocks ? '#' : '.';
                }
            }
            return s;*/
        }
    }
}
