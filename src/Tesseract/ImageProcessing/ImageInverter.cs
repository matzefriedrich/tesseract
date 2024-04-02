namespace Tesseract.ImageProcessing
{
    using System;
    using System.Runtime.InteropServices;
    using Abstractions;
    using Interop.Abstractions;
    using JetBrains.Annotations;

    public class ImageInverter
    {
        private readonly ILeptonicaApiSignatures leptonicaApi;

        public ImageInverter([NotNull] ILeptonicaApiSignatures leptonicaApi)
        {
            this.leptonicaApi = leptonicaApi ?? throw new ArgumentNullException(nameof(leptonicaApi));
        }

        /// <summary>
        ///     Inverts pix for all pixel depths.
        /// </summary>
        /// <returns></returns>
        public Pix InvertImage([NotNull] Pix source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var pixd = new HandleRef(this, IntPtr.Zero);
            IntPtr resultHandle = this.leptonicaApi.pixInvert(pixd, source.Handle);

            if (resultHandle == IntPtr.Zero) throw new LeptonicaException("Failed to invert image.");
            return new Pix(this.leptonicaApi, resultHandle);
        }
    }
}