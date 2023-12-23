using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day14
{
    public class Rock
    {
        public RockType type;

        public Rock(RockType type)
        {
            this.type = type;
        }

        public override bool Equals(object? obj)
        {
            Rock? r = obj as Rock;
            if (r == null) return false;
            return type == r.type;
        }

        public override int GetHashCode()
        {
            return type.GetHashCode();
        }

        public override string? ToString()
        {
            return ((char)type).ToString();
        }
    }

    public enum RockType
    {
        None = '.',
        Stationary = '#',
        Rolling = 'O'
    }
}