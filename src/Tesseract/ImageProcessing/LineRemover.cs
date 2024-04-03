namespace Tesseract.ImageProcessing
{
    using System;
    using System.Runtime.InteropServices;
    using Abstractions;
    using Interop.Abstractions;
    using Resources;
    using Tesseract.Abstractions;

    public class LineRemover : ILineRemover
    {
        public const float Deg2Rad = (float)(Math.PI / 180.0);

        private readonly ILeptonicaApiSignatures leptonicaApi;

        public LineRemover(ILeptonicaApiSignatures leptonicaApi)
        {
            this.leptonicaApi = leptonicaApi ?? throw new ArgumentNullException(nameof(leptonicaApi));
        }

        /// <inheritdoc />
        public Pix RemoveHorizontalLines(Pix source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            IntPtr pix2, pix3, pix4, pix5, pix6, pix7;
            IntPtr pix9;

            IntPtr pix1 = pix2 = pix3 = pix4 = pix5 = pix6 = pix7 = pix9 = IntPtr.Zero;

            try
            {
                /* threshold to binary, extracting much of the lines */
                pix1 = this.leptonicaApi.pixThresholdToBinary(source.Handle, 170);

                /* find the skew angle and deskew using an interpolated rotator for anti-aliasing (to avoid jaggies) */
                this.leptonicaApi.pixFindSkew(new HandleRef(this, pix1), out float angle, out float _);
                pix2 = this.leptonicaApi.pixRotateAMGray(source.Handle, Deg2Rad * angle, 255);

                /* extract the lines to be removed */
                pix3 = this.leptonicaApi.pixCloseGray(new HandleRef(this, pix2), 51, 1);

                /* solidify the lines to be removed */
                pix4 = this.leptonicaApi.pixErodeGray(new HandleRef(this, pix3), 1, 5);

                /* clean the background of those lines */
                pix5 = this.leptonicaApi.pixThresholdToValue(new HandleRef(this, IntPtr.Zero), new HandleRef(this, pix4), 210, 255);

                pix6 = this.leptonicaApi.pixThresholdToValue(new HandleRef(this, IntPtr.Zero), new HandleRef(this, pix5), 200, 0);

                /* get paint-through mask for changed pixels */
                pix7 = this.leptonicaApi.pixThresholdToBinary(new HandleRef(this, pix6), 210);

                /* add the inverted, cleaned lines to orig.  Because
                 * the background was cleaned, the inversion is 0,
                 * so when you add, it doesn't lighten those pixels.
                 * It only lightens (to white) the pixels in the lines! */
                this.leptonicaApi.pixInvert(new HandleRef(this, pix6), new HandleRef(this, pix6));
                IntPtr pix8 = this.leptonicaApi.pixAddGray(new HandleRef(this, IntPtr.Zero), new HandleRef(this, pix2), new HandleRef(this, pix6));

                pix9 = this.leptonicaApi.pixOpenGray(new HandleRef(this, pix8), 1, 9);

                this.leptonicaApi.pixCombineMasked(new HandleRef(this, pix8), new HandleRef(this, pix9), new HandleRef(this, pix7));
                if (pix8 == IntPtr.Zero) throw new TesseractException(Resources.LineRemover_RemoveHorizontalLines_Failed_to_remove_lines_from_image_);

                return new Pix(this.leptonicaApi, pix8);
            }
            finally
            {
                // destroy any created intermediate pix's, regardless of if the process 
                // failed for any reason.
                if (pix1 != IntPtr.Zero) this.leptonicaApi.pixDestroy(ref pix1);

                if (pix2 != IntPtr.Zero) this.leptonicaApi.pixDestroy(ref pix2);

                if (pix3 != IntPtr.Zero) this.leptonicaApi.pixDestroy(ref pix3);

                if (pix4 != IntPtr.Zero) this.leptonicaApi.pixDestroy(ref pix4);

                if (pix5 != IntPtr.Zero) this.leptonicaApi.pixDestroy(ref pix5);

                if (pix6 != IntPtr.Zero) this.leptonicaApi.pixDestroy(ref pix6);

                if (pix7 != IntPtr.Zero) this.leptonicaApi.pixDestroy(ref pix7);

                if (pix9 != IntPtr.Zero) this.leptonicaApi.pixDestroy(ref pix9);
            }
        }
    }
}