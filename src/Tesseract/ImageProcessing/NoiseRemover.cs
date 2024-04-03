namespace Tesseract.ImageProcessing
{
    using System;
    using System.Runtime.InteropServices;
    using Abstractions;
    using Interop;
    using Interop.Abstractions;
    using Resources;
    using Tesseract.Abstractions;

    public class NoiseRemover : INoiseRemover
    {
        private readonly ILeptonicaApiSignatures leptonicaApi;

        public NoiseRemover(ILeptonicaApiSignatures leptonicaApi)
        {
            this.leptonicaApi = leptonicaApi ?? throw new ArgumentNullException(nameof(leptonicaApi));
        }

        /// <inheritdoc />
        public Pix ReduceSpeckleNoise(Pix source, SelString selStr, int selSize)
        {
            ArgumentNullException.ThrowIfNull(source);

            /*  Normalize for rapidly varying background */
            IntPtr pix1 = this.leptonicaApi.pixBackgroundNormFlex(source.Handle, 7, 7, 1, 1, 10);

            /* Remove the background */
            IntPtr pix2 = this.leptonicaApi.pixGammaTRCMasked(new HandleRef(this, IntPtr.Zero), new HandleRef(this, pix1), new HandleRef(this, IntPtr.Zero), 1.0f, 100, 175);

            /* Binarize */
            IntPtr pix3 = this.leptonicaApi.pixThresholdToBinary(new HandleRef(this, pix2), 180);

            /* Remove the speckle noise up to selSize x selSize */
            IntPtr sel1 = this.leptonicaApi.selCreateFromString(selStr, selSize + 2, selSize + 2, "speckle" + selSize);
            IntPtr pix4 = this.leptonicaApi.pixHMT(new HandleRef(this, IntPtr.Zero), new HandleRef(this, pix3), new HandleRef(this, sel1));
            IntPtr sel2 = this.leptonicaApi.selCreateBrick(selSize, selSize, 0, 0, SelType.SelHit);
            IntPtr pix5 = this.leptonicaApi.pixDilate(new HandleRef(this, IntPtr.Zero), new HandleRef(this, pix4), new HandleRef(this, sel2));
            IntPtr pix6 = this.leptonicaApi.pixSubtract(new HandleRef(this, IntPtr.Zero), new HandleRef(this, pix3), new HandleRef(this, pix5));

            this.leptonicaApi.selDestroy(ref sel1);
            this.leptonicaApi.selDestroy(ref sel2);

            this.leptonicaApi.pixDestroy(ref pix1);
            this.leptonicaApi.pixDestroy(ref pix2);
            this.leptonicaApi.pixDestroy(ref pix3);
            this.leptonicaApi.pixDestroy(ref pix4);
            this.leptonicaApi.pixDestroy(ref pix5);

            if (pix6 == IntPtr.Zero) throw new TesseractException(Resources.NoiseRemover_ReduceSpeckleNoise_Failed_to_despeckle_image_);

            return new Pix(this.leptonicaApi, pix6);
        }
    }
}