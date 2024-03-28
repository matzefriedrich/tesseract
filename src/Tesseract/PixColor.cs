﻿namespace Tesseract
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PixColor : IEquatable<PixColor>
    {
        public PixColor(byte red, byte green, byte blue, byte alpha = 255)
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
            this.Alpha = alpha;
        }

        public byte Red { get; }

        public byte Green { get; }

        public byte Blue { get; }

        public byte Alpha { get; }

        public static PixColor FromRgba(uint value)
        {
            return new PixColor(
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)(value & 0xFF));
        }

        public static PixColor FromRgb(uint value)
        {
            return new PixColor(
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 8) & 0xFF));
        }

        public uint ToRGBA()
        {
            return (uint)((this.Red << 24) |
                          (this.Green << 16) |
                          (this.Blue << 8) | this.Alpha);
        }

        public static explicit operator Color(PixColor color)
        {
            return Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
        }

        public static explicit operator PixColor(Color color)
        {
            return new PixColor(color.R, color.G, color.B, color.A);
        }


        #region Equals and GetHashCode implementation

        public override bool Equals(object obj)
        {
            return obj is PixColor && this.Equals((PixColor)obj);
        }

        public bool Equals(PixColor other)
        {
            return this.Red == other.Red && this.Blue == other.Blue && this.Green == other.Green && this.Alpha == other.Alpha;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            unchecked
            {
                hashCode += 1000000007 * this.Red.GetHashCode();
                hashCode += 1000000009 * this.Blue.GetHashCode();
                hashCode += 1000000021 * this.Green.GetHashCode();
                hashCode += 1000000033 * this.Alpha.GetHashCode();
            }

            return hashCode;
        }

        public static bool operator ==(PixColor lhs, PixColor rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(PixColor lhs, PixColor rhs)
        {
            return !(lhs == rhs);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Color(0x{0:X})", this.ToRGBA());
        }
    }
}