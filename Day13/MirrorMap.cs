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
            Rocks = 1,
            Ambiguous = 2
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
        public long SolveP1()
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


        private (int[] rowOrColumnIndex, bool[] isRow) FindMirrorLine()
        {
            int[] skipIndex = [];
            bool[] skipIsRow = [];
            return FindMirrorLine(skipIndex, skipIsRow);
        }
        private (int[] rowOrColumnIndex, bool[] isRow) FindMirrorLine(int[] skipIndex, bool[] skipIsRow)
        {

            List<bool> isRow = new List<bool>();
            List<int> rowOrColumnIndex = new List<int>();

            // instead of having two for loops, combining rows and columns helps make the code more compact
            Terrain[][][] rowOrColumn = [[..rows], [..columns]];
            for (int rowOrIndexLoop = 0; rowOrIndexLoop < rowOrColumn.Length; rowOrIndexLoop++)
            {
                string rowOrColumnAsString = (rowOrIndexLoop == 0 ? "row" : "column");

                for (int i = 0; i < rowOrColumn[rowOrIndexLoop].Length - 1; i++) // Compares pairs of two
                {
                    // If the current row or column is another
                    bool skip = false;
                    for (int si = 0; si < isRow.Count; si++)
                    {
                        if (rowOrColumnIndex[si] == i && rowOrIndexLoop == (isRow[si] ? 0 : 1))
                            skip = true;
                    }
                    for (int si = 0; si < skipIsRow.Length; si++)
                    {
                        if (skipIndex[si] == i && rowOrIndexLoop == (skipIsRow[si] ? 0 : 1))
                            skip = true;
                    }
                    if (skip)
                    {
                        Console.WriteLine("Skipped " + rowOrColumnAsString + " " + i);
                        continue;
                    }


                    var l11 = rowOrColumn[rowOrIndexLoop][i];
                    var l12 = rowOrColumn[rowOrIndexLoop][i + 1];
                    if (NumberOfCorrectPositions(l11, l12) == l11.Length)
                    {
                        //////Console.WriteLine("Found contender at " + rowOrColumnAsString + " " + i);

                        // This clamps the end of the loop such that we don't accidentally step out of range
                        int length = Math.Min(i + 1, rowOrColumn[rowOrIndexLoop].Length - (i + 1));

                        bool wasBroken = false;
                        // If a pair of two is found, compare the columns to the above and below the pair
                        for (int j = 0; j < length; j++)
                        {
                            // i   -1 is the element to the before the first of the pair.                        Start   ******||****
                            // i+1 +1 is the element to the after the second of the pair.                        j=1     *****|**|***
                            // replacing -1/+1 with j means we keep comparing until we reach the end             j=2     ****|****|**
                            //if (!rowOrColumn[rowOrIndexLoop][i - j].SequenceEqual(rowOrColumn[rowOrIndexLoop][i + 1 + j]))
                            var l21 = rowOrColumn[rowOrIndexLoop][i - j];
                            var l22 = rowOrColumn[rowOrIndexLoop][i + 1 + j];
                            //////Console.WriteLine("  " + rowOrColumnAsString + " " + (i - j) + " and " + (i + 1 + j));
                            if (NumberOfCorrectPositions(l21, l22) != l21.Length)
                            {
                                wasBroken = true;
                                //////Console.WriteLine("  Broke! " + NumberOfCorrectPositions(l21, l22) + " out of required " + l21.Length);
                                break;
                            }
                        }

                        if (!wasBroken)
                        {
                            rowOrColumnIndex.Add(i);
                            if (rowOrIndexLoop == 0) isRow.Add(true);
                            else isRow.Add(false);
                            //////Console.WriteLine("  ! Found at " + (rowOrIndexLoop == 0 ? "row" : "column") + " i=" + i);
                            continue;
                        }

                        continue;
                    }
                }
            }
            //////Console.WriteLine();
            return (rowOrColumnIndex.ToArray(), isRow.ToArray());
        }

        private static int NumberOfCorrectPositions(Terrain[] array1, Terrain[] array2, bool ignoreAmbiguous = false)
        {
            if (array1.Length != array2.Length) return -1;
            if (ignoreAmbiguous)
                return Enumerable.Range(0, array1.Length).Where(i => array1[i] == array2[i] ).Count();
            else 
                return Enumerable.Range(0, array1.Length).Where(i => array1[i] == array2[i] || array1[i] == Terrain.Ambiguous || array2[i] == Terrain.Ambiguous).Count();
        }

        public void CorrectSmudge(int[] skipIndex, bool[] skipIsRow) // (int rowOrColumnIndex, bool isRow)
        {
            // Avoid the correct mirror line, but run very similar code to FindMirrorLine
            Terrain[][][] rowOrColumn = { [.. rows], [.. columns] };
            for (int rowOrIndexLoop = 0; rowOrIndexLoop < rowOrColumn.Length; rowOrIndexLoop++)
            {
                string rowOrColumnAsString = (rowOrIndexLoop == 0 ? "row" : "column");

                for (int i = 0; i < rowOrColumn[rowOrIndexLoop].Length - 1; i++) // Compares pairs of two
                {
                    // If the current row or column is the mirror line
                    // If the current row or column is another
                    bool skip = false;
                    for (int si = 0; si < skipIsRow.Length; si++)
                    {
                        if (skipIndex[si] == i && rowOrIndexLoop == (skipIsRow[si] ? 0 : 1))
                        {
                            skip = true;
                            Console.WriteLine("Skipped " + rowOrColumnAsString + " " + i);
                        }
                    }
                    if (skip) continue;



                    Terrain[] l11 = rowOrColumn[rowOrIndexLoop][i];
                    Terrain[] l12 = rowOrColumn[rowOrIndexLoop][i + 1];
                    int nrOfCorrectPositionsPair = NumberOfCorrectPositions(l11, l12);

                    int casee = -1;
                    if (nrOfCorrectPositionsPair == l11.Length)     // If we found a mirror line we want to check for smudges
                        casee = 0;
                    if (nrOfCorrectPositionsPair == l11.Length - 1) // if the start might have a smudge
                        casee = 1;

                    Console.WriteLine("Checking " + rowOrColumnAsString + " " + i + " and " + (i + 1) + "    common: " + nrOfCorrectPositionsPair + " out of " + l11.Length);
                    if (casee == 0 || casee == 1) // If pairs are equal, or one step away
                    {
                        Console.WriteLine("         " + rowOrColumnAsString + " " + i + " and " + (i + 1) + " length " + nrOfCorrectPositionsPair);


                        // This clamps the end of the loop such that we don't accidentally step out of range
                        int length = Math.Min(i + 1, rowOrColumn[rowOrIndexLoop].Length - (i + 1));

                        int jIndexSaved = 0;
                        bool foundSmudge = false;
                        bool wasBroken = false;

                        if (casee == 0)
                        {
                            int jContender = -1;
                            for (int j = 1; j < length; j++)
                            {
                                Console.WriteLine("          J comparison: " + rowOrColumnAsString + " " + (i - j) + " and " + (i + 1 + j));

                                // i   -1 is the element to the before the first of the pair.                        Start   ******||****
                                // i+1 +1 is the element to the after the second of the pair.                        j=1     *****|**|***
                                // replacing -1/+1 with j means we keep comparing until we reach the end             j=2     ****|****|**
                                Terrain[] l21 = rowOrColumn[rowOrIndexLoop][i - j];
                                Terrain[] l22 = rowOrColumn[rowOrIndexLoop][i + 1 + j];
                                int nrOfCorrectPositionsOutside = NumberOfCorrectPositions(l21, l22);

                                //if (nrOfCorrectPositionsOutside == l21.Length)
                                //    numberOfExactSets++;

                                if (nrOfCorrectPositionsOutside <= l21.Length - 1) // if we hit where the smudge is at for case 0
                                {
                                    if (jContender == -1 && nrOfCorrectPositionsOutside == l21.Length - 1)
                                    {
                                        Console.WriteLine("          Found contender. (" + rowOrColumnAsString + " " + (i - j) + " and " + (i + 1 + j) + ")");
                                        jContender = j;
                                    }
                                    else // if we already have a contender and we find another one, means it's not a mirror
                                    {
                                        Console.WriteLine("          Found contender. Already had one! Breaking loop. (" + rowOrColumnAsString + " " + (i - j) + " and " + (i + 1 + j) + ")");
                                        wasBroken = true;
                                        break;
                                    }
                                }
                                /*else if (nrOfCorrectPositionsOutside <= l21.Length - 1 && jContender != -1)
                                {
                                    Console.WriteLine("          Found contender. Already had one! Breaking loop. (" + rowOrColumnAsString + " " + (i - j) + " and " + (i + 1 + j) + ")");
                                    wasBroken = true;
                                    break;
                                }*/
                            }
                            if (!wasBroken) // then is valid
                            {
                                jIndexSaved = jContender;
                                foundSmudge = true;
                            }
                        }
                        else if (casee == 1)
                        {
                            for (int j = 1; j < length; j++)
                            {
                                // i   -1 is the element to the before the first of the pair.                        Start   ******||****
                                // i+1 +1 is the element to the after the second of the pair.                        j=1     *****|**|***
                                // replacing -1/+1 with j means we keep comparing until we reach the end             j=2     ****|****|**
                                Terrain[] l21 = rowOrColumn[rowOrIndexLoop][i - j];
                                Terrain[] l22 = rowOrColumn[rowOrIndexLoop][i + 1 + j];
                                int nrOfCorrectPositionsOutside = NumberOfCorrectPositions(l21, l22);

                                if (nrOfCorrectPositionsOutside != l21.Length)
                                {
                                    wasBroken = true;
                                    break;
                                }
                            }
                            if (!wasBroken) // then is valid
                            {
                                jIndexSaved = 0;
                                foundSmudge = true;
                            }
                        }

                        //if (numberOfExactSets == length - 1)
                        if (foundSmudge)
                        {
                            Console.WriteLine("Found smudge at " + rowOrColumnAsString + " " + i);

                            // Change the mirror map
                            int rocIndex1 = i - jIndexSaved;
                            int rocIndex2 = i + 1 + jIndexSaved;
                            Terrain[] roc1 = rowOrColumn[rowOrIndexLoop][rocIndex1]; // First  set of the mirror line
                            Terrain[] roc2 = rowOrColumn[rowOrIndexLoop][rocIndex2]; // Second set of the mirror line
                            for (int setIndex = 0; setIndex < roc1.Length; setIndex++)
                            {
                                if (roc1[setIndex] != roc2[setIndex])
                                {
                                    if (rowOrIndexLoop == 0) // if rows
                                    {
                                        //mirrors[setIndex, rocIndex1] = Terrain.Ambiguous; //(mirrors[setIndex, rocIndex1] == Terrain.Ash) ? Terrain.Rocks : Terrain.Ash; // we swap. If ash then become rocks, if rocks become ash
                                        //mirrors[setIndex, rocIndex2] = Terrain.Ambiguous;
                                        Terrain setItTo = (mirrors[setIndex, rocIndex1] == Terrain.Ash) ? Terrain.Rocks : Terrain.Ash;
                                        mirrors[setIndex, rocIndex1] = setItTo;
                                        rows[rocIndex1][setIndex] = setItTo;
                                        columns[setIndex][rocIndex1] = setItTo;

                                        return;

                                        //Console.WriteLine("(row) Edited smudge at " + setIndex + ", " + rocIndex1 + ")");
                                    }
                                    if (rowOrIndexLoop == 1) // if columns
                                    {
                                        //mirrors[rocIndex1, setIndex] = Terrain.Ambiguous; //(mirrors[rocIndex1, setIndex] == Terrain.Ash) ? Terrain.Rocks : Terrain.Ash; // we swap. If ash then become rocks, if rocks become ash
                                        //mirrors[rocIndex2, setIndex] = Terrain.Ambiguous;
                                        Terrain setItTo = (mirrors[rocIndex1, setIndex] == Terrain.Ash) ? Terrain.Rocks : Terrain.Ash;
                                        mirrors[rocIndex1, setIndex] = setItTo;
                                        rows[setIndex][rocIndex1] = setItTo;
                                        columns[rocIndex1][setIndex] = setItTo;

                                        return;

                                        //Console.WriteLine("(col) Edited smudge at " + rocIndex1 + ", " + setIndex + ")");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine(" >>>> ERROR! FOUND NO SMUDGE <<<< - - - - - - - - - - - -");
            //return (rowOrColumnIndex, isRow);
        }

        public long SolveP2()
        {
            // Firstly we want to correct the smudge
            //(int rowOrColumnIndex, bool isRow) = CorrectSmudge();

            var mirrorlines_before = FindMirrorLine();

            CorrectSmudge(mirrorlines_before.rowOrColumnIndex, mirrorlines_before.isRow);
            Console.WriteLine(ToString());

            var mirrorlines_after = FindMirrorLine(mirrorlines_before.rowOrColumnIndex, mirrorlines_before.isRow);
            long totalValue = 0;
            for (int i = 0; i < mirrorlines_after.isRow.Length; i++)
            {
                long value = mirrorlines_after.rowOrColumnIndex[i] + 1;
                if (mirrorlines_after.isRow[i]) value *= 100;
                totalValue += value;
                Console.WriteLine("  | Found mirror line at " + (mirrorlines_after.isRow[i] ? "row " : "column ") + mirrorlines_after.rowOrColumnIndex[i]);
                Console.WriteLine("  | Value: " + value);
            }
            Console.WriteLine("> Total: " + totalValue);
            return totalValue;
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
                    switch (mirrors[j, i])
                    {
                        case Terrain.Ash:
                            s += '.';
                            break;
                        case Terrain.Rocks:
                            s += '#';
                            break;
                        case Terrain.Ambiguous:
                            s += 'A';
                            break;
                        default:
                            break;
                    }
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
