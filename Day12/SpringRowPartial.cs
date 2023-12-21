//using Memoize;
using Memoization;
using System.Text.RegularExpressions;

namespace Day12
{
    partial class SpringRow
    {




        public long CreatePermutations()
        {
            bool writeDebugPrint = false;

            // In tutorial:
            // allsprings = line
            // conditions(int list) = groups

            int[] groups = continuousGroups;
            string line = RemoveConsecutiveCharacter('.', SpringsAsString().Trim('.')); // something like ...##.#..##???.???...??.?. -> ##.#.##???.???.??.?
                                                                                        //                ^^^    ^          ^^     ^
            Dictionary<(int group, int amount), long> states = new Dictionary<(int group, int amount), long>() { {(0, 0), 1} };
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
                        if (group < groups.Length && amount < groups[group])
                        {
                            nextStates[(group, amount + 1)] = permutations;
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



        // Trying to implement the following solution:
        // https://www.reddit.com/r/adventofcode/comments/18hbbxe/2023_day_12python_stepbystep_tutorial_with_bonus/

        //private static Func<(string, List<int>), int>? functionMemo_calculate;


        //private static Func<string, int[], int>? functionMemo_calculate;
        private static Memoizer<string, int[], int>? functionMemo_calculate;
        private static Memoizer<string, int[], int>? functionMemo_dot;
        private static Memoizer<string, int[], int, int>? functionMemo_pound;
        public int Calculate(string record, int[] groups)
        {
            //string record = tuple.record;
            //int[] groups = tuple.groups;


            functionMemo_calculate = Memoizer.Memoize<string, int[], int>((_record,_groups) =>
            //functionMemo_calculate ??= Memoizer.Memoize<string, int[], int>((_record, _groups) =>
            {
                if (_record == null || _groups.Length < 1)
                {
                    if (!_record.Contains('#')) return 1;
                    else return 0;
                }
                if (_record == null || _record.Length < 1) return 0;


                char nextCharacter = _record[0];
                int nextGroup = _groups[0];
                int output = 0;

                if (nextCharacter == '#')
                {
                    output = Pound(_record, _groups, nextGroup);
                }
                else if (nextCharacter == '.')
                {
                    output = Dot(_record, _groups);
                }
                else if (nextCharacter == '?')
                {
                    output = Dot(_record, _groups) + Pound(_record, _groups, nextGroup);
                }

                string groupsasstring = "(";
                for (int i = 0; i < _groups.Length; i++)
                {
                    if (i != 0) groupsasstring += ", ";
                    groupsasstring += _groups[i].ToString();
                }
                groupsasstring += ")";

                return output;
            });

            return functionMemo_calculate.Call(record, groups);


            //functionMemo_calculate ??= Memoizer.Memoize<(string record, List<int> groups), int>((v) =>
            //    {
            //        /*if (groups == null || groups.Count < 1)
            //        {
            //            if (!record.Contains('#')) return 1;
            //            else return 0;
            //        }
            //        if (record == null || record.Length < 1) return 0;


            //        char nextCharacter = record[0];
            //        int nextGroup = groups[0];
            //        int output = 0;

            //        if (nextCharacter == '#')
            //        {
            //            output = Pound(record, groups, nextGroup);
            //        }
            //        else if (nextCharacter == '.')
            //        {
            //            output = Dot(record, groups);
            //        }
            //        else if (nextCharacter == '?')
            //        {
            //            output = Dot(record, groups) + Pound(record, groups, nextGroup);
            //        }

            //        string groupsAsString = "(";
            //        for (int i = 0; i < groups.Count; i++)
            //        {
            //            if (i != 0) groupsAsString += ", ";
            //            groupsAsString += groups[i].ToString();
            //        }
            //        groupsAsString += ")";
            //        Console.WriteLine("Calc result: " + record + " " + groupsAsString + " => " + output);

            //        return output;*/

            //        if (v.groups == null || v.groups.Count < 1)
            //        {
            //            if (!v.record.Contains('#')) return 1;
            //            else return 0;
            //        }
            //        if (v.record == null || v.record.Length < 1) return 0;


            //        char nextCharacter = v.record[0];
            //        int nextGroup = v.groups[0];
            //        int output = 0;

            //        if (nextCharacter == '#')
            //        {
            //            output = Pound(v.record, v.groups, nextGroup);
            //        }
            //        else if (nextCharacter == '.')
            //        {
            //            output = Dot(v.record, v.groups);
            //        }
            //        else if (nextCharacter == '?')
            //        {
            //            output = Dot(v.record, v.groups) + Pound(v.record, v.groups, nextGroup);
            //        }

            //        string groupsAsString = "(";
            //        for (int i = 0; i < v.groups.Count; i++)
            //        {
            //            if (i != 0) groupsAsString += ", ";
            //            groupsAsString += v.groups[i].ToString();
            //        }
            //        groupsAsString += ")";
            //        //Console.WriteLine("Calc result: " + v.record + " " + groupsAsString + " => " + output);

            //        return output;
            //    });

            //return functionMemo_calculate((record, groups));

        }


        //private static Func<(string, List<int>, int), int>? functionMemo_pound; 
        public int Pound(string record, int[] groups, int nextGroup)
        {
            if (record.Length < nextGroup) return 0;

            string thisGroup = record[..nextGroup];
            thisGroup = thisGroup.Replace('?', '#');

            if (thisGroup != new string('#', nextGroup)) return 0;

            if (record.Length == nextGroup)
            {
                if (groups.Length == 1)
                    return 1;
                else
                    return 0;
            }

            if (record[nextGroup] == '?' || record[nextGroup] == '.')
                return Calculate(record[(nextGroup + 1)..], groups[1..]);

            return 0;



            /*functionMemo_pound ??= Memoizer.Memoize<string, int[], int, int>((_record, _groups, _nextGroup) =>
            {
                if (_record.Length < _nextGroup) return 0;

                string thisGroup = _record[.._nextGroup];
                thisGroup = thisGroup.Replace('?', '#');

                if (thisGroup != new string('#', _nextGroup)) return 0;

                if (_record.Length == _nextGroup)
                {
                    if (_groups.Length == 1)
                        return 1;
                    else
                        return 0;
                }

                if (_record[_nextGroup] == '?' || _record[_nextGroup] == '.')
                    return Calculate(_record[(_nextGroup + 1)..], _groups[1..]);

                return 0;
            });

            return functionMemo_pound.Call(record, groups, nextGroup);*/








            /*if (record.Length < nextGroup) return 0;

            string thisGroup = record[..nextGroup];
            thisGroup = thisGroup.Replace('?', '#');

            if (thisGroup != new string('#', nextGroup)) return 0;

            if (record.Length == nextGroup)
            {
                if (groups.Length == 1)
                    return 1;
                else
                    return 0;
            }

            if (record[nextGroup] == '?' || record[nextGroup] == '.')
                return Calculate(record[(nextGroup + 1)..], groups[1..]);

            return 0;*/


            /*if (functionMemo_pound == null)
            {
                functionMemo_pound ??= Memoizer.Memoize<(string record, List<int> groups, int nextGroup), int>((v) =>
                {
                    if (v.record.Length < v.nextGroup) return 0;

                    string thisGroup = v.record[..v.nextGroup];
                    thisGroup = thisGroup.Replace('?', '#');

                    if (thisGroup != new string('#', v.nextGroup)) return 0;

                    if (v.record.Length == v.nextGroup)
                    {
                        if (v.groups.Count == 1)
                            return 1;
                        else
                            return 0;
                    }

                    if (v.record[v.nextGroup] == '?' || v.record[v.nextGroup] == '.')
                        return Calculate((v.record[(v.nextGroup + 1)..], v.groups[1..]));

                    return 0;
                });
            }


            return functionMemo_pound((record, groups, nextGroup));*/
        }


        //private static Func<(string, List<int>), int>? functionMemo_dot;
        public int Dot(string record, int[] groups)
        {
            return Calculate(record[1..], groups);
            /*functionMemo_dot ??= Memoizer.Memoize<string, int[], int>((_record, _groups) =>
            {
                return Calculate(_record[1..], _groups);

            });
            return functionMemo_dot.Call(record, groups);*/
        }
    }
}
