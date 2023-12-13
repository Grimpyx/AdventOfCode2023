using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Day6;

namespace Day9
{
    public class OasisHistory
    {
        public OasisHistoryEntry[] entries;

        public OasisHistory(OasisHistoryEntry[] values)
        {
            this.entries = values;
        }

        public OasisHistory(string[] readAllLinesInput)
        {

            OasisHistoryEntry[] ohes = new OasisHistoryEntry[readAllLinesInput.Length];
            for (int i = 0; i < readAllLinesInput.Length; i++)
            {
                ohes[i] = new OasisHistoryEntry(readAllLinesInput[i]);
            }
            this.entries = ohes;
        }
    }
}
