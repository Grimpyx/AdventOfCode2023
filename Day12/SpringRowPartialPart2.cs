namespace Day12
{
    partial class SpringRow
    {
        // Initially I tried memoization.
        // The method worked, but was a bit too slow. I then introduced multi threading,
        // after a while the cache got so big that after 200 iterations the time taken to complete one iteration was insanely long.
        // I must have done *something* wrong, but I couldn't see the error.
        // Instead I found a solution that didn't require memoization by a reddit user u/KayZGames.
        // I translated Dart -> C# with optional console printing (set writeDebugPrint to true)
        // https://www.reddit.com/r/adventofcode/comments/18hbjdi/2023_day_12_part_2_this_image_helped_a_few_people/
        // All reference material is also included in this project: Day12/DartImplementation


        public long CreatePermutations()
        {
            bool writeDebugPrint = false;

            int[] groups = continuousGroups;
            string line = RemoveConsecutiveCharacter('.', SpringsAsString().Trim('.')); // something like ...##.#..##???.???...??.?. -> ##.#.##???.???.??.?
                                                                                        //                ^^^    ^          ^^     ^
            Dictionary<(int group, int amount), long> states = new Dictionary<(int group, int amount), long>() { { (0, 0), 1 } };
            Dictionary<(int group, int amount), long> nextStates = new Dictionary<(int group, int amount), long>();

            int totalNumberOfUnknownOrDamaged = line.Where(c => c != '.').Count();

            List<int> minimumNumberOfBrokenSpringsLeft = new List<int>();
            for (int i = 0; i < totalNumberOfUnknownOrDamaged; i++)
                minimumNumberOfBrokenSpringsLeft.Add(groups.Skip(i).Sum());

            int springCounter = 0;
            foreach (char spring in line)
            {
                if (spring != '.') totalNumberOfUnknownOrDamaged--;

                if (writeDebugPrint)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(" | ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(new string(' ', springCounter) + line[springCounter] + new string(' ', line.Length - springCounter + 1));
                    Console.ForegroundColor = ConsoleColor.White;
                }

                foreach (var state in states)
                {
                    int group = state.Key.group;
                    int amount = state.Key.amount;
                    long permutations = state.Value;

                    // increase amount of broken springs for the current group
                    // but only if the maximum of broken springs the current group hasn't been reached
                    if (spring == '#' || spring == '?')
                    {
                        if (group < groups.Length && amount < groups[group]) // If we haven't exceeded 
                        {
                            nextStates[(group, amount + 1)] = permutations; // we update the state to contain one more broken spring
                        }
                    }

                    // end current group if amount of broken springs equals the required amount
                    // or keep going if there are no broken springs in the current group
                    if (spring == '.' || spring == '?')
                    {
                        if (amount == 0)
                        {
                            // merge permutation count with other group that ended this loop
                            if (nextStates.ContainsKey((group, amount: 0)))
                                nextStates[(group, amount: 0)] += permutations;
                            else nextStates.Add((group, amount: 0), permutations);
                        }
                        else if (amount == groups[group])
                        {
                            // merge permutation count with other group that's already been ended in a previous loop
                            if (nextStates.ContainsKey((group + 1, amount: 0)))
                                nextStates[(group + 1, amount: 0)] += permutations;
                            else nextStates.Add((group + 1, amount: 0), permutations);
                        }
                    }
                }
                if (writeDebugPrint)
                {
                    foreach (var state in states)
                        Console.Write($" (({state.Key.group}, {state.Key.amount}), {state.Value})");
                    Console.WriteLine();
                }


                // remove all states that can't be finished because there aren't enough broken springs left
                nextStates.RemoveAll((key, permutations) => totalNumberOfUnknownOrDamaged + key.amount < minimumNumberOfBrokenSpringsLeft[key.group]);


                states = new Dictionary<(int group, int amount), long>(nextStates);
                nextStates.Clear();

                springCounter++;
            }

            return states.Values.Sum();

            // Explaining output:
            //  | ?###???????? 3,2,1
            //  | ?                 ((0, 0), 1)
            //  |  #                ((0, 1), 1)     ((0, 0), 1) <-- new possibility
            //  |   #               ((0, 2), 1)     ((0, 1), 1)
            //  |    #              ((0, 3), 1)     ((0, 2), 1) <-- end of road of a path, wasn't valid so is removed
            //  |     ?             ((0, 3), 1)
            //  |      ?            ((1, 0), 1)
            //  |       ?           ((1, 1), 1)     ((1, 0), 1)
            //  |        ?          ((1, 2), 1)     ((1, 1), 1)     ((1, 0), 1)
            //  |         ?         ((2, 0), 1)     ((1, 2), 1)     ((1, 1), 1)     ((1, 0), 1)
            //  |          ?        ((2, 1), 1)     ((2, 0), 2)     ((1, 2), 1)     ((1, 1), 1)     ((1, 0), 1)
            //  |           ?       ((3, 0), 1)     ((2, 1), 2)     ((2, 0), 3)     ((1, 2), 1)     ((1, 1), 1) < invalid, is removed
            //  |            ?      ((3, 0), 3)     ((2, 1), 3)     ((2, 0), 4)     ((1, 2), 1)
            //  | Value : 10
        }
    }
}
