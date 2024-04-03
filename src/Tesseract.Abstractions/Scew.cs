namespace Tesseract.Abstractions
{
    public readonly struct Scew : IEquatable<Scew>
    {
        public Scew(float angle, float confidence)
        {
            this.Angle = angle;
            this.Confidence = confidence;
        }

        public float Angle { get; }

        public float Confidence { get; }

        public override string ToString()
        {
            return $"Scew: {this.Angle} [conf: {this.Confidence}]";
        }

        public override bool Equals(object? obj)
        {
            return obj is Scew scew && this.Equals(scew);
        }

        public bool Equals(Scew other)
        {
            const float confidencePrecisionThreshold = 0.001f;
            const float anglePrecisionThreshold = 0.001f;

            return Math.Abs(this.Confidence - other.Confidence) < confidencePrecisionThreshold
                   && Math.Abs(this.Angle - other.Angle) < anglePrecisionThreshold;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Angle, this.Confidence);
        }

        public static bool operator ==(Scew lhs, Scew rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Scew lhs, Scew rhs)
        {
            return !(lhs == rhs);
        }
    }
}