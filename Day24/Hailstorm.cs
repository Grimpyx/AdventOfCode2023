using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Day24
{
    class Hailstorm
    {
        private HashSet<Hail3D> hailHashset = new HashSet<Hail3D>();

        public Hailstorm(HashSet<Hail3D> hail)
        {
            this.hailHashset = hail;
        }

        public HashSet<Hail3D> HailHashset { get => hailHashset; }

        public Dictionary<Hail3D, List<(Hail3D other, (decimal x, decimal y) intersectionPoint, bool isInsideTestArea)>> GetIntersectionsXY(Vector2 testAreaMinMax)
        {
            // The result to be returned by this method
            Dictionary<Hail3D, List<(Hail3D other, (decimal x, decimal y) intersectionPoint, bool isInsideTestArea)>> result = [];

            // Because we can't loop through a hashset with indices, so we make an array of the elements
            Hail3D[] hs = [.. hailHashset];

            for (int i = 0; i < hs.Length; i++)
            {
                Hail3D thisHail = hs[i];

                // We start counting from i+1 so we dont compare with any previously checked, because a.Intersect(b) == b.Intersect(a)
                for (int j = i + 1; j < hs.Length; j++)
                {
                    Hail3D otherHail = hs[j];

                    if (thisHail.IntersectsPathWithXYWithinArea(otherHail, testAreaMinMax,
                        out (decimal x, decimal y) ip, out bool isInside, out _))
                    {
                        if (!result.ContainsKey(thisHail))
                            result.Add(thisHail, []);

                        result[thisHail].Add((otherHail, ip, isInside));
                    }
                }
            }
            return result;
        }
    }

    class Hail3D(Decimal3 position, Decimal3 velocity)
    {
        private Decimal3 position = position; //  x  y  z
        private Decimal3 velocity = velocity; // kx ky kz

        public Decimal3 Velocity { get => velocity; }
        public Decimal3 Position { get => position; }


        public bool IntersectsPathWithXYWithinArea(Hail3D otherHail, Vector2 testAreaMinMax, out (decimal x, decimal y) intersectionPoint, out bool isInsideTestArea, out decimal intersectionTime)
        {
            isInsideTestArea = false;
            intersectionPoint = default;
            intersectionTime = -1;

            if (!IntersectsPathWithXY(otherHail, out (decimal x, decimal y) ixy, out decimal time)) return false;
            intersectionTime = time;
            intersectionPoint = ixy;

            /* Unnecessary check
            var ty1 = (y - (decimal)thisHail.position.Y) / (decimal)thisHail.velocity.Y;   // vector from start to intersection point, divide by direction to get >1 if it follows the direction, <1 if it doesnt
            var ty2 = (y - (decimal)otherHail.position.Y) / (decimal)otherHail.velocity.Y;
            if (ty1 < 0 || ty2 < 0)
                return false;*/

            // Check if inside test area
            if (ixy.x >= (decimal)testAreaMinMax.X && ixy.x <= (decimal)testAreaMinMax.Y
                && ixy.y >= (decimal)testAreaMinMax.X && ixy.y <= (decimal)testAreaMinMax.Y)
                isInsideTestArea = true;

            return true;
        }

        public bool IntersectsPathWithXY_velocityOffset(Hail3D otherHail, (decimal dvx, decimal dvy) dv, out (decimal x, decimal y) intersectionPoint, out decimal intersectionTime)
        {
            Hail3D a = new(position, velocity + new Decimal3(dv.dvx, dv.dvy, 0));
            Hail3D b = new(otherHail.Position, otherHail.Velocity + new Decimal3(dv.dvx, dv.dvy, 0));

            intersectionPoint = default;
            intersectionTime = -1;
            if (!a.IntersectsPathWithXY(b, out var ip, out decimal iTime)) return false;
            intersectionPoint = ip;
            intersectionTime = iTime;
            return true;
        }

        public bool IntersectsPathWithXY(Hail3D otherHail, out (decimal x, decimal y) intersectionPoint, out decimal intersectionTime)
        {
            Hail3D thisHail = this;
            intersectionPoint = default;
            intersectionTime = -1;

            // if we have two lines
            // y-y1=k1(x-x1)
            // y-y2=k2(x-x2)
            // we can calculate the x-value where they intersect by subtracting the left sides and right sides respectively.
            // (y-y1)-(y-y2) = (k1(x-x1))-(k2(x-x2))
            // Simplifying leaves us with
            // x = (y1-y2+k2*x2-k1*x1) / (k2-k1)
            // If this value "x" exists, then they intersect.
            decimal x1 = thisHail.position.X;
            decimal y1 = thisHail.position.Y;
            decimal x2 = otherHail.position.X;
            decimal y2 = otherHail.position.Y;

            if (thisHail.velocity.X == 0) return false;
            if (otherHail.velocity.X == 0) return false;

            // k represents how many steps y increases per step x
            decimal k1 = thisHail.velocity.Y / thisHail.velocity.X;
            decimal k2 = otherHail.velocity.Y / otherHail.velocity.X;

            // k1 can not be equal to k2, else our formula divides by zero
            if (k1 == k2) return false;

            // Our formula
            decimal x = (y1 - y2 + (k2 * x2) - (k1 * x1)) / (k2 - k1);

            // If we insert this x into any of the lines we get the y value for the intersection
            decimal y = y1 + (k1 * x) - (k1 * x1);

            // Check if intersection is back in time, if so then return false
            // y = y0 + t*vy
            // ( y - y0 ) / vy = t
            decimal tx1 = (x - x1);
            tx1 /= thisHail.velocity.X;
            decimal tx2 = (x - x2);
            tx2 /= otherHail.velocity.X;

            //decimal tx1 = (x - thisHail.position.X) / thisHail.velocity.X;   // vector from start to intersection point, divide by direction to get >1 if it follows the direction, <1 if it doesnt
            //decimal tx2 = (x - otherHail.position.X) / otherHail.velocity.X;
            //if (tx1 < 0 || tx2 < 0)
            if (tx1 < 0 || tx2 < 0)
                return false;

            intersectionPoint = (Math.Round(x, 3), Math.Round(y, 3));
            intersectionTime = Math.Round(tx2, 3);

            return true;
        }

        public override bool Equals(object? obj)
        {
            return obj is Hail3D d &&
                   position.Equals(d.position) &&
                   velocity.Equals(d.velocity);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(position, velocity);
        }

        public override string? ToString()
        {
            return $"(X:{position.X:e}, Y:{position.Y:e}, Z:{position.Z:e})";
        }
    }

    record Decimal3(decimal X, decimal Y, decimal Z)
    {
        public static Decimal3 operator +(Decimal3 a, Decimal3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }
}
