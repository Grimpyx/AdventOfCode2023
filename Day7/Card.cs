using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day7
{
    public class Card
    {
        readonly Dictionary<char, int> cardMapping = new Dictionary<char, int>()
        {
            {'2', 2},
            {'3', 3},
            {'4', 4},
            {'5', 5},
            {'6', 6},
            {'7', 7},
            {'8', 8},
            {'9', 9},
            {'T', 10},
            {'J', 11},
            {'Q', 12},
            {'K', 13},
            {'A', 14}
        };

        public int cardValue;
        public char cardLabel;
        public Card(char cardLabel)
        {
            this.cardValue = cardMapping[cardLabel];
            this.cardLabel = cardLabel;
        }
        public Card(int cardValue)
        {
            this.cardValue = cardValue;
            foreach (char c in cardMapping.Keys)
            {
                if (cardMapping[c] == cardValue)
                {
                    this.cardLabel = c;
                    break;
                }
            }
        }

        public static Card[] CreateCardListFromString(string str)
        {
            List<Card> cards = new List<Card>();
            foreach (char c in str)
            {
                cards.Add(new Card(c));
            }
            return [..cards];
        }
    }
}
