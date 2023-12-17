using System.Reflection.Metadata.Ecma335;

namespace Day10
{
    public struct Vector2 : IEquatable<Vector2>
    {
        public float x, y;
        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(Vector2 other) => this == other;

        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);
        public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);

        public static Vector2 operator *(Vector2 a, float b) => new Vector2(a.x * b, a.y * b);
        public static Vector2 operator *(float a, Vector2 b) => new Vector2(b.x * a, b.y * a);
        public static Vector2 operator /(Vector2 a, float b)
        {
            if (b == 0)
                throw new DivideByZeroException();
            return new Vector2(a.x / b, a.y / b);
        }

        public static bool operator ==(Vector2 a, Vector2 b) => (a.x == b.x) && (a.y == b.y);
        public static bool operator !=(Vector2 a, Vector2 b) => (a.x != b.x) || (a.y != b.y);

        public override bool Equals(object? obj) => obj is Vector2 coord && Equals(coord);

        public readonly static Vector2 zero = new Vector2(0, 0);

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }

        public override readonly string? ToString() => $"({x},{y})";




        public Vector2 PerpendicularClockwise() => new Vector2(y, -x);
        public Vector2 PerpendicularCounterClockwise() => new Vector2(-y, x);
        public static Vector2 PerpendicularClockwise(Vector2 v) => new Vector2(v.y, -v.x);
        public static Vector2 PerpendicularCounterClockwise(Vector2 v) => new Vector2(-v.y, v.x);
    }
}
