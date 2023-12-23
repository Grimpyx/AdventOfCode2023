using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day14
{
    public class Map2D<T>
    {
        public T[,] map;

        public Map2D(T[,] map)
        {
            this.map = map;
        }

        public T? GetValue(int x, int y)
        {
            if (x < 0 || y < 0) return default;

            return map[x, y];
        }

        public void SetValue(int x, int y, T value) => map[x, y] = value;

        public T[] GetRow(int row)
        {
            return [.. Enumerable.Range(0, map.GetLength(0))
                .Select(x => map[x, row])];

            // Explanation:
            // Enumerable.Range(x,y) generate a list of integers from x to y. (x, x+1, x+2, ..., y-2, y-1)
            // in .Select() we use each integer from the generated list to select all mirrors[LISTVALUE, row]
            // We dont need .ToArray() because we already have it in a collection expression (ie [.. collectionOfValues])
        }
        public T[] GetColumn(int column)
        {
            return [.. Enumerable.Range(0, map.GetLength(1))
                .Select(y => map[column, y])];

            // Explanation:
            // Enumerable.Range(x,y) generate a list of integers from x to y. (x, x+1, x+2, ..., y-2, y-1)
            // in .Select() we use each integer from the generated list to select all mirrors[LISTVALUE, row]
            // We dont need .ToArray() because we already have it in a collection expression (ie [.. collectionOfValues])
        }

        public void UpdateColumn(int column, int index, T value) => map[column, index] = value;
        public void UpdateRow(int row, int index, T value) => map[index, row] = value;

        public int RowLength => map.GetLength(1);
        public int ColumnLength => map.GetLength(0);

        public override string? ToString()
        {
            try
            {
                string s = "";

                for (int i = 0; i < ColumnLength; i++)
                {
                    if (i != 0) s += "\n";
                    for (int j = 0; j < RowLength; j++)
                    {
                        s += map[j, i].ToString();
                    }
                }

                return s;
            }
            catch (Exception e)
            {
                return "";
                throw;
            }

        }
    }
}
