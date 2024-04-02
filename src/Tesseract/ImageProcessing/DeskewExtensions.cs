namespace Tesseract.ImageProcessing
{
    using System;
    using Abstractions;
    using JetBrains.Annotations;

    public static class DeskewExtensions
    {
        // Skew Defaults
        public const int DefaultBinarySearchReduction = 2; // binary search part

        public const int DefaultBinaryThreshold = 130;

        /// <summary>
        ///     Determines the scew angle and if confidence is high enough returns the descewed image as the result, otherwise
        ///     returns clone of original image.
        /// </summary>
        /// <remarks>
        ///     This binarizes if necessary and finds the skew angle.  If the angle is large enough and there is sufficient
        ///     confidence, it returns a deskewed image; otherwise, it returns a clone.
        /// </remarks>
        /// <returns>Returns deskewed image if confidence was high enough, otherwise returns clone of original pix.</returns>
        public static Pix DeskewImage([NotNull] this SkewCorrector d, [NotNull] Pix source)
        {
            if (d == null) throw new ArgumentNullException(nameof(d));
            return d.DeskewImage(source, DefaultBinarySearchReduction, out Scew _);
        }

        /// <summary>
        ///     Determines the scew angle and if confidence is high enough returns the descewed image as the result, otherwise
        ///     returns clone of original image.
        /// </summary>
        /// <remarks>
        ///     This binarizes if necessary and finds the skew angle.  If the
        ///     angle is large enough and there is sufficient confidence,
        ///     it returns a deskewed image; otherwise, it returns a clone.
        /// </remarks>
        /// <param name="scew">The scew angle and confidence</param>
        /// <returns>Returns deskewed image if confidence was high enough, otherwise returns clone of original pix.</returns>
        public static Pix DeskewImage([NotNull] SkewCorrector d, [NotNull] Pix source, out Scew scew)
        {
            if (d == null) throw new ArgumentNullException(nameof(d));
            return d.DeskewImage(source, DefaultBinarySearchReduction, out scew);
        }

        /// <summary>
        ///     Determines the scew angle and if confidence is high enough returns the descewed image as the result, otherwise
        ///     returns clone of original image.
        /// </summary>
        /// <remarks>
        ///     This binarizes if necessary and finds the skew angle.  If the
        ///     angle is large enough and there is sufficient confidence,
        ///     it returns a deskewed image; otherwise, it returns a clone.
        /// </remarks>
        /// <param name="redSearch">The reduction factor used by the binary search, can be 1, 2, or 4.</param>
        /// <param name="scew">The scew angle and confidence</param>
        /// <returns>Returns deskewed image if confidence was high enough, otherwise returns clone of original pix.</returns>
        public static Pix DeskewImage([NotNull] this SkewCorrector d, Pix source, int redSearch, out Scew scew)
        {
            if (d == null) throw new ArgumentNullException(nameof(d));
            return d.DeskewImage(source, ScewSweep.Default, redSearch, DefaultBinaryThreshold, out scew);
        }
    }
}