using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day2
{
    public class Game
    {
        public int id;
        public List<Round> rounds;

        public Game(int id, List<Round> rounds)
        {
            this.id = id;
            this.rounds = rounds;
        }

        public Game(string lineOfText)
        {
            rounds = new List<Round>();

            string[] temp = lineOfText.Split(": "); // [0] is "Game 1"
            this.id = int.Parse(temp[0][5..]);

            temp = temp[1].Split("; ");
            foreach (string setOfCubes in temp) // For each set of cubes, for each "draw"
            {
                List<Cube> cubeList = new List<Cube>(); // all cubes in a set
                foreach (string cubeString in setOfCubes.Split(", ")) // for each "5 green"
                {
                    string[] t = cubeString.Split(' '); // t[0] = number,   t[1] = color,   t[1][0] = first letter of the color

                    int nrOfCubes = int.Parse(t[0]);
                    char color = t[1][0]; // first character of the right string

                    // Add all cubes
                    for (int i = 0; i < nrOfCubes; i++)
                    {
                        cubeList.Add(new Cube((Cube.CubeColor)color));
                    }
                }
                Round round = new Round(cubeList);
                rounds.Add(round);
            }
        }
        public override string ToString()
        {
            string str = "";
            str += "G" + id + ":  ";
            foreach (Round r in rounds)
            {
                str += r.ToString();
            }
            return str;
        }

        public bool IsGameLegal(int maxRed, int maxGreen, int maxBlue)
        {
            int totalRed = 0, totalGreen = 0, totalBlue = 0;

            foreach (Round round in rounds)
            {
                totalRed = Math.Max(round.NumberOfCubes(Cube.CubeColor.Red), totalRed);
                totalGreen = Math.Max(round.NumberOfCubes(Cube.CubeColor.Green), totalGreen);
                totalBlue = Math.Max(round.NumberOfCubes(Cube.CubeColor.Blue), totalBlue);
            }
            if (totalRed > maxRed || totalGreen > maxGreen || totalBlue > maxBlue) return false;
            else return true;
        }

        public List<Cube> GetAllCubes()
        {
            List<Cube> listOfCubes = new List<Cube>();
            foreach (Round round in rounds)
            {
                listOfCubes.AddRange(round.cubes);
            }
            return listOfCubes;
        }

        public int GetPower()
        {
            // In a game, this is the multiplied numbers of highest occuring number of a cube.
            // Consider the following:   Game 3:    8 green, 6 blue, 20 red;    5 blue, 4 red, 13 green;    5 green, 1 red
            // redMax = 20,  greenMax = 13,  blueMax = 6
            // Power is 20 * 13 * 6 = 1560
            int maxRed = 0;
            int maxGreen = 0;
            int maxBlue = 0;
            foreach (Round round in rounds)
            {
                maxRed = Math.Max(maxRed, round.NumberOfCubes(Cube.CubeColor.Red));
                maxGreen = Math.Max(maxGreen, round.NumberOfCubes(Cube.CubeColor.Green));
                maxBlue = Math.Max(maxBlue, round.NumberOfCubes(Cube.CubeColor.Blue));
            }
            return (maxRed * maxGreen * maxBlue);
        }
    }

    public class Round
    {
        public List<Cube> cubes;

        public Round(List<Cube> cubes)
        {
            this.cubes = cubes;
        }
        public override string ToString()
        {
            string str = "[";
            int nrRed, nrGreen, nrBlue;
            nrRed = NumberOfCubes(Cube.CubeColor.Red);
            nrGreen = NumberOfCubes(Cube.CubeColor.Green);
            nrBlue = NumberOfCubes(Cube.CubeColor.Blue);

            str += nrRed + "r" + new string(Enumerable.Repeat(' ', Math.Max(3 - nrRed.ToString().Length, 1)).ToArray()) + " ";
            str += nrGreen + "g" + new string(Enumerable.Repeat(' ', Math.Max(3 - nrGreen.ToString().Length, 1)).ToArray()) + " ";
            str += nrBlue + "b";// + new string(Enumerable.Repeat(' ', Math.Max(3 - nrBlue.ToString().Length, 1)).ToArray()) + " ";
            str += "]  ";

            return str;
        }

        public int NumberOfCubes(Cube.CubeColor cubeColor)
        {
            int num = 0;
            foreach (Cube c in this.cubes)
            {
                if (c.color == cubeColor) num++;
            }
            return num;
        }
    }

    public class Cube
    {
        public enum CubeColor
        {
            Red = 'r', Green = 'g', Blue = 'b'
        }
        public CubeColor color;

        public Cube(CubeColor color)
        {
            this.color = color;
        }

        public override string ToString()
        {
            return Char.ToString((char)color);
        }
    }

}
