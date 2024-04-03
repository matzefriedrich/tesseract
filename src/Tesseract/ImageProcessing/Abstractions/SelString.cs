namespace Tesseract.ImageProcessing.Abstractions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Resources;

    public sealed class SelString
    {
        private static readonly HashSet<char> ValidChars = ['x', 'X', 'o', 'O', ' ', 'C'];

        /// <summary>
        ///     HMT (with just misses) for speckle up to 2x2
        ///     "oooo"
        ///     "oC o"
        ///     "o  o"
        ///     "oooo"
        /// </summary>
        public static readonly SelString Str2 = "oooooC oo  ooooo";

        /// <summary>
        ///     HMT (with just misses) for speckle up to 3x3
        ///     "oC  o"
        ///     "o   o"
        ///     "o   o"
        ///     "ooooo"
        /// </summary>
        public static readonly SelString Str3 = "ooooooC  oo   oo   oooooo";

        private readonly string s;

        private SelString(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) throw new ArgumentException(Resources.Value_cannot_be_null_or_whitespace, nameof(s));
            if (s.Any(c => ValidChars.Contains(c) == false)) throw new ArgumentException("The given string contains invalid chars.");
            this.s = s;
        }

        public override string ToString()
        {
            return this.s;
        }

        public static implicit operator string(SelString s)
        {
            return s.ToString();
        }

        public static implicit operator SelString(string s)
        {
            return new SelString(s);
        }
    }
}