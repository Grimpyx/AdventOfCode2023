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
        public long handValue;
        public long highCardValue;

        public Hand(Card[] cards)
        {
            this.cards = cards;

            SetHandValues();
            SetHighCard();
            SetScaledValue();
        }

        public enum HandType
        {
            HighCard,
            OnePair,
            TwoPair,
            ThreeOfAKind,
            FullHouse,
            FourOfAKind,
            FiveOfAKind
        }

        Dictionary<HandType, int> handValues = new Dictionary<HandType, int>()
        {
            { HandType.HighCard, 0 },
            { HandType.OnePair, 0 },
            { HandType.TwoPair, 0 },
            { HandType.ThreeOfAKind, 0 },
            { HandType.FullHouse, 0 },
            { HandType.FourOfAKind, 0 },
            { HandType.FiveOfAKind, 0 }
        };

        public string GetHandMap()
        {
            string s = "";
            for (int i = 0; i < 7; i++)
            {
                if (i != 0) s += " ";
                s += handValues[(HandType)i];
            }
            return s;
        }

        private void SetHighCard()
        {
            double total = 0;
            foreach (Card c in cards)
            {
                total += Math.Pow(10, c.cardValue - 2);
            }
            highCardValue = (long)total;
        }
        public int GetActualValue()
        {
            for (int i = handValues.Keys.Count; i >= 0; i--)
            {
                int value = handValues[(HandType)i];
                if (value > 0)
                    return value;
            }
            return -1;
        }
        public void SetScaledValue()
        {
            long totalValue = 0;
            for (int i = 0; i < handValues.Keys.Count; i++)
            {
                totalValue += handValues[(HandType)i] * (long)Math.Pow(100, i-1);
            }
            handValue = totalValue;
        }

        private void SetHandValues()
        {
            List<Card> remainingCards = new List<Card>(cards);
            List<int> sortedCardValue = new List<int>(cards.Select(x => x.cardValue));
            sortedCardValue.Sort();

            for (int i = 0; i < cards.Length; i++)
            {
                for (int j = 0; j < cards.Length; j++) // one pair loop
                {
                    if (j == i) continue;

                    if (cards[i].cardLabel == cards[j].cardLabel) // if 1 pair exists
                    {
                        for (int k = 0; k < cards.Length; k++) // 3 of a kind loop
                        {
                            if (k == j || k == i) continue;

                            if (cards[k].cardLabel == cards[j].cardLabel &&
                                cards[k].cardLabel == cards[i].cardLabel) // if 3 of a kind exists
                            {
                                for (int l = 0; l < cards.Length; l++) // 4 of a kind loop
                                {
                                    if (l == k || l == j || l == i) continue;

                                    if (cards[l].cardLabel == cards[k].cardLabel &&
                                        cards[l].cardLabel == cards[j].cardLabel &&
                                        cards[l].cardLabel == cards[i].cardLabel) // if 4 of a kind exists
                                    {
                                        for (int m = 0; m < cards.Length; m++)  // 5 of a kind loop
                                        {
                                            if (m == l || m == k || m == j || m == i) continue;

                                            if (cards[m].cardLabel == cards[l].cardLabel &&
                                            cards[m].cardLabel == cards[k].cardLabel &&
                                            cards[m].cardLabel == cards[j].cardLabel &&
                                            cards[m].cardLabel == cards[i].cardLabel) // check if 5 of a kind exists
                                            {
                                                handValues[HandType.FiveOfAKind] = cards[m].cardValue;
                                                type = HandType.FiveOfAKind;
                                                goto CheckTwo;
                                            }
                                        } // end 5 of a kind loop
                                        handValues[HandType.FourOfAKind] = cards[l].cardValue;
                                        type = HandType.FourOfAKind;
                                        goto CheckTwo;
                                    }
                                } // end 4 of a kind loop

                                for (int l = 0; l < cards.Length; l++) // full house loop
                                {
                                    if (l == k || l == j || l == i) continue;

                                    for (int m = 0; m < cards.Length; m++) // look for pair loop
                                    {
                                        if (m == l || m == k || m == j || m == i) continue;

                                        if (cards[l].cardLabel == cards[m].cardLabel) // if found other pair, i.e. you have full house
                                        {
                                            // Select the value of three of a kind to assign to the full house value
                                            handValues[HandType.FullHouse] = cards[i].cardValue;
                                            type = HandType.FullHouse;
                                            goto CheckTwo;
                                        }
                                    }
                                } // end full house loop

                                handValues[HandType.ThreeOfAKind] = cards[k].cardValue;
                                type = HandType.ThreeOfAKind;
                                goto CheckTwo;
                            }
                        } // end 3 of a kind loop

                        for (int k = 0; k < cards.Length; k++) // two pair loop
                        {
                            if (k == j || k == i) continue;

                            for (int l = 0; l < cards.Length; l++) // two pair loop
                            {
                                if (l == k || l == j || l == i) continue;

                                if (cards[k].cardLabel == cards[l].cardLabel) // if found other pair
                                {
                                    // Select the value of the highest pair to assign to the two pair value
                                    handValues[HandType.TwoPair] = cards[j].cardValue < cards[k].cardValue ? cards[k].cardValue : cards[j].cardValue;
                                    goto CheckTwo;
                                }
                            }

                        }
                        handValues[HandType.OnePair] = cards[j].cardValue;
                        type = HandType.OnePair;
                        goto CheckTwo;
                    }
                }
            }
            CheckTwo:
            Console.WriteLine();
        }
    }

}
