using Day20;
using System.Collections.Generic;
using System.Diagnostics;

//List<Module> modules = InterpretData(File.ReadAllLines("./data_test.txt"));
List<Module> modules = InterpretData(File.ReadAllLines("./data_complete.txt"));

// Part 1 7307975XX correct


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

    // Initialize remembered pulses for all conjunction modules.
    // This needs to be done after all modules are created to avoid null references.
    foreach (ConjunctionModule item in conjunctionModules)
    {
        item.InitializeRememberedPulses();
    }

    return list;
}