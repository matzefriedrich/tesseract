namespace Tesseract.ImageProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;
    using Resources;

    public sealed class SelString
    {
        private readonly string s;
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

        private SelString([NotNull] string s)
        {
            if (string.IsNullOrWhiteSpace(s)) throw new ArgumentException(Resources.Value_cannot_be_null_or_whitespace, nameof(s));
            if (s.Any(c => ValidChars.Contains(c) == false)) throw new ArgumentException("The given string contains invalid chars.");
            this.s = s;
        }

        public override string ToString()
        {
            return this.s;
        }

        public static implicit operator string(SelString s) => s.ToString();
        public static implicit operator SelString(string s) => new(s);
    }
}