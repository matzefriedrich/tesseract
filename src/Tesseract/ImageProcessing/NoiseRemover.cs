namespace Tesseract.ImageProcessing
{
    using System;
    using System.Runtime.InteropServices;
    using Abstractions;
    using Interop.Abstractions;
    using JetBrains.Annotations;

    public class NoiseRemover
    {
        private readonly ILeptonicaApiSignatures leptonicaApi;
        
        public NoiseRemover([NotNull] ILeptonicaApiSignatures leptonicaApi)
        {
            this.leptonicaApi = leptonicaApi ?? throw new ArgumentNullException(nameof(leptonicaApi));
        }

        /// <summary>
        ///     Reduces speckle noise in image. The algorithm is based on Leptonica <code>speckle_reg.c</code> example
        ///     demonstrating morphological method of removing speckle.
        /// </summary>
        /// <param name="selStr">hit-miss sels in 2D layout; SEL_STR2 and SEL_STR3 are predefined values</param>
        /// <param name="selSize">2 for 2x2, 3 for 3x3</param>
        /// <returns>Returns a new <see cref="Pix"/> object.</returns>
        public Pix ReduceSpeckleNoise([NotNull] Pix source, SelString selStr, int selSize)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            /*  Normalize for rapidly varying background */
            IntPtr pix1 = this.leptonicaApi.pixBackgroundNormFlex(source.Handle, 7, 7, 1, 1, 10);

            /* Remove the background */
            IntPtr pix2 = this.leptonicaApi.pixGammaTRCMasked(new HandleRef(this, IntPtr.Zero), new HandleRef(this, pix1), new HandleRef(this, IntPtr.Zero), 1.0f, 100, 175);

            /* Binarize */
            IntPtr pix3 = this.leptonicaApi.pixThresholdToBinary(new HandleRef(this, pix2), 180);

            /* Remove the speckle noise up to selSize x selSize */
            IntPtr sel1 = this.leptonicaApi.selCreateFromString(selStr, selSize + 2, selSize + 2, "speckle" + selSize);
            IntPtr pix4 = this.leptonicaApi.pixHMT(new HandleRef(this, IntPtr.Zero), new HandleRef(this, pix3), new HandleRef(this, sel1));
            IntPtr sel2 = this.leptonicaApi.selCreateBrick(selSize, selSize, 0, 0, SelType.SEL_HIT);
            IntPtr pix5 = this.leptonicaApi.pixDilate(new HandleRef(this, IntPtr.Zero), new HandleRef(this, pix4), new HandleRef(this, sel2));
            IntPtr pix6 = this.leptonicaApi.pixSubtract(new HandleRef(this, IntPtr.Zero), new HandleRef(this, pix3), new HandleRef(this, pix5));

            this.leptonicaApi.selDestroy(ref sel1);
            this.leptonicaApi.selDestroy(ref sel2);

            this.leptonicaApi.pixDestroy(ref pix1);
            this.leptonicaApi.pixDestroy(ref pix2);
            this.leptonicaApi.pixDestroy(ref pix3);
            this.leptonicaApi.pixDestroy(ref pix4);
            this.leptonicaApi.pixDestroy(ref pix5);

            if (pix6 == IntPtr.Zero) throw new TesseractException("Failed to despeckle image.");

            return new Pix(this.leptonicaApi, pix6);
        }
    }
}