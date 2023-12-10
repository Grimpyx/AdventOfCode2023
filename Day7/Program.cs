using Day7;
using System.Runtime.InteropServices;

// 256710623 is wrong
// 256344437 is wrong
// 257375725 is wrong
// Rewriting everything
// 255048101

string[] data = File.ReadAllLines("./data_complete.txt");
List<Hand> hands = new List<Hand>();

// Split data into hands
for (int i = 0; i < data.Length; i++)
{
    // splitData[0] is the cards, splitData[1] is the bet
    string[] splitData = data[i].Split(' ');

    // Creates a hand from a set of cards
    // [..5] means take the 5 first characters from the string
    Card[] cards = Card.CreateCardListFromString(splitData[0][..5]);
    int bet = int.Parse(splitData[1]);
    Hand hand = new Hand(cards, bet);
    hands.Add(hand);
}

// Display data
bool displayInputInterpretation = false;
foreach (var item in hands)
{
    string valueString = "";
    for (int i = 0; i < item.cards.Length; i++)
    {
        if (i != 0) valueString += " ";
        valueString += item.cards[i].cardValue;
    }
    if (displayInputInterpretation) Console.WriteLine("Hand: " + item.ToString() + "  Value string: " + valueString + "  Type: " + item.type);
}

// Sort hands from rank
bool debugWrite = false;
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
            if (debugWrite) Console.WriteLine($"[{i},{j}]" + " Set start to " + availableHands[j]);
            currentTopHand = availableHands[0];
            goto NextHand;
        }

        // if new card has a more valuable type, set to that
        if (currentTopHand.type < availableHands[j].type)
        {
            if (debugWrite) Console.WriteLine($"[{i},{j}]" + "  - Greater type: " + currentTopHand + " < " + availableHands[j] + "  " + currentTopHand.type + " < " + availableHands[j].type);
            currentTopHand = availableHands[j];
            goto NextHand;
        }
        // Go to the next hand if the current one is more valuable
        else if (currentTopHand.type > availableHands[j].type)
        {
            if (debugWrite) Console.WriteLine($"[{i},{j}]" + "  - Lesser type: " + currentTopHand + " > " + availableHands[j]);
            goto NextHand;
        }

        // if new hand has same type as the current top contender, then we have to compare their cards in order
        if (currentTopHand.type == availableHands[j].type)
        {
            if (debugWrite) Console.WriteLine($"[{i},{j}]" + "  -    Same type: " + currentTopHand + " < " + availableHands[j]);
            // Index k describes the card index. We start comparing index 0
            for (int k = 0; k < availableHands[j].cards.Length; k++)
            {
                // If the cards compared are the same, look at the next card (continue)
                if (currentTopHand.cards[k].cardValue == availableHands[j].cards[k].cardValue)
                {
                    if (debugWrite) Console.WriteLine("    | " + k + " : " + currentTopHand.cards[k].cardLabel + " == " + availableHands[j].cards[k].cardLabel);
                    continue; 
                }

                // If the cards are different
                if (currentTopHand.cards[k].cardValue != availableHands[j].cards[k].cardValue)
                {
                    if (debugWrite) Console.WriteLine("    | " + k + " : " + currentTopHand.cards[k].cardLabel + " != " + availableHands[j].cards[k].cardLabel);
                    // If the card of the new hand has greater value than the current top contender's, we choose it
                    if (currentTopHand.cards[k].cardValue < availableHands[j].cards[k].cardValue)
                    {
                        if (debugWrite) Console.WriteLine("    | " + k + " : " + currentTopHand + " => " + availableHands[j]);
                        currentTopHand = availableHands[j];
                        goto NextHand;
                    }
                    else
                    {
                        if (debugWrite) Console.WriteLine("    | " + k + " : " + currentTopHand + " stays");
                        goto NextHand;
                    }
                }
                Console.WriteLine("HOW DID YOU GET HERE?");
            }
        }
        Console.WriteLine("HOW DID YOU GET HERE?");
        NextHand:
        continue;
    }

    // At the end of each ranking spot, we have to remove that hand from the list of available hands.
    // Otherwise we might add it multiple times to the sorted hands.
    if (currentTopHand == null) continue;
    sortedHands.Add(currentTopHand);
    availableHands.Remove(currentTopHand);
    if (debugWrite) Console.WriteLine("             currentTopHand for i=" + i + " => " + currentTopHand + "\n");
}


// Print the result
long[] winningsEach = new long[hands.Count];
long totalWinnings = 0;
int y = Console.GetCursorPosition().Top;
for (int p = 0; p < sortedHands.Count; p++)
{
    int rank = (sortedHands.Count - p);
    winningsEach[p] = sortedHands[p].bet * rank;
    totalWinnings += winningsEach[p];
    Console.SetCursorPosition(2, y + p);
    Console.Write(" - Rank:" + rank);
    Console.SetCursorPosition(20, y + p);
    Console.Write("Hand: " + sortedHands[p].ToString());
    Console.SetCursorPosition(35, y + p);
    Console.Write("Bet: " + sortedHands[p].bet);
    Console.SetCursorPosition(48, y + p);
    Console.Write("Won: " + winningsEach[p]);
    Console.SetCursorPosition(62, y + p);
    Console.Write("Type: " + sortedHands[p].type);
}

Console.WriteLine("\n\nTotal winnings: " + totalWinnings);