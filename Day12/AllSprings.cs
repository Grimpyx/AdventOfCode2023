using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day12
{
    public class AllSprings
    {
        public SpringRow[] springRows;

        public AllSprings(string[] readAllLinesInput, bool isPartTwo = false)
        {
            List<SpringRow> list = new();
            foreach (var item in readAllLinesInput)
            {
                if (isPartTwo) list.Add(new SpringRow(item, true));
                else list.Add(new SpringRow(item));
            }
            springRows = list.ToArray();
        }
        public AllSprings(SpringRow[] springRows)
        {
            this.springRows = springRows;
        }
    }
}
