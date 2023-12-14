using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Day6;

namespace Day9
{
    public class OasisHistoryEntry
    {
        public SubEntry[] subEntries;

        public OasisHistoryEntry(SubEntry[] subEntries)
        {
            this.subEntries = subEntries;
        }
        public OasisHistoryEntry(string fromString)
        {
            long[] startingNumbers = DataInterpret.GetLongNumbersArrayFrom(fromString, ' ');

            this.subEntries = GetSubEntries(startingNumbers).ToArray();
        }

        private List<SubEntry> GetSubEntries(long[] numbers)
        {
            if (numbers.Length <= 0)
                return new List<SubEntry>();

            List<SubEntry> entries = new List<SubEntry>();
            
            SubEntry firstEntry = new SubEntry([..numbers]);
            entries.Add(firstEntry);
            if (firstEntry == null) return entries;

            SubEntry? currentEntry = firstEntry;

            while (true)
            {
                if (currentEntry == null) return entries;
                currentEntry = currentEntry.GetNextSubEntry();
                if (currentEntry == null) continue;
                entries.Add(currentEntry);

                if (currentEntry.isEnd) break;
            }

            return entries;
        }

        public void Extrapolate(int times)
        {
            if (times > 0)
            {
                for (int iteration = 0; iteration < times; iteration++)
                {
                    // Select all Last elements and
                    List<long> lastValues = new List<long>();
                    foreach (SubEntry entry in subEntries)
                    {
                        lastValues.Add(entry.values[^1]);
                    }
                    lastValues.Reverse(); // we want to count from the bottom, so we reverse the order to make it a bit easier 


                    // Generate the extrapolated values
                    long[] extrapolatedValues = new long[lastValues.Count];
                    for (int i = 0; i < lastValues.Count; i++)
                    {
                        if (i == 0) extrapolatedValues[i] = 0; // special case for the first value which is always 0
                        else extrapolatedValues[i] = extrapolatedValues[i - 1] + lastValues[i];
                    }

                    // Merge into the sub entries
                    for (int i = 0; i < extrapolatedValues.Length; i++)
                    {
                        subEntries[i].values.Add(extrapolatedValues[^(i + 1)]);
                    }
                }
            }
            else if (times < 0)
            {
                for (int iteration = 0; iteration < times * -1; iteration++)
                {
                    // Select all Last elements and
                    List<long> lastValues = new List<long>();
                    foreach (SubEntry entry in subEntries)
                    {
                        lastValues.Add(entry.values[0]);
                    }
                    lastValues.Reverse(); // we want to count from the bottom, so we reverse the order to make it a bit easier 


                    // Generate the extrapolated values
                    long[] extrapolatedValues = new long[lastValues.Count];
                    for (int i = 0; i < lastValues.Count; i++)
                    {
                        if (i == 0) extrapolatedValues[i] = 0; // special case for the first value which is always 0
                        else extrapolatedValues[i] = lastValues[i] - extrapolatedValues[i - 1];
                    }

                    // Merge into the sub entries
                    for (int i = 0; i < extrapolatedValues.Length; i++)
                    {
                        subEntries[i].values.Insert(0, extrapolatedValues[^(i + 1)]);
                    }
                }
            }
        }

        public string ToPrintedString()
        {
            string s = "";
            long longestLength = subEntries[0].values.Select(x => x.ToString().Length).Max();
            int indentation = 1;

            foreach (SubEntry subEntry in subEntries)
            {
                for (int i = 0; i < subEntry.values.Count; i++)
                {
                    if (i != 0) s += " ";
                    string stringToAdd = subEntry.values[i].ToString();
                    s += new string(' ', (int)Math.Max(longestLength - stringToAdd.Length, 1));
                    s += stringToAdd;
                }
                s += "\n" + new string(' ', indentation * (int)longestLength);
                indentation++;
            }
            return s;
        }
    }
}
