using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day4
{
    public class Card
    {
        public int id;
        public int[] winningNumbers;
        public int[] numbersYouHaveNumbers;

        public int totalNumberOfWins = 0;

        public Card(int id, int[] winningNumbers, int[] numbersYouHaveNumbers)
        {
            this.id = id;
            this.winningNumbers = winningNumbers;
            this.numbersYouHaveNumbers = numbersYouHaveNumbers;

            CalculateNumberOfWins();
        }

        public Card(string lineInput)
        {
            string[] split = lineInput.Split(": ");

            // split[0]             Take substring "Card   4"
            // ["Card".Length..]    Ignore all characters 
            // .TrimStart()         Remove all white spaces
            this.id = int.Parse(split[0]["Card".Length..].TrimStart());
            split = split[1].Split(" | "); // <-- [0] and [1] the two lists of ints

            this.winningNumbers = ParseNumbers(split[0].Split(' '));
            this.numbersYouHaveNumbers = ParseNumbers(split[1].Split(' '));

            CalculateNumberOfWins();
        }

        private void CalculateNumberOfWins()
        {
            // Calculate total number of wins
            foreach (int nrYouHave in this.numbersYouHaveNumbers)
            {
                foreach (int winNr in this.winningNumbers)
                {
                    if (nrYouHave == winNr)
                    {
                        totalNumberOfWins++;
                    }
                }
            }
        }

        private int[] ParseNumbers(string[] numbersAsString)
        {
            List<int> numbers = new List<int>();
            for (int i = 0; i < numbersAsString.Length; i++)
            {
                if (numbersAsString[i].Trim() == string.Empty) continue;
                numbers.Add(int.Parse(numbersAsString[i].Trim())); // Trim() to remove white spaces in front and afterward
            }
            return numbers.ToArray();
        }

        public override string? ToString()
        {
            string returnString = $"Card {this.id}: ";
            foreach (int number in this.winningNumbers) { returnString += number + " "; }
            returnString += "| ";
            foreach (int number in this.numbersYouHaveNumbers) { returnString += number + " "; }
            return returnString;
        }
    }
}
