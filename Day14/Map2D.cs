using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day14
{
    public class Map2D<T>
    {
        public T[,] values;

        public Map2D(T[,] map)
        {
            this.values = map;
        }

        public T? GetValue(int x, int y)
        {
            if (x < 0 || y < 0) return default;

            return values[x, y];
        }

        public void SetValue(int x, int y, T value) => values[x, y] = value;

        public T[] GetRow(int row)
        {
            return [.. Enumerable.Range(0, values.GetLength(0))
                .Select(x => values[x, row])];

            // Explanation:
            // Enumerable.Range(x,y) generate a list of integers from x to y. (x, x+1, x+2, ..., y-2, y-1)
            // in .Select() we use each integer from the generated list to select all mirrors[LISTVALUE, row]
            // We dont need .ToArray() because we already have it in a collection expression (ie [.. collectionOfValues])
        }
        public T[] GetColumn(int column)
        {
            return [.. Enumerable.Range(0, values.GetLength(1))
                .Select(y => values[column, y])];

            // Explanation:
            // Enumerable.Range(x,y) generate a list of integers from x to y. (x, x+1, x+2, ..., y-2, y-1)
            // in .Select() we use each integer from the generated list to select all mirrors[LISTVALUE, row]
            // We dont need .ToArray() because we already have it in a collection expression (ie [.. collectionOfValues])
        }

        public void UpdateColumn(int column, int index, T value) => values[column, index] = value;
        public void UpdateRow(int row, int index, T value) => values[index, row] = value;

        public int RowLength => values.GetLength(1);
        public int ColumnLength => values.GetLength(0);

        public override string? ToString()
        {
            try
            {
                string s = "";

                for (int i = 0; i < RowLength; i++)
                {
                    if (i != 0) s += "\n";
                    for (int j = 0; j < ColumnLength; j++)
                    {
                        s += values[j, i].ToString();
                    }
                }

                return s;
            }
            catch// (Exception e)
            {
                return "";
                throw;
            }

        }
    }
}
