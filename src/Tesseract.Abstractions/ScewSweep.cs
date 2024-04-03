﻿namespace Tesseract.Abstractions
{
    /// <summary>
    ///     Represents the parameters for a sweep search used by scew algorithms.
    /// </summary>
    public struct ScewSweep
    {
        public static readonly ScewSweep Default = new(DefaultReduction);

        private const int DefaultReduction = 4; // Sweep part; 4 is good
        private const float DefaultRange = 7.0F;
        private const float DefaultDelta = 1.0F;

        public ScewSweep(int reduction = DefaultReduction, float range = DefaultRange, float delta = DefaultDelta)
        {
            this.Reduction = reduction;
            this.Range = range;
            this.Delta = delta;
        }

        public int Reduction { get; }

        public float Range { get; }

        public float Delta { get; }
    }
}