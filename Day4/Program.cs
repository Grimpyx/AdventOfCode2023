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
            string[] data = File.ReadAllLines("./data_complete.txt");

            List<Card> initialCards = new List<Card>();
            List<Card> additionalCards = new List<Card>();
            foreach (string s in data)
            {
                Card newCard = new Card(s);
                initialCards.Add(newCard);
            }
            foreach (Card card in initialCards)
            {
                Console.WriteLine("\n Checking card " + card.id + "   Wins: " + card.totalNumberOfWins);
                additionalCards.AddRange(GetCardsRecursive(card, initialCards));
            }

            int totalCards = initialCards.Count + additionalCards.Count;

            Console.WriteLine(" >>> Total cards: " + totalCards);
        }

        static int indent = -1;
        private static List<Card> GetCardsRecursive(Card card, List<Card> referenceList)
        {
            indent++;
            string indentString = "";
            List<Card> additionalCards = new List<Card>();

            //Console.WriteLine(listCardIndent + "Card " + card.id);
            for (int i = 0; i < indent; i++)
            {
                indentString += "| ";
            }
            //Console.Write("Wins: " + card.totalNumberOfWins);
            for (int i = 0; i < card.totalNumberOfWins; i++)
            {
                int index = Math.Clamp(card.id + i, 0, referenceList.Count - 1);
                Card newCard = referenceList[index];


                // Write
                // This works really well, but it takes an insane amount of time to draw in the console.
                // around 19 million cards will be obtained
                /*Console.Write(" > Wins: " + newCard.totalNumberOfWins);
                (int posx, int posy) = Console.GetCursorPosition();
                Console.SetCursorPosition(posx + 7, posy);
                Console.WriteLine(indentString + "Card " + newCard.id);*/

                additionalCards.Add(newCard); // -1 cause range is 1,2,3,4 but we need index range 0,1,2,3
                additionalCards.AddRange(GetCardsRecursive(newCard, referenceList));
            }
            //listCardIndent = listCardIndent[(listCardIndent.Length)..];
            //listCardIndent = listCardIndent.Remove(2);
            indent--;
            return additionalCards;

            /*List<Card> additionalCards = new List<Card>();
            foreach (Card card in initialCards)
            {
                Console.WriteLine(" " + card);
                for (int i = 0; i < card.totalNumberOfWins; i++)
                {
                    Card newCard = initialCards[card.id + i];
                    Console.WriteLine(" > " + newCard);
                    additionalCards.Add(newCard); // -1 cause range is 1,2,3,4 but we need index range 0,1,2,3
                }
            }
            additionalCards.AddRange(ListCard(additionalCards));
            return additionalCards;*/
        }
    }
}