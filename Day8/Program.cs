
using Day8;

// Answers P1:
// 457 is too low
// 19951 was correct! I had not set start/destination to AAA->ZZZ (it was the start of the keys, and end of keys)

// Answers P2:

CamelNetwork net = new CamelNetwork(File.ReadAllLines("./data_complete.txt"));
//Console.WriteLine(" > Steps to complete: " + net.FindStepsToReach("AAA", "ZZZ")); //p1
Console.WriteLine(" > Steps to complete: " + net.FindStepsToReachGhostMode()); //p2