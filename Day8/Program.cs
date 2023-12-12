
using Day8;

// Answers P1:
// 457 is too low
// 19951 was correct! I had not set start/destination to AAA->ZZZ (it was the start of the keys, and end of keys)

// Answers P2:
// 16342438708751 (from my numeric finder) (this calculator confirms http://www.alcula.com/calculators/math/lcm/)
// 20906588438832807 (first result from my non numeric but it didnt work)
// I had to let it be done recursively ((A*B)/lcm(A,B) didnt work when I first got the product of all factors and divided by their lcm, i.e. PRODUCT(A,B,..,Z)/lcm(A,B,..,Z) didnt work)
// Now I got 16342438708751 just like I did from my brute force algorithm.
// It was the correct answer!!

CamelNetwork net = new CamelNetwork(File.ReadAllLines("./data_complete.txt"));
Console.WriteLine("LCM: " + net.FindStepsToReachGhostMode());