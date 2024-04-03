namespace Tesseract.ImageProcessing
{
    using System;
    using System.Runtime.InteropServices;
    using Abstractions;
    using Interop.Abstractions;
    using Resources;
    using Tesseract.Abstractions;

    public class ImageInverter : IImageInverter
    {
        private readonly ILeptonicaApiSignatures leptonicaApi;

        public ImageInverter(ILeptonicaApiSignatures leptonicaApi)
        {
            this.leptonicaApi = leptonicaApi ?? throw new ArgumentNullException(nameof(leptonicaApi));
        }

        /// <inheritdoc />
        public Pix InvertImage(Pix source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var pixd = new HandleRef(this, IntPtr.Zero);
            IntPtr resultHandle = this.leptonicaApi.pixInvert(pixd, source.Handle);

            if (resultHandle == IntPtr.Zero) throw new LeptonicaException(Resources.ImageInverter_InvertImage_Failed_to_invert_image_);
            return new Pix(this.leptonicaApi, resultHandle);
        }
    }
}