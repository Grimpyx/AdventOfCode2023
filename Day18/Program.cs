using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

// p1 486XX
// p2 952408144XXX

Part1(useCompleteData: true);
Part2(useCompleteData: true);

void Part2(bool useCompleteData)
{
    Console.WriteLine("Part 2:");

    (int dx, int dy)[] instructions;
    if (useCompleteData) instructions = InterpretInputP2(File.ReadAllLines("./data_test.txt"));
    else instructions = InterpretInputP2(File.ReadAllLines("./data_complete.txt"));

    // We will use better math to calculate the area of this new polygon, which happens to be huge, rendering my other implementation (although genious!) unusable.
    // Math is called Shoelace formula, or Gauss' area formula
    var polygons = CoordsFromInstructions(instructions);

    // But when we calculate the area, we only get the inside, not including the perimeter.
    // The perimeter is all instruction lengths summed.
    long perimeter = 0;
    for (int i = 0; i < instructions.Length; i++)
    {
        perimeter += Math.Abs(instructions[i].dx + instructions[i].dy);
    }
    long area = AreaFromCoords(polygons, perimeter);

    Console.WriteLine();
}

void Part1(bool useCompleteData)
{
    Console.WriteLine("Part 1:");

    (int dx, int dy)[] instructions;
    if (useCompleteData) instructions = InterpretInputP1(File.ReadAllLines("./data_test.txt"));
    else instructions = InterpretInputP1(File.ReadAllLines("./data_complete.txt"));

    ((int[] x, int[] y) path_coords, PathState[][] map) = DrawPath(instructions);
    AreaState[][] area = FindArea(map);

    ExportAreaImage(area, "./Part1_area.png");

    int outsideArea = 0;
    foreach (var row in area)
    {
        outsideArea += row.Where(x => x == AreaState.Outside).Count();
    }

    Console.WriteLine(" > Total  inside: " + ((area.Length * area[0].Length) - outsideArea));
    Console.WriteLine(" > Total outside: " + outsideArea);
    Console.WriteLine();
}

long AreaFromCoords((int x, int y)[] coords, long perimeterLength)
{
    //List<long> tX = new List<long>();
    //List<long> tY = new List<long>();
    long area = 0;
    for (int i = 0; i < coords.Length - 1; i++)
    {
        long a, b, c, d;
        a = coords[i].x;
        b = coords[i + 1].y;
        c = coords[i].y;
        d = coords[i + 1].x;
        long add = a * b - c * d; // this gives the correct answer
        //long add = (coords[i].x * coords[i + 1].y) - (coords[i].y * coords[i + 1].x); // This gave wrong answer for some reason, no idea why

        area += add;
    }
    area = Math.Abs(area);

    Console.WriteLine(" >        twoArea: " + area);
    area /= 2;
    Console.WriteLine(" >           area: " + area);
    area += perimeterLength / 2 + 1;
    Console.WriteLine(" > corrected area: " + area);

    return area;
}

(int x, int y)[] CoordsFromInstructions((int dx, int dy)[] instructions)
{
    List<(int x, int y)> coords = new List<(int x, int y)>() { (0,0) };
    for (int i = 0; i < instructions.Length; i++)
    {
        // The next coordinate is just the previous one plus the step from the instruction
        var last = coords.Last();
        coords.Add((last.x + instructions[i].dx, last.y + instructions[i].dy));
    }
    if (coords[0] != coords[^1]) coords.Add(coords[0]);

    return coords.ToArray();
}


void ExportAreaImage(AreaState[][] map, string fileName)
{
    int width, height;
    width = map[0].Length;
    height = map.Length;

    Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);

    for (int i = 0; i < height; i++)
    {
        for (int j = 0; j < width; j++)
        {
            AreaState s = map[i][j];
            Color c = Color.White;
            switch (s)
            {
                case AreaState.Outside:
                    c = Color.DarkGray;
                    break;
                case AreaState.Path:
                    c = Color.LawnGreen;
                    break;
                case AreaState.Inside:
                    c = Color.ForestGreen;
                    break;
            }
            bitmap.SetPixel(j, i, c);
        }
    }
    bitmap.Save(fileName, ImageFormat.Png);

}

AreaState[][] FindArea(PathState[][] map)
{
    bool draw = false;

    AreaState[][] areaMap = new AreaState[map.Length][];

    (int x, int y) startConsolePos = Console.GetCursorPosition();

    // If we draw a horizontal line, depending on how many pieces of the
    // path we cross we can determine what is inside and what is outside.
    for (int i = 0; i < map.Length; i++)
    {
        PathState[] row = map[i];
        AreaState[] areaRow = new AreaState[row.Length];

        bool outside = true;
        for (int j = 0; j < row.Length; j++)
        {
            if (row[j] == PathState.Empty)
            {
                if (outside)
                {
                    areaRow[j] = AreaState.Outside;
                }
                else
                {
                    areaRow[j] = AreaState.Inside;
                }
            }
            else // count consecutive path elements
            {

                // Skip until next
                int consecutivePath = 0;
                while (row[j + consecutivePath] == PathState.HasPath)
                {
                    areaRow[j + consecutivePath] = AreaState.Path;

                    consecutivePath++;
                    if (j + consecutivePath >= row.Length) // if out of bounds
                    {
                        consecutivePath = -1;
                        break;
                    }
                }
                if (consecutivePath == -1) continue; // reached end of row

                j += consecutivePath;
                if (consecutivePath > 1) // found horizontal line
                {
                    if (i - 1 >= 0)
                    {
                        areaRow[j] = areaMap[i - 1][j];
                        outside = areaMap[i - 1][j] == AreaState.Outside ? true : false;
                    }
                }
                else if (consecutivePath == 1)
                {
                    j--;
                    outside = !outside;
                }

            }
        }

        areaMap[i] = areaRow;
    }

    // Draw
    if (draw)
    {
        for (int i = 0; i < areaMap.Length; i++)
        {
            for (int j = 0; j < areaMap[0].Length; j++)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                switch (areaMap[i][j])
                {
                    case AreaState.Outside:
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        break;
                    case AreaState.Path:
                        Console.BackgroundColor = ConsoleColor.Gray;
                        break;
                    case AreaState.Inside:
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        break;
                    default:
                        break;
                }

                Console.SetCursorPosition(startConsolePos.x + j, startConsolePos.y + i);
                Console.Write(' ');
            }
        }
        

        // Restore color
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Black;
    }

    return areaMap;
}

// From a set of instructions
((int[] x, int[] y), PathState[][] map) DrawPath((int dx, int dy)[] instructions)
{
    bool draw = false;

    int x = 0;
    int y = 0;
    int highestX = int.MinValue;
    int highestY = int.MinValue;
    int lowestX = int.MaxValue;
    int lowestY = int.MaxValue;

    List<int> all_x = new List<int>() { 0 };
    List<int> all_y = new List<int>() { 0 };

    (int x, int y) startConsolePos = Console.GetCursorPosition();

    // Loop through instructions and create the map
    for (int i = 0; i < instructions.Length; i++)
    {
        int dirX = Math.Sign(instructions[i].dx);
        int dirY = Math.Sign(instructions[i].dy);

        // Step in x
        for (int j = 0; j < Math.Abs(instructions[i].dx); j++)
        {
            x += dirX;
            if (x > highestX) highestX = x;
            if (x < lowestX) lowestX = x;

            all_x.Add(x);
            all_y.Add(y);
        }
        // Step in y
        for (int j = 0; j < Math.Abs(instructions[i].dy); j++)
        {
            y += dirY;
            if (y > highestY) highestY = y;
            if (y < lowestY) lowestY = y;

            all_x.Add(x);
            all_y.Add(y);
        }
    }

    if (draw) Console.SetBufferSize(highestX - lowestX + 200, highestY - lowestY + 200);

    // Create map with default values State.Empty
    PathState[][] map = new PathState[highestY - lowestY + 1][];
    for (int i = 0; i < map.Length; i++) // for each row
    {
        map[i] = Enumerable.Repeat(PathState.Empty, highestX - lowestX + 1).ToArray();
    }

    // Correct to positive coordinates (for example (-5,9) and (-3,4) becomes (0,9) and (2,4)) and then draw it
    for (int i = 0; i < all_x.Count; i++)
    {
        all_x[i] -= lowestX;
    }
    for (int i = 0; i < all_y.Count; i++)
    {
        all_y[i] -= lowestY;

        map[all_y[i]][all_x[i]] = PathState.HasPath;
    }

    // Drawing the map in the console
    if (draw)
    {
        Console.ForegroundColor = ConsoleColor.Black;
        for (int i = 0; i < map.Length; i++)
        {
            for (int j = 0; j < map[0].Length; j++)
            {
                if (map[i][j] == PathState.HasPath) Console.BackgroundColor = ConsoleColor.Gray;
                else Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.SetCursorPosition(startConsolePos.x + j, startConsolePos.y + i);
                Console.Write(' ');
            }
        }
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;

        Console.SetCursorPosition(startConsolePos.x, startConsolePos.y + highestY + 1);
    }

    return ((all_x.ToArray(), all_y.ToArray()), map);
}


// In part 2 we interpret the data differently
(int dx, int dy)[] InterpretInputP2(string[] readAllLines)
{
    List<(int dx, int dy)> instructions = new List<(int dx, int dy)>();

    foreach (string line in readAllLines)
    {
        string[] split = line.Split(' ');

        string hex = split[2][2..^1];

        // First char determines direction
        char directionChar = hex[^1];
        int dx = directionChar switch
        {
            '0' => 1,
            '2' => -1,
            _ => 0
        };
        int dy = directionChar switch
        {
            '3' => -1,
            '1' => 1,
            _ => 0
        };


        // The rest of the hex data indicates the steps
        hex = hex[..^1];
        int steps = Convert.ToInt32(hex, 16);
        dx *= steps;
        dy *= steps;

        instructions.Add((dx, dy));
    }
    return instructions.ToArray();
}

(int dx, int dy)[] InterpretInputP1(string[] readAllLines)
{
    List<(int dx, int dy)> values = new List<(int dx, int dy)>();

    foreach (string line in readAllLines)
    {
        string[] split = line.Split(' ');

        // First in split represents direction
        int dx = split[0] switch
        {
            "R" => 1,
            "L" => -1,
            _ => 0
        };
        int dy = split[0] switch
        {
            "U" => -1,
            "D" => 1,
            _ => 0
        };

        // Second split represents the number of steps
        int steps = int.Parse(split[1]);
        dx *= steps;
        dy *= steps;

        string color = split[2][1..^1];

        values.Add((dx, dy));
    }
    return values.ToArray();
}



Direction L90(Direction direction) => direction switch
{
    Direction.N => Direction.W,
    Direction.W => Direction.S,
    Direction.S => Direction.E,
    Direction.E => Direction.N,
    _ => throw new UnreachableException()
};
Direction R90(Direction direction) => direction switch
{
    Direction.N => Direction.E,
    Direction.E => Direction.S,
    Direction.S => Direction.W,
    Direction.W => Direction.N,
    _ => throw new UnreachableException()
};

enum PathState
{
    Empty = 0,
    HasPath = 1
}

enum AreaState
{
    Outside= 0,
    Path = 1,
    Inside = 2
}

enum Direction
{
    N = 0,
    E = 2,
    S = 1,
    W = 3
}



