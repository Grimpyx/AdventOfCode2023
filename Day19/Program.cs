


// Interpret data
//List<Part> parts = InterpretDataP1(File.ReadAllLines("./data_test.txt"));
List<Part> parts = InterpretDataP1(File.ReadAllLines("./data_complete.txt"));
// 4876XX correct

int result = 0;
for (int parti = 0; parti < parts.Count; parti++)
{
    Part p = parts[parti];

    while (!p.IsComplete)
    {
        string nextWf = p.EvaluateCurrentWorkflow();
        p.SetNextWorkflow(nextWf);
    }

    int add = 0;
    if (p.CompletionStatus == 'A') add = p.X + p.M + p.A + p.S;

    Console.WriteLine("x=" + p.X + "\t" + p.CompletionStatus + " " + add);
    result += add;
}
Console.WriteLine(" > Total: " + result);


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

                if (containsLessThan && partValue < value) return destination;
                else if (containsGreaterThan && partValue > value) return destination;
            }
            else return step;
        }
        return steps[^1];
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
    private int x; // Exremely coollooking
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