namespace Tesseract.Abstractions
{
    using System.Drawing;
    using System.Runtime.InteropServices;

    using JetBrains.Annotations;

    [NoReorder]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct PixColor(byte red, byte green, byte blue, byte alpha = 255)
        : IEquatable<PixColor>
    {
        public byte Red { get; } = red;

        public byte Green { get; } = green;

        public byte Blue { get; } = blue;

        public byte Alpha { get; } = alpha;

        public static PixColor FromRgba(uint value)
        {
            var red = (byte)((value >> 24) & 0xFF);
            var green = (byte)((value >> 16) & 0xFF);
            var blue = (byte)((value >> 8) & 0xFF);
            var alpha = (byte)(value & 0xFF);

            return new PixColor(red, green, blue, alpha);
        }

        public static PixColor FromRgb(uint value)
        {
            var red = (byte)((value >> 24) & 0xFF);
            var green = (byte)((value >> 16) & 0xFF);
            var blue = (byte)((value >> 8) & 0xFF);

            return new PixColor(red, green, blue);
        }

        public uint ToRGBA()
        {
            return (uint)((this.Red << 24) | (this.Green << 16) | (this.Blue << 8) | this.Alpha);
        }

        public static explicit operator Color(PixColor color)
        {
            return Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
        }

        public static explicit operator PixColor(Color color)
        {
            return new PixColor(color.R, color.G, color.B, color.A);
        }

        public override bool Equals(object? obj)
        {
            return obj is PixColor color && this.Equals(color);
        }

        public bool Equals(PixColor other)
        {
            return this.Red == other.Red && this.Blue == other.Blue && this.Green == other.Green && this.Alpha == other.Alpha;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Red, this.Green, this.Blue, this.Alpha);
        }

        public static bool operator ==(PixColor lhs, PixColor rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(PixColor lhs, PixColor rhs)
        {
            return !(lhs == rhs);
        }

        public override string ToString()
        {
            return $"Color(0x{this.ToRGBA():X})";
        }
    }
}