using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Day7.Hand;

namespace Day7
{
    public class Hand
    {
        public Card[] cards = new Card[5];
        public HandType type;
        public int bet;

        public enum HandType
        {
            HighCard = 0,
            OnePair = 1,
            TwoPair = 2,
            ThreeOfAKind = 3,
            FullHouse = 4,
            FourOfAKind = 5,
            FiveOfAKind = 6
        }

        public Dictionary<Card, int> cardOccurances = new Dictionary<Card, int>() // <cardValue, occurances>
        {
            {new Card(2), 0},
            {new Card(3), 0},
            {new Card(4), 0},
            {new Card(5), 0},
            {new Card(6), 0},
            {new Card(7), 0},
            {new Card(8), 0},
            {new Card(9), 0},
            {new Card(10),0},
            {new Card(11),0},
            {new Card(12),0},
            {new Card(13),0},
            {new Card(14),0}
        };

        // Create hand from a set of cards and its bet
        public Hand(Card[] cards, int bet)
        {
            this.cards = cards;

            CalculateType();
            this.bet = bet;
        }

        private void CalculateType()
        {
            // For each card we have, add how often it occurs into cardOccurances
            foreach (Card card in cards) cardOccurances[card]++;

            // Depending on how many times each card appears, we know if we have one pair, two pair, full house, three of a kind, etc
            List<int> valuesToIntList = cardOccurances.Values.ToList();

            // If five of a kind
            if (valuesToIntList.Contains(5)) type = HandType.FiveOfAKind;

            // If four of a kind
            else if (valuesToIntList.Contains(4)) type = HandType.FourOfAKind;

            // If has three of a kind
            else if (valuesToIntList.Contains(3))
            {
                // If it has three of a kind, it might still contain a pair and be of type Full House
                if (valuesToIntList.Contains(2)) type = HandType.FullHouse;
                else type = HandType.ThreeOfAKind;
            }

            // If has a pair
            else if (valuesToIntList.Contains(2))
            {
                // Check how many pairs we have to determine if we have one pair or maybe two
                int numberOfPairs = 0;
                for (int i = 0; i < valuesToIntList.Count; i++)
                {
                    if (valuesToIntList[i] == 2) numberOfPairs++;
                }
                if (numberOfPairs == 2) type = HandType.TwoPair;
                else type = HandType.OnePair;
            }

            // Else is just a high card
            else type = HandType.HighCard;
        }

        // Override this to get an easier debug.
        // If we do Console.WriteLine(this) then we can define here what that string interpretation looks like.
        public override string ToString()
        {
            string? s = "";
            foreach (var item in cards)
            {
                s += item.cardLabel;
            }
            return s;
        }

        // We need to override this to ensure our comparisons work (List.Contains() for example, and also ==)
        public override bool Equals(object? obj)
        {
            Hand? otherHand = obj as Hand;
            if (otherHand == null || otherHand.cards.Length != this.cards.Length) return false; // return if null

            bool condition = true; // store condition, condition is that all cards must be the same
            for (int i = 0; i < otherHand.cards.Length; i++)
            {
                condition = condition && (otherHand.cards[i] == this.cards[i]);
            }

            return condition;
        }

        // We need to override this to ensure our comparisons work (List.Contains() for example, and also ==)
        public override int GetHashCode()
        {
            string s = "";
            for (int i = 0; i < cards.Length; i++)
            {
                s += cards[i].cardLabel;
            }
            return s.GetHashCode();
        }
    }
}
