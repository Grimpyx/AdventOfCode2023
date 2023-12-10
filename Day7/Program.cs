using Day7;
using System.Runtime.InteropServices;

// 256710623 is wrong
// 256344437 is wrong
// 257375725 is wrong
// Rewriting everything
// 255048101

string[] data = File.ReadAllLines("./data_complete.txt");
List<Hand> hands = new List<Hand>();
//List<int> bets = new List<int>();

for (int i = 0; i < data.Length; i++)
{
    string[] splitData = data[i].Split(' ');
    Hand hand = new Hand(Card.CreateCardListFromString(splitData[0][..5]), int.Parse(splitData[1]));
    hands.Add(hand);
}

foreach (var item in hands)
{
    string valueString = "";
    for (int i = 0; i < item.cards.Length; i++)
    {
        if (i != 0) valueString += " ";
        valueString += item.cards[i].cardValue;
    }
    Console.WriteLine("Hand: " + item.ToString() + "  Value string: " + valueString + "  Type: " + item.type);
}

bool debugWrite = false;
List<Hand> sortedHands = new List<Hand>();
List<Hand> availableHands = new List<Hand>(hands.ToArray());
for (int i = 0; i < hands.Count; i++) // loop through rank
{
    Hand currentTopHand = null;// = hands[i];
    for (int j = 0; j < availableHands.Count; j++)
    {
        if (!availableHands.Contains(availableHands[j]))
        {
            if (debugWrite) Console.WriteLine($"[{i},{j}]" + " skipped " + availableHands[j] + $" (current: {currentTopHand})");
            goto NextHand; // Dont compare with itself
        }

        if (currentTopHand == null)
        {
            if (debugWrite) Console.WriteLine($"[{i},{j}]" + " Set start to " + availableHands[j]);
            currentTopHand = availableHands[0];
            goto NextHand;
        }

        if (currentTopHand.type < availableHands[j].type) // if new card has a more valuable type, set to that
        {
            if (debugWrite) Console.WriteLine($"[{i},{j}]" + "  - Greater type: " + currentTopHand + " < " + availableHands[j] + "  " + currentTopHand.type + " < " + availableHands[j].type);
            currentTopHand = availableHands[j];
            goto NextHand;
        }
        else if (currentTopHand.type > availableHands[j].type)
        {
            if (debugWrite) Console.WriteLine($"[{i},{j}]" + "  - Lesser type: " + currentTopHand + " > " + availableHands[j]);
            goto NextHand;
        }

        if (currentTopHand.type == availableHands[j].type) // if new card has same type, then we compare their cards in order
        {
            if (debugWrite) Console.WriteLine($"[{i},{j}]" + "  -    Same type: " + currentTopHand + " < " + availableHands[j]);
            for (int k = 0; k < availableHands[j].cards.Length; k++)
            {
                if (currentTopHand.cards[k].cardValue == availableHands[j].cards[k].cardValue) // if cards are same, continue look at next card
                {
                    if (debugWrite) Console.WriteLine("    | " + k + " : " + currentTopHand.cards[k].cardLabel + " == " + availableHands[j].cards[k].cardLabel);
                    continue; 
                }

                if (currentTopHand.cards[k].cardValue != availableHands[j].cards[k].cardValue)       // if new card is greater in value, set to that hand
                {
                    if (debugWrite) Console.WriteLine("    | " + k + " : " + currentTopHand.cards[k].cardLabel + " != " + availableHands[j].cards[k].cardLabel);
                    if (currentTopHand.cards[k].cardValue < availableHands[j].cards[k].cardValue)
                    {
                        if (debugWrite) Console.WriteLine("    | " + k + " : " + currentTopHand + " => " + availableHands[j]);
                        currentTopHand = availableHands[j];
                        goto NextHand;
                    }
                    if (debugWrite) Console.WriteLine("    | " + k + " : " + currentTopHand + " stays");

                    goto NextHand;
                }
                Console.WriteLine("ERROR1");
            }
        }
        Console.WriteLine("ERROR2");
        NextHand:
        continue;
    }
    sortedHands.Add(currentTopHand);
    availableHands.Remove(currentTopHand);
    if (debugWrite) Console.WriteLine("             currentTopHand for i=" + i + " => " + currentTopHand + "\n");
}

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


//for (int i = 0; i < hands.Count; i++)
//{
//    int decidedHandID = -1;
//    //Console.WriteLine("");
//    for (int j = 0; j < hands.Count; j++)
//    {
//        if (unavailableHandIDs.Contains(j))
//        {
//            //Console.WriteLine("   / Skipped hand " + j + " cause it was already placed in sortedHands.");
//            continue;
//        }

//        if (sortedHands[i] == null)
//        {
//            sortedbets[i] = bets[j];
//            sortedHands[i] = hands[j];
//            decidedHandID = j;
//            continue;
//        }

//        if (hands[j].type == sortedHands[i].type)
//        {
//            for (int u = 0; u < hands[j].cards.Length; u++)
//            {
//                if (hands[j].cards[u].cardValue == sortedHands[i].cards[u].cardValue) continue;
//                else
//                {
//                    if (hands[j].cards[u].cardValue > sortedHands[i].cards[u].cardValue)
//                    {
//                        sortedbets[i] = bets[j];
//                        sortedHands[i] = hands[j];
//                        decidedHandID = j;
//                        goto MoveToEnd;
//                    }
//                    else
//                        break;
//                }
//            }
//        }


//        if (hands[j].handValue > sortedHands[i].handValue)
//        {
//            sortedbets[i] = bets[j];
//            sortedHands[i] = hands[j];
//            decidedHandID = j;
//            goto MoveToEnd;
//        }

//    MoveToEnd:
//        Console.Write("");

//        // If two hands have the same type, a second ordering rule takes effect.
//        /*if (hands[j].handValue == sortedHands[i].handValue &&
//            hands[j].highCardValue > sortedHands[i].highCardValue)
//        {
//            sortedbets[i] = bets[j];
//            sortedHands[i] = hands[j];
//            decidedHandID = j;
//            continue;
//        }*/



//        /*// If two hands have the same type, a second ordering rule takes effect.
//        if (hands[j].handValue == sortedHands[i].handValue)
//        {
//            for (int u = 0; u < hands[j].cards.Length; u++)
//            {
//                if (hands[j].cards[u].cardValue == sortedHands[i].cards[u].cardValue) continue;
//                else
//                {
//                    if (hands[j].cards[u].cardValue > sortedHands[i].cards[u].cardValue)
//                    {
//                        sortedbets[i] = bets[j];
//                        sortedHands[i] = hands[j];
//                        decidedHandID = j;
//                        break;
//                    }
//                    else
//                        break;
//                }
//            }
//        }*/

//        // If two hands have the same type, a second ordering rule takes effect.


//        //else
//        //    Console.WriteLine("   - Hand " + j + " was below " + "(" + hands[j].highCardValue + " < " + sortedHands[i].highCardValue + ")");
//    }
//    unavailableHandIDs.Add(decidedHandID);
//}

//long[] winningsEach = new long[bets.Count];
//long totalWinnings = 0;
//int y = Console.GetCursorPosition().Top;
//for (int p = 0; p < sortedHands.Count; p++)
//{
//    int rank = (sortedHands.Count - p);
//    winningsEach[p] = sortedbets[p] * rank;
//    totalWinnings += winningsEach[p];
//    Console.SetCursorPosition(2, y+p);
//    Console.Write(" - Rank:" + rank);
//    Console.SetCursorPosition(20, y + p);
//    Console.Write("Hand: " + sortedHands[p].handValue);
//    Console.SetCursorPosition(41, y + p);
//    Console.Write("High card:" + sortedHands[p].highCardValue);
//    Console.SetCursorPosition(68, y + p);
//    Console.Write("Bet:" + sortedbets[p]);
//    Console.SetCursorPosition(85, y + p);
//    Console.Write("Won:" + winningsEach[p]);
//}

//Console.WriteLine("\n\nTotal winnings: " + totalWinnings);