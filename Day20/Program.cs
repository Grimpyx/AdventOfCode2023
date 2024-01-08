using Day20;
using System.Collections.Generic;
using System.Diagnostics;

//List<Module> modules = InterpretData(File.ReadAllLines("./data_test.txt"));
List<Module> modules = InterpretData(File.ReadAllLines("./data_complete.txt"));

// Part 1 7307975XX correct
// Part 2 2267320771523XX correct

Part2();

void Part2()
{
    Stopwatch sw = Stopwatch.StartNew();
    bool writeOutput = true;
    ButtonModule button = new ButtonModule("button", ["broadcaster"]);

    /// Part 2:
    /// Seems like the only connection to rx is &gh.
    /// To send ONE -low signal to rx, meaning all remembered connections to gh must be HIGH.
    /// &zf || &qx || &cd || &rk -> &gh
    ///     &jj -> &rk
    ///         %mj || %qn || %xr || %gx || %zk || %qv || %vj -> &jj
    ///     &gf -> &cd
    ///     &bz -> &qx
    ///     &xz -> &zf
    ///     on and on and on

    /// What we can do is look how many times we have to press the button to
    /// change the value of each connection. In this case, we need zf, qx, cd, rk all to be positive,
    /// so we find how many loops we need to set each to high, then get the least common multiple
    /// between them.
    Module start = Module.moduleDict["rx"];
    List<Module> allModules = [.. Module.moduleDict.Values];
    Module writerToRX = start.connectedModules.First(); // We know there will only be one connection to RX (namely gh), hence First()
    List<Module> modulesThatNeedToShowTRUE = writerToRX.connectedModules; // For each connection to gh, they need to send a HIGH pulse simultaneously
    Dictionary<Module, int> timesPressedButtonForEachModule = new Dictionary<Module, int>();

    int timesWePressedButton = 0;
    while (modulesThatNeedToShowTRUE.Count > 0)
    {
        foreach (var m in modulesThatNeedToShowTRUE.ToArray())
        {
            // If the module had sent High this button press.
            // We can do this because we know "rx" only has one connection, gh, and it is a ConjunctionModule.
            // All connections to &gh need to send a HIGH pulse.
            if (m.SentHighCount > 0)
            {
                timesPressedButtonForEachModule.Add(m, timesWePressedButton);
                modulesThatNeedToShowTRUE.Remove(m);
            }
        }

        timesWePressedButton++;
        button.PushButton(); // Push button
        while (!AdvanceQueues(false)) ; // Step through until the end
    }
    string s = "";
    foreach (var item in timesPressedButtonForEachModule)
    {
        if (s != "") s += ", ";
        s += $"{item.Key}={item.Value}";
    }

    long lcm = LeastCommonMultiple(timesPressedButtonForEachModule.Values.Select(x => (long)x).ToArray());

    sw.Stop();
    Console.WriteLine("The values below indicate how after how many loops we each connected module receives a HIGH pulse.");
    Console.WriteLine("When all show high, rx will show low. We can calculate this with the least common multiple.");
    Console.WriteLine(" > Values: " + s);
    Console.WriteLine(" > LCM:    " + lcm);
    Console.WriteLine("\nDone! Took " + sw.ElapsedMilliseconds + "ms");
}


void Part1()
{
    Stopwatch sw = Stopwatch.StartNew();
    bool writeOutput = true;

    ButtonModule button = new ButtonModule("button", ["broadcaster"]);
    int buttonPresses = 1000;
    for (int numberOfButtonPresses = 0; numberOfButtonPresses < buttonPresses; numberOfButtonPresses++)
    {
        if (writeOutput) Console.WriteLine($"\nPressed button (step {numberOfButtonPresses + 1})");
        button.PushButton();

        while (!AdvanceQueues(writeOutput))
        {
            //Thread.Sleep(100);
            //Console.WriteLine();
        }
    }
    sw.Stop();
    Console.WriteLine(" > Total high: " + Module.totalHighPulses);
    Console.WriteLine(" > Total  low: " + Module.totalLowPulses);
    Console.WriteLine(" > Multiplied: " + (Module.totalLowPulses * Module.totalHighPulses));

    Console.WriteLine("\nDone! Took " + sw.ElapsedMilliseconds + "ms");
}

bool AdvanceQueues(bool writeResult) // returns True if complete
{
    //modules.ForEach(module => module.Step());
    bool noUpdate = true;
    List<Module> modulesToStep = new List<Module>();
    foreach (var m in Module.moduleDict.Values)
    {
        if (m.receiveQueue.Count > 0)
        {
            modulesToStep.Add(m);
            noUpdate &= false;
        }
    }
    modulesToStep.ForEach(module => module.Step(writeResult));
    if (writeResult) Console.WriteLine();
    return noUpdate;
}


// Interprets the data by creating modules from each line.
List<Module> InterpretData(string[] ReadAllLines)
{
    List<Module> list = new List<Module>();
    List<ConjunctionModule> conjunctionModules = new List<ConjunctionModule>(); // this exists to use later for a specific initialization

    for (int i = 0; i < ReadAllLines.Length; i++)
    {
        string line = ReadAllLines[i];

        string leftS = line.Split(" -> ")[0];
        string rightS = line.Split(" -> ")[1];

        string name = "";
        List<string> destinations = [..rightS.Split(", ")];

        switch (leftS[0])
        {
            case '%':
                name = leftS[1..];
                list.Add(new FlipFlopModule(name, destinations));
                break;
            case '&':
                name = leftS[1..];
                ConjunctionModule cm = new ConjunctionModule(name, destinations);
                conjunctionModules.Add(cm);
                list.Add(cm);
                break;
            default: // broadcaster
                name = leftS;
                list.Add(new BroadcastModule(name, destinations));
                break;
        }
    }


    // In part 1, used to initialize remembered pulses for all conjunction modules.
    // This needs to be done after all modules are created to avoid null references.
    // In part 2 we
    /*foreach (ConjunctionModule item in conjunctionModules)
    {
        item.InitializeConnections();
    }*/
    foreach (Module item in list)
    {
        foreach (var s in item.destinationModules)
        {
            if (!Module.moduleDict.ContainsKey(s))
            {
                Module.moduleDict.Add(s, new OutputModule(s, []));
            }
        }
        item.InitializeConnections();
    }
    foreach (ConjunctionModule item in conjunctionModules)
    {
        item.InitializeConnections();
    }
    List<OutputModule> oms = Module.moduleDict.Values.OfType<OutputModule>().ToList();

    return list;
}

// There is a faster method of calculating the least common multiple, by dividing the product of the list of values with the greatest common divisor
// One way to calc the GCD: https://en.wikipedia.org/wiki/Euclidean_algorithm#Worked_example <-- see the worked example
// Apparently, assuming X>Y, the GCD of X and Y is the same as for X and X-Y.
// Essentially you can recursively make X and Y smaller and smaller until you get a remainder of 0, meaning you found the least common divisor.
// If you have many values, the least common divisor for all values can be calculated doing  GCD(A,B ... Z) = GCD(A,GCD(B,GCD(C,D))) and on and on and on until you hit Z
// Below is my implementation of that.
static long GreatestCommonDenominator(long[] list)
{
    // can't find GCD for a list of length 1
    if (list.Length < 2) return -1;

    // If we have more than 2 values, we have to recursively call this until it is only 2 values.
    if (list.Length > 2)
    {
        long gcdForTheFirstTwo = GreatestCommonDenominator(list[1..]);
        list = [list[0], gcdForTheFirstTwo];
    }

    // If we have only 2 values we can proceed
    if (list.Length == 2)
    {
        long greater = Math.Max(list[0], list[1]);
        long lesser = Math.Min(list[0], list[1]);

        while (true)
        {
            //Console.WriteLine("OLD - Greater: " + greater + "   Lesser: " + lesser);
            long remainder = greater;

            // instead of doing -lesser over and over again we can just use modulus to get the remainder
            remainder = greater % lesser;
            if (remainder == 0) return lesser; // if true, it is the GCD

            // When out of that loop, assign new Greater and Lesser
            greater = lesser;
            lesser = remainder;
        }
    }

    Console.WriteLine("How did you get here?");
    return -1;
}
static long LeastCommonMultiple(long[] list)
{
    // can't find LCM for a list of length 1
    if (list.Length < 2) return -1;

    // If we have more than 2 values, we have to recursively call this until it is only 2 values.
    if (list.Length > 2)
    {
        long lcmForTheFirstTwo = LeastCommonMultiple(list[1..]);
        list = [list[0], lcmForTheFirstTwo];
    }

    // If we have only 2 values we can proceed
    if (list.Length == 2)
    {
        // We follow the definition of lcm(A,B) = (A*B)/gcd(A,B) from https://en.wikipedia.org/wiki/Least_common_multiple#Using_the_greatest_common_divisor
        ulong product = 1;
        foreach (long item in list) product *= (ulong)item;
        ulong gcd = (ulong)GreatestCommonDenominator(list);
        return (long)(product / gcd);
    }

    Console.WriteLine("How did you get here?");
    return -1;
}