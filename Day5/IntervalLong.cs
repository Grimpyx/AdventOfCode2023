using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Day5
{
    public class IntervalLong
    {
        public long lower;
        public long upper;

        public IntervalLong(long lower, long upper)
        {
            this.lower = lower;
            this.upper = upper;
        }

        public bool IsBetweenIncluded(long value) => (value >= lower && value <= upper);
        public bool IsBelowInterval(long value) => value < lower;
        public bool IsOverInterval(long value) => value > upper;

        public IntervalLong? ClampInterval(IntervalLong otherInterval)
        {
            long newLower = Math.Clamp(otherInterval.lower, lower, upper);
            long newUpper = Math.Clamp(otherInterval.upper, lower, upper);
            if (newLower == newUpper || newLower > newUpper) return null;
            else return new IntervalLong(newLower, newUpper);
        }
    }
}
