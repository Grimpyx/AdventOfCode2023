// Link
// https://adventofcode.com/2023/day/5

namespace Day5
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            PartOne();
            //PartTwo();
        }

        private static void PartOne()
        {
            string[] data = File.ReadAllLines("./data_test.txt");

            X2YMap map = new X2YMap(data[2..4]);
            Console.WriteLine();
        }
    }
}