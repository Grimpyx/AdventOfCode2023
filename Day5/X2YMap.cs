using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Day5
{
    public class X2YMap
    {
        // This is an example of an X2YMap

        // seed-to-soil map:
        // 50 98 2
        // 52 50 48

        // The name X2Y comes from "making a map from X to (2) Y"
        // It contains values in a grid defined by a int[,].
        // It also contains what it translates from, and to what, for example seed to soil


        public enum MapType
        {
            Seed = 0,
            Soil = 1,
            Fertilizer = 2,
            Water = 3 ,
            Light = 4,
            Temperature = 5,
            Humidity = 6,
            Location = 7

        }
        public MapType FromMap { get; private set; }
        public MapType ToMap { get; private set; }

        private readonly string[] maptypeAsString = new string[]
        {
            "seed",
            "soil",
            "fertilizer",
            "water",
            "light",
            "temperature",
            "humidity",
            "location"
        };

        public long[,] values;

        public X2YMap(MapType fromMap, MapType toMap, long[,] values)
        {
            this.FromMap = fromMap;
            this.ToMap = toMap;
            this.values = values;
        }

        // Create map from text input
        public X2YMap(string[] readAllLinesInput)
        {
            string[] x2y = readAllLinesInput[0][..(readAllLinesInput[0].Length-5)].Split('-');
            for (long i = 0; i < maptypeAsString.Length; i++)
            {
                if (x2y[0].ToLower() == maptypeAsString[i]) FromMap = (MapType)i;
                if (x2y[2].ToLower() == maptypeAsString[i]) ToMap = (MapType)i;
            }

            //values = new int[,];

            List<List<long>> list = new List<List<long>>();
            for (long i = 1; i < readAllLinesInput.Length; i++)
            {
                string[] rowNumbersAsString = readAllLinesInput[i].Split(' ');
                List<long> rowNumbers = new List<long>();
                foreach (string nrString in rowNumbersAsString)
                {
                    long nr = long.Parse(nrString);
                    rowNumbers.Add(nr);
                    //Console.WriteLine(nr);
                }
                list.Add(rowNumbers);
            }

            values = new long[list.Count, list[0].Count];
            for (long x = 0; x < list.Count; x++)
            {
                for (long y = 0; y < list[0].Count; y++) values[x,y] = list[(int)x][(int)y];
            }

            /*Console.Write("x2y: ");
            for (int i = 0; i < x2y.Length; i++)
            {
                Console.Write(x2y[i] + " ");
            }
            Console.WriteLine(); // blank line*/
        }


        public long[] GetColumn(long column)
        {
            /*return new int[](Enumerable.Range(0, values.GetLength(1))
                .Select(y => values[column, y])
                .ToArray());*/
            long[] longList = new long[values.GetLength(0)]; // GetLength(0) is number of rows
            for (long i = 0; i < values.GetLength(0); i++) 
            {
                longList[i] = values[i, column];
            }
            return longList;
        }
        public long[] GetRow(long row)
        {
            if (row < 0 || row >= values.GetLength(0)) return new long[] { -1 };

            /*return new int[](Enumerable.Range(0, values.GetLength[0]) // not really sure how this works
                .Select(x => values[x, row]) // select all x values from a specific row
                .ToArray());*/
            //Console.WriteLine("Row length: " + values.GetLength(1));

            long[] longList = new long[values.GetLength(1)]; // GetLength(1) is number of columns
            for (long i = 0; i < values.GetLength(1); i++) 
            {
                longList[i] = values[row,i];
                //Console.WriteLine("val: " + intList[i]);
            }
            return longList;
        }
    }
}
