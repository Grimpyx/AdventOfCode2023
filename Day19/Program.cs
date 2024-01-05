
using System.Diagnostics;

// Interpret data
List<Part> parts = InterpretDataP1(File.ReadAllLines("./data_complete.txt"));
//List<Part> parts = InterpretDataP1(File.ReadAllLines("./data_test.txt"));

// p1 4876XX correct
// p2 11355023831XXXX correct (total possible will be 4000^4, 256 * 10^12)

Stopwatch stopwatch = Stopwatch.StartNew();
//Part1();
Part2();
stopwatch.Stop();
Console.WriteLine(" > Took " + stopwatch.ElapsedMilliseconds + "ms");

void Part2()
{
    PartRange pr = new PartRange("in",
        new Range(1, 4000),
        new Range(1, 4000),
        new Range(1, 4000),
        new Range(1, 4000));

    ulong A_numberOfCombinations = 0;
    ulong R_numberOfCombinations = 0;

    var v = pr.Evaluate();
    foreach (var item in v)
    {
        //Console.WriteLine("Completion: " + item.CompletionStatus);
        if (item.CompletionStatus == 'A')
        {
            //long combinations = (item.x.TotalCount); * (item.m.TotalCount) * (item.a.TotalCount) * (item.s.TotalCount);
            ulong combinations = (ulong)item.x.TotalCount;
            combinations *= (ulong)item.m.TotalCount;
            combinations *= (ulong)item.a.TotalCount;
            combinations *= (ulong)item.s.TotalCount;
            A_numberOfCombinations += combinations;
            //Console.WriteLine("       Add: " + combinations);
        }
        else if (item.CompletionStatus == 'R')
        {
            ulong combinations = (ulong)item.x.TotalCount;
            combinations *= (ulong)item.m.TotalCount;
            combinations *= (ulong)item.a.TotalCount;
            combinations *= (ulong)item.s.TotalCount;
            R_numberOfCombinations += combinations;
            //Console.WriteLine("       Add: " + combinations);
        }
        //Console.WriteLine("\n\n);
    }

    Console.WriteLine(" >    Total A: " + A_numberOfCombinations);
    Console.WriteLine(" >    Total R: " + R_numberOfCombinations);
    Console.WriteLine(" >    Total T: " + (A_numberOfCombinations + R_numberOfCombinations));
}

void Part1()
{
    int result = 0;
    Stopwatch stopwatch = Stopwatch.StartNew();
    for (int parti = 0; parti < parts.Count; parti++)
    {
        Part p = parts[parti];

        Stopwatch sw = Stopwatch.StartNew();
        while (!p.IsComplete)
        {
            string nextWf = p.EvaluateCurrentWorkflow();
            p.SetNextWorkflow(nextWf);
        }
        sw.Stop();

        int add = 0;
        if (p.CompletionStatus == 'A') add = p.X + p.M + p.A + p.S;

        //Console.WriteLine("x=" + p.X + "\t" + p.CompletionStatus + " " + add + " " + sw.ElapsedMilliseconds + "ms");
        result += add;
    }
    stopwatch.Stop();
    Console.WriteLine(" > Total: " + result);
    Console.WriteLine(" > Took " + stopwatch.ElapsedMilliseconds + "ms");
}


List<Part> InterpretDataP1(string[] readAllLines)
{
    Dictionary<string, Workflow> wf = new Dictionary<string, Workflow>();
    List<Part> parts = new List<Part>();

    bool inThePartsSection = false;
    for (int rowi = 0; rowi < readAllLines.Length; rowi++)
    {
        if (readAllLines[rowi].Length < 2)
        {
            inThePartsSection = true;
            continue;
        }

        string row = readAllLines[rowi];
        if (!inThePartsSection) // Workflow
        {
            string name = "";
            while (row[0] != '{')
            {
                name += row[0];
                row = row[1..];
            }
            row = row[1..^1]; // remove { }
            string[] conditions = row.Split(',');
            wf.Add(name, new Workflow(name, conditions));
        }
        else // Parts
        {
            row = row[1..^1]; // remove { }
            string[] setValues = row.Split(",");
            Part p = new Part(
                int.Parse(setValues[0][2..]),
                int.Parse(setValues[1][2..]),
                int.Parse(setValues[2][2..]),
                int.Parse(setValues[3][2..]));

            parts.Add(p);
        }
    }

    Part.workflows = wf;

    return parts;
}

class Workflow
{
    public string name;
    public string[] steps;

    public Workflow(string name, string[] steps)
    {
        this.name = name;
        this.steps = steps;
    }

    public string GetNext(Part p)
    {
        for (int i = 0; i < steps.Length - 1; i++)
        {
            string step = steps[i];
            bool containsLessThan = step.Contains('<');
            bool containsGreaterThan = step.Contains('>');

            if (containsLessThan || containsGreaterThan)
            {
                string destination = step.Split(':')[^1];
                step = step.Split(':')[0];

                int value = 0;
                if (containsLessThan) value = int.Parse(step.Split('<')[^1]);
                else if (containsGreaterThan) value = int.Parse(step.Split('>')[^1]);

                // for example   a<2006:qkq
                int partValue = step[0] switch
                {
                    'x' => p.X,
                    'm' => p.M,
                    'a' => p.A,
                    's' => p.S,
                    _ => throw new NotImplementedException()
                };

                // Do something depending on if "<" or ">"
                if (containsLessThan && partValue < value) return destination;
                else if (containsGreaterThan && partValue > value) return destination;
            }
            else return step;
        }
        return steps[^1];
    }

    public List<PartRange> EvaluatePartRange(PartRange pr)
    {
        List<PartRange> returnList = new List<PartRange>();

        for (int i = 0; i < steps.Length; i++)
        {
            string step = steps[i];
            bool containsLessThan = step.Contains('<');
            bool containsGreaterThan = step.Contains('>');

            if (containsLessThan || containsGreaterThan)
            {
                string destination = step.Split(':')[^1];
                step = step.Split(':')[0];

                int value = 0;
                if (containsLessThan) value = int.Parse(step.Split('<')[^1]);
                else if (containsGreaterThan) value = int.Parse(step.Split('>')[^1]);

                // for example   a<2006:qkq
                PartRange next = new PartRange(pr);
                switch (step[0])
                {
                    case 'x':
                        if (containsLessThan)
                        {
                            next.x.Intersect(new Range(int.MinValue, value - 1)); // Less than, not including value ->  -1
                            next.SetNextWorkflow(destination);
                            returnList.Add(next);

                            pr.x.Intersect(new Range(value, int.MaxValue));
                        }
                        else if (containsGreaterThan)
                        {

                            next.x.Intersect(new Range(value + 1, int.MaxValue)); // Greater than, not including the value ->  +1
                            next.SetNextWorkflow(destination);
                            returnList.Add(next);

                            pr.x.Intersect(new Range(int.MinValue, value));
                        }
                        break;
                    case 'm':
                        if (containsLessThan)
                        {
                            next.m.Intersect(new Range(int.MinValue, value - 1)); // Less than, not including value ->  -1
                            next.SetNextWorkflow(destination);
                            returnList.Add(next);

                            pr.m.Intersect(new Range(value, int.MaxValue));
                        }
                        else if (containsGreaterThan)
                        {

                            next.m.Intersect(new Range(value + 1, int.MaxValue)); // Greater than, not including the value ->  +1
                            next.SetNextWorkflow(destination);
                            returnList.Add(next);

                            pr.m.Intersect(new Range(int.MinValue, value));
                        }
                        break;
                    case 'a':
                        if (containsLessThan)
                        {
                            next.a.Intersect(new Range(int.MinValue, value - 1)); // Less than, not including value ->  -1
                            next.SetNextWorkflow(destination);
                            returnList.Add(next);

                            pr.a.Intersect(new Range(value, int.MaxValue));
                        }
                        else if (containsGreaterThan)
                        {

                            next.a.Intersect(new Range(value + 1, int.MaxValue)); // Greater than, not including the value ->  +1
                            next.SetNextWorkflow(destination);
                            returnList.Add(next);

                            pr.a.Intersect(new Range(int.MinValue, value));
                        }
                        break;
                    case 's':
                        if (containsLessThan)
                        {
                            next.s.Intersect(new Range(int.MinValue, value - 1)); // Less than, not including value ->  -1
                            next.SetNextWorkflow(destination);
                            returnList.Add(next);

                            pr.s.Intersect(new Range(value, int.MaxValue));
                        }
                        else if (containsGreaterThan)
                        {

                            next.s.Intersect(new Range(value + 1, int.MaxValue)); // Greater than, not including the value ->  +1
                            next.SetNextWorkflow(destination);
                            returnList.Add(next);

                            pr.s.Intersect(new Range(int.MinValue, value));
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // This Else only runs if it's the last in the workflow steps
                PartRange newRange = new PartRange(pr);
                newRange.SetNextWorkflow(steps[^1]);
                returnList.Add(newRange);
            }
        }
        return returnList;
    }


    public override bool Equals(object? obj)
    {
        return obj is Workflow workflow &&
               name == workflow.name &&
               EqualityComparer<string[]>.Default.Equals(steps, workflow.steps);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(name, steps);
    }


}

class Part
{
    private int x; // Exremely cool looking
    private int m; // Musical (it makes a noise when you hit it)
    private int a; // Aerodynamic
    private int s; // Shiny

    public static Dictionary<string, Workflow> workflows = new Dictionary<string, Workflow>();

    private string nextWorkflow = "in";
    public char CompletionStatus { get; set; }
    public bool IsComplete => CompletionStatus == 'A' || CompletionStatus == 'R';
    public int S { get => s; set => s = value; }
    public int A { get => a; set => a = value; }
    public int M { get => m; set => m = value; }
    public int X { get => x; set => x = value; }
    public string NextWorkflow { get => nextWorkflow; }

    public Part(int x, int m, int a, int s)
    {
        this.X = x;
        this.M = m;
        this.A = a;
        this.S = s;
    }

    public string EvaluateCurrentWorkflow()
    {
        if(workflows.TryGetValue(NextWorkflow, out Workflow wf))
        {
            return wf.GetNext(this);
        }
        return "Failed.";
    }


    public void SetNextWorkflow(string value)
    {
        if (value == "A")
        {
            CompletionStatus = 'A';
            nextWorkflow = "None";
            return;
        }

        if (value == "R")
        {
            CompletionStatus = 'R';
            nextWorkflow = "None";
            return;
        }

        nextWorkflow = value;
    }

    public override bool Equals(object? obj)
    {
        return obj is Part part &&
               X == part.X &&
               M == part.M &&
               A == part.A &&
               S == part.S;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, M, A, S);
    }
}

class PartRange
{
    private string nextWorkflow;
    public string NextWorkflow { get => nextWorkflow; }
    public char CompletionStatus { get; set; }
    public bool IsComplete => CompletionStatus == 'A' || CompletionStatus == 'R';

    public void SetNextWorkflow(string value)
    {
        if (value == "A")
        {
            CompletionStatus = 'A';
            nextWorkflow = "None";
            return;
        }

        if (value == "R")
        {
            CompletionStatus = 'R';
            nextWorkflow = "None";
            return;
        }

        nextWorkflow = value;
    }

    public Range x;
    public Range m;
    public Range a;
    public Range s;

    public PartRange(string instruction, Range x, Range m, Range a, Range s)
    {
        this.nextWorkflow = instruction;
        this.x = x;
        this.m = m;
        this.a = a;
        this.s = s;
    }

    public PartRange(PartRange copyOf)
    {
        nextWorkflow = copyOf.nextWorkflow;

        x = new Range(copyOf.x);
        m = new Range(copyOf.m);
        a = new Range(copyOf.a);
        s = new Range(copyOf.s);
    }

    public static Dictionary<string, Workflow> workflows => Part.workflows;

    public List<PartRange>? Evaluate()
    {
        if (workflows.TryGetValue(NextWorkflow, out Workflow wf))
        {
            // We utilize a PriorityQueue which lets us Queue items and then Dequeue them for an easy workflow.
            // This is a good option when you otherwise would loop through a List<> as if you could change it during runtime.
            PriorityQueue<PartRange, int> priorityQueue = new PriorityQueue<PartRange, int>();
            priorityQueue.Enqueue(this, 0); // Add This PartRange as the first entry

            List<PartRange> completeParts = new List<PartRange>();

            while (priorityQueue.Count > 0)
            {
                // Dequeue item.
                // This removes it from the queue, and we access it through the variable "pr"
                var pr = priorityQueue.Dequeue();

                if (pr.IsComplete) completeParts.Add(pr);
                else // If partrange not complete we "step" by evaluating it
                {
                    // Evaluate the range for its current
                    var nextList = pr.EvaluateCurrentWorkflow();
                    foreach (var item in nextList)
                    {
                        // Queue the new PartRanges
                        priorityQueue.Enqueue(item, 0);
                    }
                }
            }
            return completeParts;
        }
        return null;
    }

    // From the list of all workflows, find the one we're currently on and run its steps
    public List<PartRange>? EvaluateCurrentWorkflow()
    {
        if (workflows.TryGetValue(NextWorkflow, out Workflow wf))
        {
            return wf.EvaluatePartRange(this);
        }
        return null;
    }

    public override bool Equals(object? obj)
    {
        return obj is PartRange range &&
               nextWorkflow == range.nextWorkflow &&
               EqualityComparer<Range>.Default.Equals(x, range.x) &&
               EqualityComparer<Range>.Default.Equals(m, range.m) &&
               EqualityComparer<Range>.Default.Equals(a, range.a) &&
               EqualityComparer<Range>.Default.Equals(s, range.s);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(nextWorkflow, x, m, a, s);
    }
}

class Range
{
    public int lower;
    public int upper;
    public bool BothIsZero { get; private set; }
    public int TotalCount => (upper + 1) - lower;

    public Range(int lower, int upper)
    {
        this.lower = lower;
        this.upper = upper;

        if (lower == 0 && upper == 0) BothIsZero = true;
        else BothIsZero = false;
    }

    // Creates a copy of another range
    public Range(Range copyOf)
    {
        this.lower = copyOf.lower;
        this.upper = copyOf.upper;

        if (lower == 0 && upper == 0) BothIsZero = true;
        else BothIsZero = false;
    }


    public void Intersect(Range other)
    {
        int newLower = Math.Max(other.lower, lower);
        int newUpper = Math.Min(other.upper, upper);

        lower = newLower;
        upper = newUpper;
    }

    public override bool Equals(object? obj)
    {
        return obj is Range range &&
               lower == range.lower &&
               upper == range.upper;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(lower, upper);
    }

    public override string? ToString()
    {
        return $"({lower}, {upper})";
    }
}


// Records?