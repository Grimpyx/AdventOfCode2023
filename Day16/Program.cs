
using Day10;
using Day14;
using System.Linq;

// 5606 is too low
// 76xx was correct for part 1
// 82xx was correct for part 2

List<Beam> beamTracker;

// Data interpret
string[] data = File.ReadAllLines("./data_complete.txt");
//string[] data = File.ReadAllLines("./data_complete.txt");
char[,] chars = new char[data[0].Length, data.Length];
bool[,] energized = new bool[chars.GetLength(0), chars.GetLength(1)];
for (int y = 0; y < data.Length; y++)
{
    for (int x = 0; x < data[0].Length; x++)
    {
        chars[x, y] = data[y][x];
        energized[x, y] = false;
    }
}
Map2D<char> map = new Map2D<char>(chars);

Dictionary<(Vector2, Vector2), bool> symbolsAlreadyInteractedWith = new Dictionary<(Vector2, Vector2), bool>();
int stepsSinceLastValidUpdate = -1;
Part2();

void Part1()
{
    Beam startBeam;
    startBeam = new Beam(Vector2.right, [-Vector2.right]);
    beamTracker = [startBeam];

    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine(map.ToString());

    while (!Step()) { }

    Console.ForegroundColor= ConsoleColor.White;

    // Calculate energized
    int totalEnergized = 0;
    for (int y = 0; y < energized.GetLength(1); y++)
    {
        for (int x = 0; x < energized.GetLength(0); x++)
        {
            if (energized[x, y] == true) totalEnergized++;
        }
    }
    Console.SetCursorPosition(0, chars.GetLength(1) + 1);
    Console.Write("Total energized: ");
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine(totalEnergized);
    Console.ForegroundColor = ConsoleColor.White;
}

void Part2()
{

    Beam baseBeam = new Beam(Vector2.up, [Vector2.down]);
    List<Beam> allStartingBeams = new List<Beam>();
    beamTracker = [];


    allStartingBeams.AddRange(Enumerable.Range(0, chars.GetLength(0)).Select(x => new Beam(baseBeam.direction, [new Vector2(x, -1)])).ToList());
    baseBeam.direction = Vector2.left;
    allStartingBeams.AddRange(Enumerable.Range(0, chars.GetLength(1)).Select(x => new Beam(baseBeam.direction, [new Vector2(chars.GetLength(0), x)])).ToList());
    baseBeam.direction = Vector2.down;
    allStartingBeams.AddRange(Enumerable.Range(0, chars.GetLength(0)).Select(x => new Beam(baseBeam.direction, [new Vector2(chars.GetLength(0) - x - 1, chars.GetLength(1))])).ToList());
    baseBeam.direction = Vector2.right;
    allStartingBeams.AddRange(Enumerable.Range(0, chars.GetLength(1)).Select(x => new Beam(baseBeam.direction, [new Vector2(-1, chars.GetLength(1) - x - 1)])).ToList());



    List<int> nrOfEnergized = new List<int>();

    foreach (Beam startBeam in allStartingBeams)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(0, 0);
        Console.WriteLine(map.ToString());

        // Clear
        energized = new bool[chars.GetLength(0), chars.GetLength(1)];
        symbolsAlreadyInteractedWith.Clear();
        beamTracker.Clear();

        beamTracker = [startBeam];
        while (!Step()) { }

        // Calculate energized
        int totalEnergized = 0;
        for (int y = 0; y < energized.GetLength(1); y++)
        {
            for (int x = 0; x < energized.GetLength(0); x++)
            {
                if (energized[x, y] == true) totalEnergized++;
            }
        }
        nrOfEnergized.Add(totalEnergized);

    }
    Console.SetCursorPosition(0, chars.GetLength(1) + 1);
    Console.WriteLine(" > Highest: " + nrOfEnergized.Max());
}
bool Step()
{
    stepsSinceLastValidUpdate++;

    bool allHasReachedEnd = true;
    List<Beam> newBeams = new List<Beam>();
    List<Beam> beamsMarkedForRemoval = new List<Beam>();

    Console.ForegroundColor = ConsoleColor.Green;
    foreach (Beam beam in beamTracker.ToArray())
    {
        if (beam.reachedEnd)
            continue;
        allHasReachedEnd = false;


        // Advance all beams once
        Vector2 nextPos = beam.straightLines[^1] + beam.direction;

        // Coordinate within bounds
        if (nextPos.x < 0 || nextPos.y < 0 || nextPos.x > map.ColumnLength - 1 || nextPos.y > map.RowLength - 1)
        {
            beam.reachedEnd = true;
            beamTracker.Remove(beam);
            //beamsMarkedForRemoval.Add(beam);
            continue;
        }
        char symbol = map.GetValue((int)nextPos.x, (int)nextPos.y);

        // Check if we've already been here
        if (symbolsAlreadyInteractedWith.ContainsKey((beam.direction, nextPos))) continue;
        symbolsAlreadyInteractedWith.Add((beam.direction, nextPos), true);

        if (beam.CheckConsecutive() > 2)
        {
            beam.reachedEnd = true;
            beamTracker.Remove(beam);
            //beamsMarkedForRemoval.Add(beam);
            continue;
        }
        if (energized[(int)nextPos.x, (int)nextPos.y] != true)
        {
            Console.SetCursorPosition((int)nextPos.x, (int)nextPos.y);
            Console.Write('X');
            stepsSinceLastValidUpdate = -1;
            energized[(int)nextPos.x, (int)nextPos.y] = true;
        }
        beam.straightLines[^1] = nextPos;

        // Check for next
        Vector2[] newDirections = InterpretSymbol(beam.direction, symbol);
        if (newDirections == null || newDirections[0] == Vector2.none)
            return false;

        // If length longer than 1, we split into two directions.
        // We create duplicate of the beam and update direction.
        if (newDirections.Length > 1)
        {
            //beam.straightLines[^1] = newPaths[0]; //Add(newPaths[0] + beam.straightLines[^1]);
            beam.direction = newDirections[0];

            for (int i = 1; i < newDirections.Length; i++)
            {
                Beam copiedBeam = new Beam(beam);
                copiedBeam.straightLines.Add(copiedBeam.straightLines[^1]);
                copiedBeam.direction = newDirections[i];
                newBeams.Add(copiedBeam);
            }
        }
        else
            beam.direction = newDirections[0];


    }
    beamTracker.AddRange(newBeams);
    /*foreach (Beam b in beamsMarkedForRemoval)
    {
        beamTracker.Remove(b);
    }*/

    if (stepsSinceLastValidUpdate > 100) allHasReachedEnd = true;

    return allHasReachedEnd;
}

Vector2[] InterpretSymbol(Vector2 entryDirection, char symbol)
{
	switch (symbol)
    {
        case '.':
            return [entryDirection];
        case '/':
            switch ((entryDirection.x, entryDirection.y))
            {
                case (0, 1):
                    return [Vector2.left];
                case (0, -1):
                    return [Vector2.right];
                case (1, 0):
                    return [Vector2.down];
                case (-1, 0):
                    return [Vector2.up];
                default:
                    break;
            }
            return [Vector2.none];
        case '\\':
            switch ((entryDirection.x, entryDirection.y))
            {
                case (0, 1):
                    return [Vector2.right];
                case (0, -1):
                    return [Vector2.left];
                case (1, 0):
                    return [Vector2.up];
                case (-1, 0):
                    return [Vector2.down];
                default:
                    break;
            }
            return [Vector2.none];
        case '|':
            switch ((entryDirection.x, entryDirection.y))
            {
                case (0, 1):
                    return [entryDirection];
                case (0, -1):
                    return [entryDirection];
                case (1, 0):
                    return [Vector2.up, Vector2.down];
                case (-1, 0):
                    return [Vector2.up, Vector2.down];
                default:
                    break;
            }
            return [Vector2.none];
        case '-':
            switch ((entryDirection.x, entryDirection.y))
            {
                case (0, 1):
                    return [Vector2.left, Vector2.right];
                case (0, -1):
                    return [Vector2.left, Vector2.right];
                case (1, 0):
                    return [entryDirection];
                case (-1, 0):
                    return [entryDirection];
                default:
                    break;
            }
            return [Vector2.none];
        default:
            return [Vector2.none];
	}
}

class Beam
{
    public bool reachedEnd = false;
    public Vector2 direction;
    public List<Vector2> straightLines;

    public Beam(Vector2 direction, List<Vector2> straightLines)
    {
        this.direction = direction;
        this.straightLines = straightLines;
    }

    public Beam(Beam otherBeam)
    {
        straightLines = new List<Vector2>(otherBeam.straightLines);
        direction = otherBeam.direction;
    }

    public int CheckConsecutive()
    {
        int consecutiveRequired = 5;

        if (straightLines.Count <= 2 * consecutiveRequired) return 0;
        List<Vector2> tenMostRecent = straightLines.Slice(straightLines.Count - 1 - consecutiveRequired, consecutiveRequired);

        int totalExactCopies = 0;
        int counter = 0;

        for (int i = 0; i < straightLines.Count - consecutiveRequired; i++)
        {
            for (int j = 0; j < consecutiveRequired; j++)
            {
                if (straightLines[i + j] == tenMostRecent[j]) counter++;
            }
            if (counter >= consecutiveRequired) totalExactCopies++;
            counter = 0;
        }
        return totalExactCopies;
    }

    public override bool Equals(object? obj)
    {
        return obj is Beam beam &&
               direction.Equals(beam.direction) &&
               EqualityComparer<List<Vector2>>.Default.Equals(straightLines, beam.straightLines);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(direction, straightLines);
    }
}