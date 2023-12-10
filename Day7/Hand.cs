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
        //public long handValue;
        //public long highCardValue;

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

        public Hand(Card[] cards, int bet)
        {
            this.cards = cards;

            CalculateType();
            this.bet = bet;
            //SetHighCard();
            //SetScaledValue();
        }

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

        /*Dictionary<HandType, int> handValues = new Dictionary<HandType, int>()
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
        }*/

        /*private void SetHighCard()
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
        }*/

        private void CalculateType()
        {
            /*
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
                        for (int k = 0; k < cards.Length; k++) // 3 of a kind loop & twopair loop
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

                            if (cards[k].cardLabel == cards[j].cardLabel &&
                                cards[k].cardLabel == cards[i].cardLabel) // if two pair exists
                            {

                            }
                        } // end 3 of a kind loop & twopair loop

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
            Console.WriteLine();*/

            foreach (Card card in cards) cardOccurances[card]++;

            //  Determine how many cards
            List<int> valuesToIntList = cardOccurances.Values.ToList();
            if (valuesToIntList.Contains(5)) type = HandType.FiveOfAKind;
            else if (valuesToIntList.Contains(4)) type = HandType.FourOfAKind;
            else if (valuesToIntList.Contains(3))
            {
                if (valuesToIntList.Contains(2)) type = HandType.FullHouse;
                else type = HandType.ThreeOfAKind;
            }
            else if (valuesToIntList.Contains(2))
            {
                int numberOfPairs = 0;
                for (int i = 0; i < valuesToIntList.Count; i++)
                {
                    if (valuesToIntList[i] == 2) numberOfPairs++;
                }
                if (numberOfPairs == 2) type = HandType.TwoPair;
                else type = HandType.OnePair;
            }
            else type = HandType.HighCard;

            Console.WriteLine(" / Calculating Type for " + this.ToString() + "\n   Type: " + type);
        }

        public override string ToString()
        {
            string? s = "";
            foreach (var item in cards)
            {
                s += item.cardLabel;
            }
            return s;
        }

        public override bool Equals(object? obj)
        {
            Hand otherHand = obj as Hand;
            if (otherHand == null || otherHand.cards.Length != this.cards.Length) return false; // return if null

            bool condition = true; // store condition, condition is that all cards must be the same
            for (int i = 0; i < otherHand.cards.Length; i++)
            {
                condition = condition && (otherHand.cards[i] == this.cards[i]);
            }

            return condition;
        }

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
