// See https://aka.ms/new-console-template for more information

using System.Collections.Generic;
using System.Diagnostics;
using V3 = (int x, int y, int z);


//Part1(); // 617 too high, 51X correct
Part2(); // 62767 too high, 612XX correct
Console.WriteLine();

void Part2()
{
    Stopwatch sw = Stopwatch.StartNew();
    //List<Brick> bricks = InterpretInput(File.ReadAllLines("./data_test.txt"));
    List<Brick> bricks = InterpretInput(File.ReadAllLines("./data_complete.txt"));
    BrickManager brickManager = new BrickManager(bricks);
    int bricksThatWillFall = brickManager.GetBricksThatWillFall();
    sw.Stop();

    Console.WriteLine(" > Nr: " + bricksThatWillFall);
    Console.WriteLine(" > Took " + sw.ElapsedMilliseconds + "ms");
    Console.WriteLine();
}


void Part1()
{
    //List<Brick> bricks = InterpretInput(File.ReadAllLines("./data_test.txt"));
    Stopwatch sw = Stopwatch.StartNew();
    List<Brick> bricks = InterpretInput(File.ReadAllLines("./data_complete.txt"));
    BrickManager brickManager = new BrickManager(bricks);
    int bricksThatCanBeRemoved = brickManager.GetBricksThatCanBeRemoved();
    sw.Stop();

    Console.WriteLine(" > Nr: " + bricksThatCanBeRemoved);
    Console.WriteLine(" > Took " + sw.ElapsedMilliseconds + "ms");
    Console.WriteLine();
}



List<Brick> InterpretInput(string[] readAllLines)
{
    List<Brick> lob = new List<Brick>();
    foreach (var line in readAllLines)
    {
        lob.Add(new Brick(line));
    }
    return lob;
}

class BrickManager
{
    List<Brick> allBricks;
    List<V3> allOccupiedSpaces;
    HashSet<V3> allOccupiedSpacesHashset;

    public BrickManager(List<Brick> allBricks)
    {
        this.allBricks = allBricks;
        allOccupiedSpaces = new List<V3>();
        foreach (var item in allBricks)
        {
            allOccupiedSpaces.AddRange(item.GetOccupiedSpace());
        }
        allOccupiedSpaces = allOccupiedSpaces.Distinct().ToList();
        allOccupiedSpacesHashset = [..allOccupiedSpaces];
        MoveAllBricksDown();
    }

    public int GetBricksThatWillFall()
    {
        Dictionary<V3, Brick> coordinateToBrick = new Dictionary<V3, Brick>();
        foreach (var item1 in allBricks)
        {
            foreach (var item2 in item1.GetOccupiedSpace())
            {
                coordinateToBrick.Add(item2, item1);
            }
        }

        Dictionary<Brick, List<Brick>> allBelowBrick = new Dictionary<Brick, List<Brick>>();
        Dictionary<Brick, List<Brick>> allAboveBrick = new Dictionary<Brick, List<Brick>>();
        foreach (var item in allBricks)
        {
            HashSet<V3> vs_below = item.GetAllSpacesBelow();
            HashSet<V3> vs_above = item.GetAllSpacesAbove();
            List<Brick> allBricksBelow = new List<Brick>();
            List<Brick> allBricksAbove = new List<Brick>();
            foreach (var item2 in vs_below)
            {
                if (coordinateToBrick.TryGetValue(item2, out Brick brick))
                {
                    if (brick != item && !allBricksBelow.Contains(brick))
                        allBricksBelow.Add(brick);
                }
            }
            foreach (var item2 in vs_above)
            {
                if (coordinateToBrick.TryGetValue(item2, out Brick brick))
                {
                    if (brick != item && !allBricksAbove.Contains(brick))
                        allBricksAbove.Add(brick);
                }
            }

            allBelowBrick.Add(item, allBricksBelow); // ALL BELOW. If you call allBelowBrick[b] you get all bricks below b
            allAboveBrick.Add(item, allBricksAbove); // ALL ABOVE. If you call allAboveBrick[b] you get all bricks above b
        }
        
        // Select a brick.
        // How many bricks will fall at least one step
        int numberOfBricksThatWillFall = 0;
        foreach (var brick in allBricks)
        {
            HashSet<Brick> bricksThatWillFall = RemovalLeadToFall(brick);
            numberOfBricksThatWillFall += bricksThatWillFall.Count - 1; // -1 because it counts the first brick parameter
        }
        return numberOfBricksThatWillFall;



        HashSet<Brick> RemovalLeadToFall(Brick brick)
        {
            HashSet<Brick> fallenBricks = [];

            PriorityQueue<Brick, int> queue = new PriorityQueue<Brick, int>();
            int queueWeight = 0;

            // First element we look at is the supplied brick
            queue.Enqueue(brick, queueWeight);
            queueWeight++;

            while (queue.Count > 0)
            {
                var b = queue.Dequeue();
                fallenBricks.Add(b); // add it to fallen bricks

                // For all bricks above the current fallen brick
                foreach (var above in allAboveBrick[b])
                {
                    // If (the supports of that above brick) minus (all the already fallen) has any elements it will not fall. Continue.
                    if (allBelowBrick[above].Except(fallenBricks).Any())
                        continue;

                    // Otherwise we add it to the queue
                    queue.Enqueue(above, queueWeight);
                    queueWeight++;
                }
            }

            return fallenBricks;

            /*// For a particular brick B,
            // All bricks above (ABOVE) it must satisfy the condition:
            // Among those below ABOVE, at least one needs to NOT be B.
            // This means B can be safely removed, and the brick above
            // will still have at least one supporting brick.

            // if All bricks above brick B, has at least 1 other supporting beam that is not B

            // Select all bricks above (A:s) that only has brick B as support, meaning A:s will fall if B is removed
            //HashSet<Brick> bricksAboveThatWillFall = new HashSet<Brick>();
            HashSet<Brick> newWillBeDisintegrated = new HashSet<Brick>();
            foreach (var brick in bricksThatWillDis)
            {
                // Select all bricks above
                var allAboveBricksAndTheirSupports = allAboveBrick[brick].Select(above => (aboveBrick: above, supports: new HashSet<Brick>(allBelowBrick[above]))); // .Where(above => allBelowBrick[above].All(x => x == brick)).ToList();

                foreach (var aboveBrickAndItsSupports in allAboveBricksAndTheirSupports)
                {
                    HashSet<Brick> supportsExcludingDisintegrated = [..aboveBrickAndItsSupports.supports.Except(bricksThatWillDis)];
                    // if this returned list contains more than 0 elements, it has a support that is not getting disintegrated -> wont fall
                    if (supportsExcludingDisintegrated.Count() <= 0)
                    {
                        newWillBeDisintegrated.Add(aboveBrickAndItsSupports.aboveBrick);
                        totalBefore.Add(aboveBrickAndItsSupports.aboveBrick);
                    }
                }
            }
            if (newWillBeDisintegrated.Count > 0)
                totalBefore = RemovalLeadToFall(newWillBeDisintegrated, totalBefore);

            return totalBefore;*/




            /*
            HashSet<Brick> bricksAboveThatWillFall = allAboveBrick[brick].Where(above => allBelowBrick[above].All(x => x == brick)).ToList();
            PriorityQueue<Brick, int> priorityQueue = new PriorityQueue<Brick, int>();
            int queueWeight = 0;
            priorityQueue.EnqueueRange(bricksAboveThatWillFall, queueWeight);
            while (priorityQueue.Count > 0)
            {
                queueWeight++;
                Brick dequeuedBrick = priorityQueue.Dequeue();
                List<Brick> nextBricks = RemovalLeadToFall(dequeuedBrick);

                bricksAboveThatWillFall.AddRange(nextBricks);
                priorityQueue.EnqueueRange(nextBricks, queueWeight);
            }

            // Perform same check for all A:s, becuase if they fall it kind of counts as being removed for the sake of analysing falls.

            return bricksAboveThatWillFall;*/
        }
    }


    public int GetBricksThatCanBeRemoved()
    {
        Dictionary<V3, Brick> coordinateToBrick = new Dictionary<V3, Brick>();
        foreach (var item1 in allBricks)
        {
            foreach (var item2 in item1.GetOccupiedSpace())
            {
                coordinateToBrick.Add(item2, item1);
            }
        }


        Dictionary<Brick, List<Brick>> allBelowBrick = new Dictionary<Brick, List<Brick>>();
        Dictionary<Brick, List<Brick>> allAboveBrick = new Dictionary<Brick, List<Brick>>();
        foreach (var item in allBricks)
        {
            HashSet<V3> vs_below = item.GetAllSpacesBelow();
            HashSet<V3> vs_above = item.GetAllSpacesAbove();
            List<Brick> allBricksBelow = new List<Brick>();
            List<Brick> allBricksAbove = new List<Brick>();
            foreach (var item2 in vs_below)
            {
                if (coordinateToBrick.TryGetValue(item2, out Brick brick))
                {
                    if (brick != item && !allBricksBelow.Contains(brick))
                        allBricksBelow.Add(brick);
                }
            }
            foreach (var item2 in vs_above)
            {
                if (coordinateToBrick.TryGetValue(item2, out Brick brick))
                {
                    if (brick != item && !allBricksAbove.Contains(brick))
                        allBricksAbove.Add(brick);
                }
            }

            allBelowBrick.Add(item, allBricksBelow); // ALL BELOW. If you call allBelowBrick[b] you get all bricks below b
            allAboveBrick.Add(item, allBricksAbove); // ALL ABOVE. If you call allAboveBrick[b] you get all bricks above b
        }



        HashSet<Brick> bricksToDisintegrate = new HashSet<Brick>();

        int counter = 0;
        foreach (var item in allBricks.OrderBy(x => x.id))
        {
            if (IsDisintegratable(item)) counter++;
        }
        return counter;


        bool IsDisintegratable(Brick brick)
        {
            // For a particular brick B,
            // All bricks above (ABOVE) it must satisfy the condition:
            // Among those below ABOVE, at least one needs to NOT be B.
            // This means B can be safely removed, and the brick above
            // will still have at least one supporting brick.

            // if All bricks above brick B, has at least 1 other supporting beam that is not B
            return allAboveBrick[brick].All(above => allBelowBrick[above].Any(x => x != brick));
        }
    }


    public void MoveAllBricksDown()
    {
        int maxZ = allOccupiedSpaces.Select(x => x.z).Max();

        for (int i = 1; i <= maxZ; i++)
        {
            // All objects on this level
            // Selects Z coordinate of occupied spaces, and if that Z is i, that brick has one space occupied in z-level i
            //var bricksOnThisLevel = allBricks.Select(x => x.GetOccupiedSpace().Select(x => x.z)).Where(x => x.Contains(i));
            var bricksOnThisLevel = allBricks.Where(brick => brick.GetOccupiedSpace().Select(v => v.z).Contains(i)).ToList();

            // Move all of them down, until none can be moved down any further
            PriorityQueue<Brick, int> brickQueue = new PriorityQueue<Brick, int>();
            //brickQueue.EnqueueRange(bricksOnThisLevel, 1);
            int[] queueValues = Enumerable.Range(0, bricksOnThisLevel.Count).ToArray();
            brickQueue.EnqueueRange(bricksOnThisLevel.Select(x => (x, queueValues[bricksOnThisLevel.IndexOf(x)])));
            

            while (brickQueue.Count > 0)
            {
                var b = brickQueue.Dequeue();
                if (IsClearUnderneath(b))
                {
                    foreach (var os in b.GetOccupiedSpace())
                        allOccupiedSpacesHashset.Remove(os);
                    b.MoveDownOnce();
                    foreach (var item in b.GetOccupiedSpace())
                        allOccupiedSpacesHashset.Add(item);
                    brickQueue.Enqueue(b,0);
                    continue;
                }
            }

            /*
            bool anyWasMoved = false;
            while (true)
            {
                anyWasMoved = false;

                foreach (Brick brick in bricksOnThisLevel)
                {
                    if (IsClearUnderneath(brick))
                    {
                        foreach (var item in brick.GetOccupiedSpace())
                            allOccupiedSpacesHashset.Remove(item);
                        brick.MoveDownOnce();
                        foreach (var item in brick.GetOccupiedSpace())
                            allOccupiedSpacesHashset.Add(item);
                        anyWasMoved = true;
                        break;
                    }
                }

                if (anyWasMoved) continue;
                else break;

            }*/

        }
    }

    public bool IsClearUnderneath(Brick brick)
    {
        foreach (var v in brick.GetAllSpacesBelow())
        {
            if (brick.GetOccupiedSpace().Contains(v))
                continue; // if contains itself, look at next instead

            if (v.z <= 0)
                return false; // is on the bottom

            if (allOccupiedSpacesHashset.Contains(v))
                return false;
        }

        return true;
    }
}

class Brick
{
    static int brickCounter = 0;

    public int id;
    public V3 start;
    public V3 end;

    HashSet<V3> occupiedSpace;
    HashSet<V3> spaceAbove;
    HashSet<V3> spaceBelow;

    public Brick(V3 start, V3 end)
    {
        this.start = start;
        this.end = end;

        this.id = brickCounter;
        brickCounter++;
    }

    public Brick(string line)
    {
        string[] split = line.Split('~');
        string[] one = split[0].Split(',');
        string[] two = split[1].Split(',');

        start = (int.Parse(one[0]), int.Parse(one[1]), int.Parse(one[2]));
        end = (int.Parse(two[0]), int.Parse(two[1]), int.Parse(two[2]));

        this.id = brickCounter;
        brickCounter++;
    }

    public HashSet<V3> GetAllSpacesAbove()
    {
        if (spaceAbove == null || spaceAbove.Count == 0)
        {
            V3[] os = [..GetOccupiedSpace()];
            for (int i = 0; i < os.Length; i++)
            {
                os[i].z++;
            }
            spaceAbove = new HashSet<V3>(os);
        }
        return spaceAbove;

    }
    public HashSet<V3> GetAllSpacesBelow()
    {
        if (spaceBelow == null || spaceBelow.Count == 0)
        {
            V3[] os = [.. GetOccupiedSpace()];
            for (int i = 0; i < os.Length; i++)
            {
                os[i].z--;
            }
            spaceBelow = new HashSet<V3>(os);
        }
        return spaceBelow;
    }

    public void MoveDownOnce()
    {
        start = (start.x, start.y, start.z - 1);
        end = (end.x, end.y, end.z - 1);

        // such that the occupied space will update 
        occupiedSpace = [];
        spaceBelow = [];
        spaceAbove = [];
    }

    public HashSet<V3> GetOccupiedSpace()
    {
        if (occupiedSpace == null || occupiedSpace.Count == 0)
        {
            V3 first = start;
            V3 second = end;

            V3 diff = (second.x - first.x, second.y - first.y, second.z - first.z);
            V3 step = (Math.Sign(diff.x), Math.Sign(diff.y), Math.Sign(diff.z));
            int totalSteps = Math.Abs(diff.x + diff.y + diff.z);
            HashSet<V3> space = new HashSet<V3>();

            for (int i = 0; i <= totalSteps; i++)
            {
                V3 next = (first.x + i * step.x, first.y + i * step.y, first.z + i * step.z);
                space.Add(next);
            }
            occupiedSpace = [..space];
        }

        return occupiedSpace;
    }

    public override bool Equals(object? obj)
    {
        return obj is Brick brick &&
               id == brick.id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(id);
    }

    public override string? ToString()
    {
        return id.ToString();
    }
}