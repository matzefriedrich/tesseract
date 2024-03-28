namespace Tesseract
{
    public struct Scew
    {
        public Scew(float angle, float confidence)
        {
            this.Angle = angle;
            this.Confidence = confidence;
        }

        public float Angle { get; }


        public float Confidence { get; }

        #region ToString

        public override string ToString()
        {
            return string.Format("Scew: {0} [conf: {1}]", this.Angle, this.Confidence);
        }

        #endregion

        #region Equals and GetHashCode implementation

        public override bool Equals(object obj)
        {
            return obj is Scew && this.Equals((Scew)obj);
        }

        public bool Equals(Scew other)
        {
            return this.Confidence == other.Confidence && this.Angle == other.Angle;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            unchecked
            {
                hashCode += 1000000007 * this.Angle.GetHashCode();
                hashCode += 1000000009 * this.Confidence.GetHashCode();
            }

            return hashCode;
        }

        public static bool operator ==(Scew lhs, Scew rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Scew lhs, Scew rhs)
        {
            return !(lhs == rhs);
        }

        #endregion
    }
}