// See https://aka.ms/new-console-template for more information
using Day6;

Console.WriteLine("Hello, World!");

string[] data = File.ReadAllLines("./data_test.txt");
DataInterpret.GetNumbersFrom(data[0], ' ');