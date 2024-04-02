namespace Tesseract.ImageProcessing
{
    using System;
    using Abstractions;
    using Interop.Abstractions;
    using JetBrains.Annotations;

    public class SkewCorrector
    {
        private readonly ILeptonicaApiSignatures leptonicaApi;

        public SkewCorrector([NotNull] ILeptonicaApiSignatures leptonicaApi)
        {
            this.leptonicaApi = leptonicaApi ?? throw new ArgumentNullException(nameof(leptonicaApi));
        }

        /// <summary>
        ///     Determines the scew angle and if confidence is high enough returns the descewed image as the result, otherwise
        ///     returns clone of original image.
        /// </summary>
        /// <remarks>
        ///     This binarizes if necessary and finds the skew angle.  If the angle is large enough and there is sufficient
        ///     confidence, it returns a deskewed image; otherwise, it returns a clone.
        /// </remarks>
        /// <param name="source">The image to deskew.</param>
        /// <param name="sweep">linear sweep parameters</param>
        /// <param name="redSearch">The reduction factor used by the binary search, can be 1, 2, or 4.</param>
        /// <param name="thresh">The threshold value used for binarizing the image.</param>
        /// <param name="scew">The scew angle and confidence</param>
        /// <returns>Returns deskewed image if confidence was high enough, otherwise returns clone of original pix.</returns>
        public Pix DeskewImage([NotNull] Pix source, ScewSweep sweep, int redSearch, int thresh, out Scew scew)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            IntPtr resultPixHandle = this.leptonicaApi.pixDeskewGeneral(source.Handle, sweep.Reduction, sweep.Range, sweep.Delta, redSearch, thresh, out float pAngle, out float pConf);
            if (resultPixHandle == IntPtr.Zero) throw new TesseractException("Failed to deskew image.");
            scew = new Scew(pAngle, pConf);
            return new Pix(this.leptonicaApi, resultPixHandle);
        }
    }
}