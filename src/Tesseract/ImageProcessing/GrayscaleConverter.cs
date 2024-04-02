namespace Tesseract.ImageProcessing
{
    using System;
    using Abstractions;
    using Interop.Abstractions;
    using JetBrains.Annotations;

    public class GrayscaleConverter
    {
        private readonly ILeptonicaApiSignatures leptonicaApi;

        public GrayscaleConverter([NotNull] ILeptonicaApiSignatures leptonicaApi)
        {
            this.leptonicaApi = leptonicaApi ?? throw new ArgumentNullException(nameof(leptonicaApi));
        }

        /// <summary>
        ///     Conversion from RBG to 8bpp grayscale using the specified weights. Note red, green, blue weights should add up to
        ///     1.0.
        /// </summary>
        /// <param name="rwt">Red weight</param>
        /// <param name="gwt">Green weight</param>
        /// <param name="bwt">Blue weight</param>
        /// <returns>The Grayscale pix.</returns>
        public Pix ConvertRgbToGray(Pix source, float rwt = 0, float gwt = 0, float bwt = 0)
        {
            if (source.Depth != 32) throw new InvalidOperationException("The source image must have a depth of 32 (32 bpp).");
            if (rwt < 0) throw new ArgumentException("All weights must be greater than or equal to zero; red was not.");
            if (gwt < 0) throw new ArgumentException("All weights must be greater than or equal to zero; green was not.");
            if (bwt < 0) throw new ArgumentException("All weights must be greater than or equal to zero; blue was not.");

            IntPtr resultPixHandle = this.leptonicaApi.pixConvertRGBToGray(source.Handle, rwt, gwt, bwt);
            if (resultPixHandle == IntPtr.Zero) throw new TesseractException("Failed to convert to grayscale.");
            return new Pix(this.leptonicaApi, resultPixHandle);
        }
    }
}