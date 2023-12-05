using System;

namespace Day4
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //PartOne();
            PartTwo();
        }

        private static void PartOne()
        {
            string[] data = File.ReadAllLines("./data_complete.txt");

            List<Card> cards = new List<Card>();
            foreach (string s in data)
            {
                cards.Add(new Card(s));
            }

            int totalPoints = 0;
            foreach (Card card in cards)
            {
                // Determine points for the card
                int points = 0;
                foreach (int nrYouHave in card.numbersYouHaveNumbers)
                {
                    foreach (int winNr in card.winningNumbers)
                    {
                        if (nrYouHave == winNr)
                        {
                            points *= 2;
                            if (points == 0) points = 1;
                        }
                    }
                }
                totalPoints += points;

                Console.WriteLine(card.ToString() + "  > Points: " + points);
            }
            Console.WriteLine(" >>> Total points: " + totalPoints);
        }

        private static void PartTwo()
        {
            string[] data = File.ReadAllLines("./data_test.txt");

            List<Card> initialCards = new List<Card>();
            foreach (string s in data)
            {
                initialCards.Add(new Card(s));
            }

            int totalCards = initialCards.Count;

            foreach (Card card in initialCards)
            {
                Console.WriteLine(" " + card);
                List<Card> additionalCards = new List<Card>();
                for (int i = 0; i < card.totalNumberOfWins; i++)
                {
                    Card newCard = initialCards[card.id + i];
                    Console.WriteLine(" > " + newCard);
                    additionalCards.Add(newCard); // -1 cause range is 1,2,3,4 but we need index range 0,1,2,3
                }
                totalCards += additionalCards.Count;

                //Console.WriteLine(card.ToString() + "  > Points: " + points);
            }
            Console.WriteLine(" >>> Total cards: " + totalCards);
        }

        private static List<Folder> ListFolders(Folder initialFolder)
    }
}