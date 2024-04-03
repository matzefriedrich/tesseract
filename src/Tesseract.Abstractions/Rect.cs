namespace Tesseract.Abstractions
{
    public readonly struct Rect(int x, int y, int width, int height)
        : IEquatable<Rect>
    {
        public static readonly Rect Empty = new();

        public static Rect FromCoords(int x1, int y1, int x2, int y2)
        {
            return new Rect(x1, y1, x2 - x1, y2 - y1);
        }

        public int X1 { get; } = x;

        public int Y1 { get; } = y;

        public int X2 => this.X1 + this.Width;

        public int Y2 => this.Y1 + this.Height;

        public int Width { get; } = width;

        public int Height { get; } = height;

        public override bool Equals(object? obj)
        {
            return obj is Rect rect && this.Equals(rect);
        }

        public bool Equals(Rect other)
        {
            return this.X1 == other.X1 && this.Y1 == other.Y1 && this.Width == other.Width && this.Height == other.Height;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.X1, this.Y1, this.Width, this.Height);
        }

        public static bool operator ==(Rect lhs, Rect rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Rect lhs, Rect rhs)
        {
            return !(lhs == rhs);
        }

        public override string ToString()
        {
            return $"[Rect X={this.X1}, Y={this.Y1}, Width={this.Width}, Height={this.Height}]";
        }
    }
}