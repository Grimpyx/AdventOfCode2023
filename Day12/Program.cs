


//GalaxyMap galaxyMap = new GalaxyMap(File.ReadAllLines("./data_test_p1.txt"));
using Day12;
using System.Diagnostics;
using System.Threading;

Stopwatch sw1 = Stopwatch.StartNew();

//string[] data = File.ReadAllLines("./data_test_p1.txt");
string[] data = File.ReadAllLines("./data_complete.txt");
List<int> completedIterations = new List<int>();
long totalNumberOfWays = 0;
AllSprings springs = new AllSprings(data, true);

foreach (var row in springs.springRows)
{
    string nrstring = "";
    foreach (var item in row.continuousGroups)
    {
        nrstring += item.ToString() + ",";
    }

    Console.WriteLine(" | " + row.SpringsAsString() + " " + nrstring[..(nrstring.Length-1)]);
    long v = row.CreatePermutations();
    totalNumberOfWays += v;
    Console.WriteLine(" | Value: " + v);
    Console.WriteLine(" | Total: " + totalNumberOfWays + "\n");
}


//Console.ForegroundColor = ConsoleColor.Green;

//Mutex mutex = new Mutex();
//List<Task> allTasks = new List<Task>();
//ThreadPool.SetMaxThreads(8,8);
//ThreadPool.SetMaxThreads(64,64);
//for (int i = 0; i < data.Length; i++)
//{
//    /*int banana = i;
//    Task task = Task.Run(() => { ThreadTask(banana); });
//    allTasks.Add(task);*/

//    int banana = i;
//    //Thread t = new Thread(new ThreadStart(() => ThreadTask(banana)));
//    //ThreadPool.QueueUserWorkItem(new WaitCallback((v) => TestMethod(v, 2)));
//    ThreadPool.QueueUserWorkItem(new WaitCallback((v) => ThreadTask(v, banana)));
//    //t.Start();
//    //Thread t = new Thread(() => ThreadTask(i));
//    //t.Start();
//    //completedIterations.Sort();
//}

////Console.WriteLine(ThreadPool.ThreadCount);
////Console.WriteLine();

//while (completedIterations.Count < data.Length)
//{
//    Thread.Sleep(5000);
//}
//sw1.Stop();
//Console.WriteLine("> Completed.");
//Console.WriteLine("> Total number of ways = " + totalNumberOfWays);

//Console.WriteLine("\n Took time " + sw1.ElapsedMilliseconds + "ms\n\n");

///*foreach (Task task in allTasks)
//{
//    task.Start();
//}*/

///*
//Task whenAllComplete = Task.WhenAll(allTasks);
//try
//{
//    whenAllComplete.Wait();
//}
//catch (Exception) { Console.WriteLine("WhenAllComplete threw an error."); }*/

////void TestMethod(object obj, int inn)
////{
////    return;
////}

//void ThreadTask(object obj, int i)
//{
//    if (i > data.Length - 1)
//    {
//        Console.WriteLine("Index out of range");
//        return;
//    }
//    if (completedIterations.Contains(i)) return;

//    /*
//    mutex.WaitOne();
//    Console.CursorTop = i;
//    (int left, int top) cp = Console.GetCursorPosition();
//    string s = "";
//    Console.ForegroundColor = ConsoleColor.Green;
//    Console.Write(s += "   | nr: ");
//    Console.ForegroundColor = ConsoleColor.Cyan;
//    Console.WriteLine(i);
//    s += i.ToString();
//    cp.left = s.Length;
//    mutex.ReleaseMutex();

//    Stopwatch sw2 = Stopwatch.StartNew();
//    int result = springs.springRows[i].Calculate(springs.springRows[i].SpringsAsString(), [.. springs.springRows[i].continuousGroups]);
//    sw2.Stop();
   
//    mutex.WaitOne();
//    Console.SetCursorPosition(cp.left, cp.top);

//    Console.ForegroundColor = ConsoleColor.Green;
//    Console.Write(new string(' ', 20 - s.Length) + " Result: ");
//    s += new string(' ', 30 - i.ToString().Length) + " Result: ";
//    Console.ForegroundColor = ConsoleColor.Cyan;
//    Console.Write(result.ToString());
//    s += result.ToString();

//    Console.ForegroundColor = ConsoleColor.Green;
//    Console.Write(new string(' ', 60 - s.Length) + " Elapsed: ");
//    Console.ForegroundColor = ConsoleColor.Cyan;
//    Console.Write(sw2.ElapsedMilliseconds.ToString());
//    Console.ForegroundColor = ConsoleColor.Green;
//    Console.WriteLine("ms (thread nr " + Thread.CurrentThread.ManagedThreadId + ")");
//    mutex.ReleaseMutex();

//    sw2.Reset();*/

//    int result = springs.springRows[i].Calculate(springs.springRows[i].SpringsAsString(), [.. springs.springRows[i].continuousGroups]);
//    totalNumberOfWays += result;

//    completedIterations.Add(i);
//    Console.WriteLine("Completed nr " + i + new string(' ', 7 - i.ToString().Length) + "(" + (completedIterations.Count - 1) + "/" + (data.Length - 1) + ")");
//}

///*foreach (var item in data)
//{
//    Stopwatch sw2 = Stopwatch.StartNew();
//    SpringRow row = new SpringRow(item);
//    int nrOfWays = row.ResolveUnknownSprings();
//    totalNumberOfWays += nrOfWays;
//    Console.ForegroundColor = ConsoleColor.Yellow;
//    Console.Write(" > " + row.ToString());
//    Console.ForegroundColor = ConsoleColor.Cyan;
//    Console.WriteLine("   nrOfWays: " + nrOfWays + "   Took " + sw2.ElapsedMilliseconds + "ms");
//    sw2.Stop();
//}*/


//Console.ForegroundColor = ConsoleColor.Green;
//Console.WriteLine("\n >>> Total nr of ways: " + totalNumberOfWays + "\nTook time " + sw1.ElapsedMilliseconds + "ms\n\n");
//Console.ForegroundColor = ConsoleColor.White;
////SpringRow row = new SpringRow(data[0]);