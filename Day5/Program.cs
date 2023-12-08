// Link
// https://adventofcode.com/2023/day/5

namespace Day5
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //PartOne();
            PartTwo();
        }

        private static void PartOne()
        {
            string[] data = File.ReadAllLines("./data_complete.txt");

            string[] seedsAsString = data[0][7..].Split(' ');
            long[] seeds = new long[seedsAsString.Length];
            for (long i = 0; i < seeds.Length; i++)
            {
                seeds[i] = long.Parse(seedsAsString[i]);
            }

            List<X2YMap> maps = new List<X2YMap>();
            List<string> mapAsString = new List<string>();
            for (long i = 2; i < data.Length; i++) // start at index 2 (skip first line "seeds")
            {
                mapAsString.Add(data[i]);
                if (i+1 >= data.Length || data[i+1] == string.Empty) // if next is a blank line
                {
                    maps.Add(new X2YMap(mapAsString.ToArray()));
                    mapAsString.Clear();
                    i++; // skip the empty line
                }
            }

            Dictionary<long, long> locations = new Dictionary<long, long>();
            long smallestLocation = long.MaxValue;
            for (long i = 0; i < seeds.Length; i++) // for each seed
            {
                long loc = FindLocationFromSeed(seeds[i], maps.ToArray());
                if (loc < smallestLocation) smallestLocation = loc;
                locations.Add(seeds[i], loc);
            }

            Console.WriteLine("\nAssignment result:");
            foreach (long key in locations.Keys)
            {
                Console.WriteLine($" > Seed {key} leads to location {locations[key]}");
            }
            Console.WriteLine($" Lowest value = {smallestLocation}");

            /* Took a while to work through this, but this is the result:
            Assignment result:
             > Seed 2880930400 leads to location 1561682361
             > Seed 17599561 leads to location 1968230273
             > Seed 549922357 leads to location 3510499789
             > Seed 200746426 leads to location 2633630198
             > Seed 1378552684 leads to location 3072121186
             > Seed 43534336 leads to location 1994165048
             > Seed 155057073 leads to location 2007594035
             > Seed 56546377 leads to location 2671588922
             > Seed 824205101 leads to location 2993761341
             > Seed 378503603 leads to location 2602698596
             > Seed 1678376802 leads to location 227653707
             > Seed 130912435 leads to location 2142327956
             > Seed 2685513694 leads to location 3062844213
             > Seed 137778160 leads to location 2149193681
             > Seed 2492361384 leads to location 2187261044
             > Seed 188575752 leads to location 2621459524
             > Seed 3139914842 leads to location 2084386313
             > Seed 1092214826 leads to location 4096859939
             > Seed 2989476473 leads to location 1728350770
             > Seed 58874625 leads to location 2673917170
             Lowest value = 227653707 (correct answer)
            */
        }


        private static void PartTwo()
        {
            string[] data = File.ReadAllLines("./data_complete.txt");
            
            // Define maps
            List<X2YMap> maps = new List<X2YMap>();
            List<string> mapAsString = new List<string>();
            for (long i = 2; i < data.Length; i++) // start at index 2 (skip first line "seeds")
            {
                mapAsString.Add(data[i]);
                if (i + 1 >= data.Length || data[i + 1] == string.Empty) // if next is a blank line
                {
                    maps.Add(new X2YMap(mapAsString.ToArray()));
                    mapAsString.Clear();
                    i++; // skip the empty line
                }
            }

            // They change the way we read the seeds.
            // Now the first seed value represents the START of the range
            // Second seed value is the LENGTH of the range
            string[] seedInputAsString = data[0][7..].Split(' ');
            long[] seedInput = new long[seedInputAsString.Length];
            for (long i = 0; i < seedInput.Length; i++)
            {
                seedInput[i] = long.Parse(seedInputAsString[i]);
            }

            // We create Intervals, ranges of values, defined by the lower and upper limits.
            List<IntervalLong> seedsRange = new List<IntervalLong>();
            for (int i = 0; i < seedInput.Length; i += 2) // range number
            {
                seedsRange.Add(new IntervalLong(seedInput[i], seedInput[i] + seedInput[i + 1]));
                Console.WriteLine("[INFO] Found a seed range between " + seedInput[i] + " and " + (seedInput[i] + seedInput[i + 1]));
            }


            // For each range we have, we return the "mapped" version of that range
            // as defined by the method FindLocationFromSeed.
            long lowest = long.MaxValue;
            foreach (IntervalLong seedRange in seedsRange)
            {
                Console.WriteLine("[INFO] Looping through seed range starting at " + seedRange.lower);
                long location = FindLocationFromSeed(seedRange, maps.ToArray()).lower;
                if (location < lowest)
                {
                    lowest = location;
                    Console.WriteLine(" - New lowest location: " + lowest);
                }
                else
                    Console.WriteLine(" - Found no new lowest location.");
            }
            Console.WriteLine(" > Lowest location: " + lowest);

            /*
            [INFO] Looping through seed range starting at 2880930400
             - New lowest location: 1561682361
            [INFO] Looping through seed range starting at 549922357
             - Found no new lowest location.
            [INFO] Looping through seed range starting at 1378552684
             - Found no new lowest location.
            [INFO] Looping through seed range starting at 155057073
             - Found no new lowest location.
            [INFO] Looping through seed range starting at 824205101
             - Found no new lowest location.
            [INFO] Looping through seed range starting at 1678376802
             - New lowest location: 78775051
            [INFO] Looping through seed range starting at 2685513694
             - Found no new lowest location.
            [INFO] Looping through seed range starting at 2492361384
             - Found no new lowest location.
            [INFO] Looping through seed range starting at 3139914842
             - Found no new lowest location.
            [INFO] Looping through seed range starting at 2989476473
             - Found no new lowest location.
             > Lowest location: 78775051
            */
        }

        private static bool writeConsoleOutput = false;

        private static long FindLocationFromSeed(long seed, X2YMap[] maps)
        {
            long destinationRangeStart;
            long sourceRangeStart;
            long rangeLength;

            long source = seed;
            if (writeConsoleOutput) Console.WriteLine("\n\n------------------------ Seed {0} ------------------------", seed);
            for (X2YMap.MapType i = 0; i <= X2YMap.MapType.Location; i++)
            {
                foreach (X2YMap map in maps) // Checking all maps
                {
                    if (map.FromMap == (X2YMap.MapType)i) // pick out the map that we're on
                    {
                        if (writeConsoleOutput) Console.WriteLine($" ## Mapping {source} from {map.FromMap} to {map.ToMap}.");
                        long destination;

                        // Look at one row at a time
                        for (long row = 0; row < map.values.GetLength(0); row++)
                        {
                            long[] currentRow = map.GetRow(row);
                            destinationRangeStart = currentRow[0];  // As defined by the assignment
                            sourceRangeStart = currentRow[1];       // As defined by the assignment
                            rangeLength = currentRow[2];            // As defined by the assignment

                            bool isBetweenRange = source >= sourceRangeStart && source <= sourceRangeStart + rangeLength;
                            if (isBetweenRange)
                            {
                                long diff = source - sourceRangeStart;
                                destination = destinationRangeStart + diff;

                                goto End; // we use a goto to ensure we exit two loops, because break only exits one.
                            }
                        }

                        // if number is not contained in the ranges described above
                        // a for example seed 10 would have soil value 10 (the same)
                        destination = source;
                        if (writeConsoleOutput) Console.WriteLine("   - currentSource: " + source + "   currentDestination: " + source);
                        
                        End:
                        source = destination;
                        if (writeConsoleOutput) Console.WriteLine(" >>> Location for seed {0} is {1}.", seed, destination);
                    }
                }
            }
            return source;
        }



        private static IntervalLong FindLocationFromSeed(IntervalLong seedRange, X2YMap[] maps)
        {
            long destinationRangeStart;
            long sourceRangeStart;
            long rangeLength;

            IntervalLong source = seedRange;

            for (X2YMap.MapType i = 0; i <= X2YMap.MapType.Location; i++)
            {
                foreach (X2YMap map in maps) // Checking all maps
                {
                    if (map.FromMap == (X2YMap.MapType)i) // pick out the map that we're on
                    {
                        IntervalLong destination = new IntervalLong(-1,-1);

                        // Look at one row at a time
                        for (long row = 0; row < map.values.GetLength(0); row++)
                        {
                            long[] currentRow = map.GetRow(row);
                            destinationRangeStart = currentRow[0];  // As defined by the assignment
                            sourceRangeStart = currentRow[1];       // As defined by the assignment
                            rangeLength = currentRow[2];            // As defined by the assignment

                            // The interval defined by the row
                            IntervalLong rowInterval = new IntervalLong(sourceRangeStart, sourceRangeStart + rangeLength - 1);

                            // If the intervals intersect at all, we say the destination is the lowest limit of the interval.
                            // If the previous destination was a smaller number, we keep the old one instead.
                            // This should lead to this method returning the lowest location
                            IntervalLong? clampedInterval = source.ClampInterval(rowInterval);
                            if (clampedInterval == null) continue;
                            else
                            {
                                long diff = destinationRangeStart - sourceRangeStart;
                                destination = new IntervalLong(clampedInterval.lower + diff, clampedInterval.upper + diff);
                                if (writeConsoleOutput) Console.Write(" + " + diff + "  low: " + clampedInterval.lower + "  high: " + clampedInterval.upper + "   ");
                                goto End;
                            }
                        }
                        if (writeConsoleOutput) Console.Write(" - ");
                        destination = source;

                        End:
                        source = destination;
                        if (writeConsoleOutput) Console.WriteLine((X2YMap.MapType)(i+1) + " range " + destination.lower + " to " + destination.upper);
                    }
                }
            }
            return source;
        }
    }
}