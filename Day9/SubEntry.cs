using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day9
{
    public class SubEntry
    {
        public List<long> values;
        public bool isEnd;
        //public SubEntry? nextSubEntry;

        public SubEntry(List<long> values)
        {
            this.values = values;

            bool allIsZero = true;
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] != 0)
                {
                    allIsZero = false;
                    break;
                }
            }

            isEnd = allIsZero;

            //if (!isEnd) nextSubEntry = GetNextSubEntry();
        }

        public SubEntry? GetNextSubEntry()
        {
            if (isEnd) return null;

            long[] diff = new long[values.Count - 1];
            for (int i = 0; i < diff.Length; i++)
            {
                diff[i] = values[i + 1] - values[i];
            }
            return new SubEntry([..diff]);
        }
    }
}
