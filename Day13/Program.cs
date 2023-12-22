
// Interpret data
using Day13;

// 14357 too low
// 33536 too low
// 30282 too low
// 34536 is the correct answer. Thanks to Ryan heath https://github.com/ryanheath/aoc2023/blob/master/Day13.cs

List<MirrorMap> mirrorMaps = new List<MirrorMap>();

string[] rowData = File.ReadAllLines("./data_test_p2.txt");
List<string> currentMapAsString = new List<string>();
int rowCounter = 0;
int mapNr = 1;
long totalValue = 0;
while (rowCounter < rowData.Length)
{
    if (rowData[rowCounter].Length > 1)
        currentMapAsString.Add(rowData[rowCounter]);

    if (rowData[rowCounter].Length == 0 || rowCounter == rowData.Length - 1)
    {
        MirrorMap mm = new([.. currentMapAsString]);
        mirrorMaps.Add(mm);
        currentMapAsString.Clear();
        Console.WriteLine("\n\nMap " + mapNr);

        //long value = mm.SolveP1();
        long value = mm.SolveP2();
        totalValue += value;

        Console.WriteLine(" > Total TOTAL is " + totalValue);
        mapNr++;
    }
    rowCounter++;
}







// Example solution
// Run the below console writeline to execute part 2
Console.WriteLine("\n\nCorrect total is: " + BorrowedPart2(rowData));

int BorrowedPart1(string[] lines) => BorrowedParseMirrors(lines).Select(x => BorrowedDetectMirror(x, useSmudge: false)).Sum();
int BorrowedPart2(string[] lines) => BorrowedParseMirrors(lines).Select(x => BorrowedDetectMirror(x, useSmudge: true)).Sum();


static IEnumerable<string[]> BorrowedParseMirrors(string[] lines)
{
    var mirror = new List<string>();

    foreach (var line in lines)
    {
        if (line == "")
        {
            yield return mirror.ToArray();
            mirror.Clear();
        }
        else
        {
            mirror.Add(line);
        }
    }

    if (mirror.Any())
    {
        yield return mirror.ToArray();
    }
}

static int BorrowedDetectMirror(string[] mirror, bool useSmudge)
{
    var mid = IsHorizontalMirror();
    if (mid >= 0) return (mid + 1) * 100;

    return IsVerticalMirror() + 1;

    int IsHorizontalMirror() =>
        ScanMirror(mirror.Length, mirror[0].Length,
            (int i, int i2) => Enumerable.Range(0, mirror[0].Length).Count(x => mirror[i][x] == mirror[i2][x]));

    int IsVerticalMirror() =>
        ScanMirror(mirror[0].Length, mirror.Length,
            (i, i2) => mirror.Count(l => l[i] == l[i2]));

    int ScanMirror(int imax, int dmax, Func<int, int, int> getSameCount)
    {
        for (var i = 0; i < imax - 1; i++)
            if (IsMirror(i, imax, dmax, getSameCount))
                return i;
        return -1;
    }

    bool IsMirror(int i, int imax, int length, Func<int, int, int> getSameCount)
    {
        var smudgedLength = useSmudge ? length - 1 : length;
        var wasSmudged = false;

        for (var i2 = i + 1; ; i--, i2++)
        {
            var sameCount = getSameCount(i, i2);

            if (sameCount != length && sameCount != smudgedLength)
                break;

            if (useSmudge && smudgedLength == sameCount)
            {
                // smudged may be used only once
                if (wasSmudged)
                    return false;

                wasSmudged = true;
            }

            // reached one of the ends?
            if (i == 0 || i2 == imax - 1)
                return !useSmudge || wasSmudged;
        }

        return false;
    }
}

