namespace Tesseract.Abstractions
{
    public struct Scew : IEquatable<Scew>
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
            return this.Confidence == other.Confidence && this.Angle == other.Angle;
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