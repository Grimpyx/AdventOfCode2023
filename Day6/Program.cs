// See https://aka.ms/new-console-template for more information
using Day6;

Console.WriteLine("Hello, World!");

string[] data = File.ReadAllLines("./data_test.txt");
int[] times = DataInterpret.GetNumbersFrom(data[0], ' ');
int[] distances = DataInterpret.GetNumbersFrom(data[1], ' ');