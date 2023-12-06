using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
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
        MapType fromMap, toMap;
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

        public int[,] values;

        public X2YMap(MapType fromMap, MapType toMap, int[,] values)
        {
            this.fromMap = fromMap;
            this.toMap = toMap;
            this.values = values;
        }

        // Create map from text input
        public X2YMap(string[] readAllLinesInput)
        {
            string[] x2y = readAllLinesInput[0][..(readAllLinesInput[0].Length-5)].Split('-');
            for (int i = 0; i < maptypeAsString.Length; i++)
            {
                if (x2y[0].ToLower() == maptypeAsString[i]) fromMap = (MapType)i;
                if (x2y[2].ToLower() == maptypeAsString[i]) toMap = (MapType)i;
            }

            values = new int[,];
            for (int i = 1; i < readAllLinesInput.Length; i++)
            {
                string[] rowNumbersAsString = readAllLinesInput[i].Split(' ');
                int[] rowNumbers = new int[rowNumbersAsString.Length];
                for (int nr = 0; nr < rowNumbers.Length; nr++)
                {

                }
            }


            /*Console.Write("x2y: ");
            for (int i = 0; i < x2y.Length; i++)
            {
                Console.Write(x2y[i] + " ");
            }
            Console.WriteLine(); // blank line*/
        }


        public int[] GetColumn(int column)
        {
            /*return new int[](Enumerable.Range(0, values.GetLength(1))
                .Select(y => values[column, y])
                .ToArray());*/
            int[] intList = new int[values.GetLength(1)]; // GetLength(1) is number of rows
            for (int i = 0; i < intList.Length; i++) 
            {
                intList[i] = values[column, i];
            }
            return intList;
        }
        public int[] GetRow(int row)
        {
            /*return new int[](Enumerable.Range(0, values.GetLength[0]) // not really sure how this works
                .Select(x => values[x, row]) // select all x values from a specific row
                .ToArray());*/

            int[] intList = new int[values.GetLength(0)]; // GetLength(0) is number of columns
            for (int i = 0; i < intList.Length; i++) 
            {
                intList[i] = values[i, row];
            }
            return intList;
        }
    }
}
