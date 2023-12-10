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
            {new Card(1), 0},
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
            {new Card(13),0}
        };

        // Create hand from a set of cards and its bet
        public Hand(Card[] cards, int bet, bool correctForJokers = false)
        {
            this.cards = cards;

            if (correctForJokers) CalculateTypeForJokers();
            else CalculateType();
            //CalculateType1();
            this.bet = bet;
        }

        private void CalculateType1()
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

        // Calculates the type from the 
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


        void CalculateTypeForJokers()
        {
            // Generate all possible hands
            List<Hand> generatedHands = GetAllJokerHands(this);
            
            if (generatedHands.Count <= 0) return;

            // Sort hands
            List<Hand> sortedGeneratedHands = Hand.ReturnSortedListOfHands(generatedHands, false);

            // Pick
            Card[] oldCards = cards;

            type = sortedGeneratedHands[0].type;
            cards = sortedGeneratedHands[0].cards;  // the function after this requires cards to be set, so we set it now and revert later

            CalculateType();
            cards = oldCards; // restore cards, but keep the type generated
        }

        private List<Hand> GetAllJokerHands(Hand fromHand)
        {
            // Special cases to lighten the load on the recursive stuff
            int numberOfJokers = 0;
            for (int jokerIndex = 0; jokerIndex < fromHand.cards.Length; jokerIndex++)
            {
                if (fromHand.cards[jokerIndex].cardLabel == 'J') numberOfJokers++;
            }
            if (numberOfJokers == 5) return new List<Hand> { new Hand(Card.CreateCardListFromString("AAAAA"), fromHand.bet) };
            if (numberOfJokers == 4)
            {
                string otherChar = "";
                // Find the card that is not a 
                for (int jokerIndex = 0; jokerIndex < fromHand.cards.Length; jokerIndex++)
                {
                    if (fromHand.cards[jokerIndex].cardLabel != 'J')
                    {
                        otherChar = new string([fromHand.cards[jokerIndex].cardLabel]);
                        break;
                    }
                }
                string newString = otherChar + otherChar + otherChar + otherChar + otherChar;

                return new List<Hand> { new Hand(Card.CreateCardListFromString(newString), fromHand.bet) };
            }

            List<Hand> generatedHands = new List<Hand>();
            for (int jokerIndex = 0; jokerIndex < fromHand.cards.Length; jokerIndex++)
            {
                // If card is joker
                if (fromHand.cards[jokerIndex].cardLabel == 'J')
                {
                    foreach (char c in Card.cardMapping.Keys)
                    {
                        if (c == 'J') continue;

                        char[] cardsCharArray = fromHand.cards.Select(x => x.cardLabel).ToArray();
                        cardsCharArray[jokerIndex] = c;

                        Card[] generatedHandCards = Card.CreateCardListFromString(new string(cardsCharArray));

                        Hand newHand = new Hand(generatedHandCards, fromHand.bet, true);
                        generatedHands.AddRange(GetAllJokerHands(newHand)); // this recursively creates hands that in turn check for more jokers
                        //Console.WriteLine("::   " + fromHand + " => " + newHand + "                     type: " + newHand.type);
                    }
                }
            }
            if (generatedHands.Count <= 0) // then no jokers were found
                return new List<Hand> { fromHand };

            return generatedHands;
        }

        public static List<Hand> ReturnSortedListOfHands(List<Hand> hands, bool writeOutput = false)
        {
            List<Hand> sortedHands = new List<Hand>();
            List<Hand> availableHands = new List<Hand>(hands.ToArray()); // For each card we put into the sorted list, we track what cards we have left
            for (int i = 0; i < hands.Count; i++) // loop through rank
            {
                Hand? currentTopHand = null; // Is set to be the first available in
                for (int j = 0; j < availableHands.Count; j++)
                {
                    // If current top card is yet to be set, we set to the first available one (availableHands[0])
                    if (currentTopHand == null)
                    {
                        if (writeOutput) Console.WriteLine($"[{i},{j}]" + " Set start to " + availableHands[j]);
                        currentTopHand = availableHands[0];
                        goto NextHand;
                    }

                    // if new card has a more valuable type, set to that
                    if (currentTopHand.type < availableHands[j].type)
                    {
                        if (writeOutput) Console.WriteLine($"[{i},{j}]" + "  - Greater type: " + currentTopHand + " < " + availableHands[j] + "  " + currentTopHand.type + " < " + availableHands[j].type);
                        currentTopHand = availableHands[j];
                        goto NextHand;
                    }
                    // Go to the next hand if the current one is more valuable
                    else if (currentTopHand.type > availableHands[j].type)
                    {
                        if (writeOutput) Console.WriteLine($"[{i},{j}]" + "  - Lesser type: " + currentTopHand + " > " + availableHands[j]);
                        goto NextHand;
                    }

                    // if new hand has same type as the current top contender, then we have to compare their cards in order
                    if (currentTopHand.type == availableHands[j].type)
                    {
                        if (writeOutput) Console.WriteLine($"[{i},{j}]" + "  -    Same type: " + currentTopHand + " = " + availableHands[j]);
                        // Index k describes the card index. We start comparing index 0
                        for (int k = 0; k < availableHands[j].cards.Length; k++)
                        {
                            // If the cards compared are the same, look at the next card (continue)
                            if (currentTopHand.cards[k].cardValue == availableHands[j].cards[k].cardValue)
                            {
                                if (writeOutput) Console.WriteLine("    | " + k + " : " + currentTopHand.cards[k].cardLabel + " == " + availableHands[j].cards[k].cardLabel);
                                continue;
                            }

                            // If the cards are different
                            if (currentTopHand.cards[k].cardValue != availableHands[j].cards[k].cardValue)
                            {
                                if (writeOutput) Console.WriteLine("    | " + k + " : " + currentTopHand.cards[k].cardLabel + " != " + availableHands[j].cards[k].cardLabel);
                                // If the card of the new hand has greater value than the current top contender's, we choose it
                                if (currentTopHand.cards[k].cardValue < availableHands[j].cards[k].cardValue)
                                {
                                    if (writeOutput) Console.WriteLine("    | " + k + " : " + currentTopHand + " => " + availableHands[j]);
                                    currentTopHand = availableHands[j];
                                    goto NextHand;
                                }
                                else
                                {
                                    if (writeOutput) Console.WriteLine("    | " + k + " : " + currentTopHand + " stays");
                                    goto NextHand;
                                }
                            }
                            //Console.WriteLine($"(1) HOW DID YOU GET HERE?    CURTOP:{currentTopHand}   CURCOMPARE:{availableHands[j]}");
                        }
                    }
                    // If you come here, you have two identical hands. This means they should have the same rank.
                    //Console.WriteLine($"(2) HOW DID YOU GET HERE?    CURTOP:{currentTopHand}   CURCOMPARE:{availableHands[j]}");
                NextHand:
                    continue;
                }

                // At the end of each ranking spot, we have to remove that hand from the list of available hands.
                // Otherwise we might add it multiple times to the sorted hands.
                if (currentTopHand == null) continue;
                sortedHands.Add(currentTopHand);
                availableHands.Remove(currentTopHand);
                if (writeOutput) Console.WriteLine("             currentTopHand for i=" + i + " => " + currentTopHand + "\n");
            }
            return sortedHands;

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
            if (obj is not Hand otherHand || otherHand.cards.Length != cards.Length)
            {
                return false; // return if null
            }

            bool condition = true; // store condition, condition is that all cards must be the same
            for (int i = 0; i < otherHand.cards.Length; i++)
            {
                condition = condition && (otherHand.cards[i].cardValue == this.cards[i].cardValue);
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
