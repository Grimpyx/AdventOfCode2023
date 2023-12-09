using Day7;

// 256710623 is wrong
// 256344437 is wrong
// 257375725 is wrong

string[] data = File.ReadAllLines("./data_complete.txt");
List<Hand> hands = new List<Hand>();
List<int> bets = new List<int>();

for (int i = 0; i < data.Length; i++)
{
    string[] splitData = data[i].Split(' ');
    Hand hand = new Hand(Card.CreateCardListFromString(splitData[0][..5]));
    hands.Add(hand);
    bets.Add(int.Parse(splitData[1]));
    Console.WriteLine("Map:   " + hand.GetHandMap());
    Console.WriteLine("Value: " + hand.handValue + "\n------------------------\n");
}


long[] scaledHandValues = new long[hands.Count];
for (int i = 0; i < hands.Count; i++)
{
    scaledHandValues[i] = hands[i].highCardValue;
    Console.WriteLine(scaledHandValues[i]);
}

List<Hand> sortedHands = new List<Hand>(new Hand[hands.Count]);
List<int> sortedbets = new List<int>(new int[bets.Count]);
List<int> unavailableHandIDs = new List<int >();
for (int i = 0; i < hands.Count; i++)
{
    int decidedHandID = -1;
    //Console.WriteLine("");
    for (int j = 0; j < hands.Count; j++)
    {
        if (unavailableHandIDs.Contains(j))
        {
            //Console.WriteLine("   / Skipped hand " + j + " cause it was already placed in sortedHands.");
            continue;
        }

        if (sortedHands[i] == null)
        {
            sortedbets[i] = bets[j];
            sortedHands[i] = hands[j];
            decidedHandID = j;
            continue;
        }

        if (hands[j].type == sortedHands[i].type)
        {
            for (int u = 0; u < hands[j].cards.Length; u++)
            {
                if (hands[j].cards[u].cardValue == sortedHands[i].cards[u].cardValue) continue;
                else
                {
                    if (hands[j].cards[u].cardValue > sortedHands[i].cards[u].cardValue)
                    {
                        sortedbets[i] = bets[j];
                        sortedHands[i] = hands[j];
                        decidedHandID = j;
                        goto MoveToEnd;
                    }
                    else
                        break;
                }
            }
        }


        if (hands[j].handValue > sortedHands[i].handValue)
        {
            sortedbets[i] = bets[j];
            sortedHands[i] = hands[j];
            decidedHandID = j;
            goto MoveToEnd;
        }

        MoveToEnd:
        Console.Write("");

        // If two hands have the same type, a second ordering rule takes effect.
        /*if (hands[j].handValue == sortedHands[i].handValue &&
            hands[j].highCardValue > sortedHands[i].highCardValue)
        {
            sortedbets[i] = bets[j];
            sortedHands[i] = hands[j];
            decidedHandID = j;
            continue;
        }*/



        /*// If two hands have the same type, a second ordering rule takes effect.
        if (hands[j].handValue == sortedHands[i].handValue)
        {
            for (int u = 0; u < hands[j].cards.Length; u++)
            {
                if (hands[j].cards[u].cardValue == sortedHands[i].cards[u].cardValue) continue;
                else
                {
                    if (hands[j].cards[u].cardValue > sortedHands[i].cards[u].cardValue)
                    {
                        sortedbets[i] = bets[j];
                        sortedHands[i] = hands[j];
                        decidedHandID = j;
                        break;
                    }
                    else
                        break;
                }
            }
        }*/

        // If two hands have the same type, a second ordering rule takes effect.


        //else
        //    Console.WriteLine("   - Hand " + j + " was below " + "(" + hands[j].highCardValue + " < " + sortedHands[i].highCardValue + ")");
    }
    unavailableHandIDs.Add(decidedHandID);
}

long[] winningsEach = new long[bets.Count];
long totalWinnings = 0;
int y = Console.GetCursorPosition().Top;
for (int p = 0; p < sortedHands.Count; p++)
{
    int rank = (sortedHands.Count - p);
    winningsEach[p] = sortedbets[p] * rank;
    totalWinnings += winningsEach[p];
    Console.SetCursorPosition(2, y+p);
    Console.Write(" - Rank:" + rank);
    Console.SetCursorPosition(20, y + p);
    Console.Write("Hand: " + sortedHands[p].handValue);
    Console.SetCursorPosition(41, y + p);
    Console.Write("High card:" + sortedHands[p].highCardValue);
    Console.SetCursorPosition(68, y + p);
    Console.Write("Bet:" + sortedbets[p]);
    Console.SetCursorPosition(85, y + p);
    Console.Write("Won:" + winningsEach[p]);
}

Console.WriteLine("\n\nTotal winnings: " + totalWinnings);