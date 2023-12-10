using Day7;
using System.Runtime.InteropServices;

// 256710623 is wrong
// 256344437 is wrong
// 257375725 is wrong

// After rewriting everything:
// 255048101 is correct answer.

// Part 2:
// 252323263 is too low.
// 253718286 was correct!

bool partTwo = true;

Console.WriteLine("Processing...");
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
    Hand hand = new Hand(cards, bet, partTwo);
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
List<Hand> sortedHands = Hand.ReturnSortedListOfHands(hands, debugWrite);

// Print the result
long[] winningsEach = new long[hands.Count];
long totalWinnings = 0;
int y = Console.GetCursorPosition().Top;
int offset = 0;
int distinctLength = sortedHands.Distinct().ToArray().Length;
for (int p = 0; p < sortedHands.Count; p++)
{
    //if (p > 0) Console.WriteLine("   Comp: " + sortedHands[p - 1] + " == " + sortedHands[p] + "   eval: " + sortedHands[p - 1].Equals(sortedHands[p].GetHashCode()));
    if (p > 0 && sortedHands[p - 1].Equals(sortedHands[p])) offset++;
    int rank = (distinctLength - p + offset);
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