
using Day24;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.Intrinsics.X86;

bool useCompleteData = true;

string testDataPath = "./data_test.txt";
string completeDataPath = "./data_complete.txt";

//Part1(); // 16470 too high
Part2();

void Part2()
{
    // Because we essentially have 300 trajectories already defined
    // we *should* be able to intersect all of them as long as we have
    // all unknowns calculated.
    // x = x_0 + t*a
    // y = y_0 + t*b
    // z = z_0 + t*c
    // or for three lines 1, 2, and 3:
    // PI1 = P1 + t*V1
    // PI2 = P2 + t*V2
    // PI3 = P3 + t*V3
    // pI1..pI3 all on the same line, i.e. has same equation
    // There exists a value t such that
    // PI1 = P1 + t*V1
    // PI2 = P2 + t*V2
    // ...
    // PIn = Pn + t*Vn
    // where PI1, PI2, ... PIn all exist on the same line.
    // I can't really wrap my head around solving this mathematically through code.

    // Big thank you to Ryan Heath who I am shamelessly plagiarizing here https://github.com/ryanheath/aoc2023
    // (Although I changed it such that it works for my existing systems, with added comments)
    // Everything else used in this method is my own creation!

    Hailstorm storm;
    if (useCompleteData) storm = InterpretData(completeDataPath);
    else storm = InterpretData(testDataPath);

    Hail3D[] hails = storm.HailHashset.OrderBy(x => x.Position.X).ToArray();

    var (hail0, hail1, hail2, hail3) = (hails[0], hails[1], hails[^2], hails[^1]);

    foreach (var dvy in SearchSpace())
    {
        foreach (var dvx in SearchSpace())
        {
            // If we find an offset dvx dvy (var y and var x in search space)
            // that leads to the first line intersecting with all other three (second, and last, and second last)
            (decimal x, decimal y) ip01;
            decimal t01;
            (decimal x, decimal y) ip02;
            decimal t02;
            (decimal x, decimal y) ip03;
            decimal t03;

            // IsIntersectionXY checks intersection between lines 1 and 2 with an offset velocity, ignoring Z coordinate
            if (!hail0.IntersectsPathWithXY_velocityOffset(hail1, (dvx, dvy), out ip01, out t01))
                continue;
            if (!hail0.IntersectsPathWithXY_velocityOffset(hail2, (dvx, dvy), out ip02, out t02))
                continue;
            if (!hail0.IntersectsPathWithXY_velocityOffset(hail3, (dvx, dvy), out ip03, out t03))
                continue;

            // If the intersection coordinates aren't the same
            //if ((ip01.y, ip01.x) != (ip02.y, ip02.x) || (hail1.y, hail1.x) != (ip03.y, ip03.x)) continue;
            if (ip01 != ip02 || ip01 != ip03)
                continue;

            // if all intersects in XY, we can start checking for what Z value
            // For any value Z
            foreach (var dvz in SearchSpace())
            {
                if (dvz != -249) continue;

                var z1 = Zoft(hail1, t01, dvz);
                var z2 = Zoft(hail2, t02, dvz);
                if (z1 != z2)
                    continue;
                var z3 = Zoft(hail3, t03, dvz);
                if (z1 != z3)
                    continue;

                long result = (long)(ip01.x + ip01.y + z1);
                Console.WriteLine(" > Done: " + result);
                return;
            }

            // Z(t), i.e. a hail's Z position after time t has passed
            decimal Zoft(Hail3D hail, decimal t, long dvz) => Math.Round(hail.Position.Z + t * (hail.Velocity.Z + dvz), 3);
        }
    }

    throw new UnreachableException();

    // Represents the range of deltaV we will explore
    static IEnumerable<int> SearchSpace() => Enumerable.Range(-300, 600);
}
void Part1()
{
    Hailstorm storm;

    if (useCompleteData) storm = InterpretData(completeDataPath);
    else storm = InterpretData(testDataPath);

    Dictionary<Hail3D, List<(Hail3D other, (decimal x, decimal y) intersectionPoint, bool isInsideTestArea)>> result;

    if (useCompleteData)
        result = storm.GetIntersectionsXY(new Vector2(200000000000000, 400000000000000));
    else
        result = storm.GetIntersectionsXY(new Vector2(7, 27));

    List<(Hail3D firstHail, Hail3D secondHail, (decimal x, decimal y) intersectionPoint, bool isInsideTestArea)> totalResults = [];
    foreach (var list in result)
    {
        foreach (var element in list.Value)
        {
            totalResults.Add((list.Key, element.other, element.intersectionPoint, element.isInsideTestArea));
        }
    }

    Console.WriteLine(" > Total intersecting hail: " + totalResults.Count);
    Console.WriteLine("             Within bounds: " + totalResults.Where(x => x.isInsideTestArea == true).Count());
}

Hailstorm InterpretData(string path)
{
    string[] data = File.ReadAllLines(path);
    HashSet<Hail3D> hails = new HashSet<Hail3D>();

    for (int j = 0; j < data.Length; j++)
    {
        string[] lines = data[j].Split(" @ ");
        long[] posArray = lines[0].Split(", ").Select(long.Parse).ToArray();
        long[] velArray = lines[1].Split(", ").Select(long.Parse).ToArray();
        //Vector3 pos = new Vector3((float)((double)posArray[0] / 100000000000000d), (float)((double)posArray[1] / 100000000000000d), (float)((double)posArray[2] / 100000000000000d));
        Decimal3 pos = new Decimal3(posArray[0], posArray[1], posArray[2]);
        Decimal3 vel = new Decimal3(velArray[0], velArray[1], velArray[2]);

        hails.Add(new Hail3D(pos, vel));
    }
    return new Hailstorm(hails);
}