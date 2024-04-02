namespace Tesseract.ImageProcessing
{
    using System;
    using Abstractions;
    using Interop.Abstractions;
    using JetBrains.Annotations;

    public class ColorQuantizer
    {
        private readonly ILeptonicaApiSignatures leptonicaApi;

        public ColorQuantizer([NotNull] ILeptonicaApiSignatures leptonicaApi)
        {
            this.leptonicaApi = leptonicaApi ?? throw new ArgumentNullException(nameof(leptonicaApi));
        }

        /// <summary>
        ///     Top-level conversion to 8 bpp.
        /// </summary>
        /// <param name="source">The source image to convert.</param>
        /// <param name="colorMapFlag"></param>
        /// <returns></returns>
        public Pix ConvertTo8(Pix source, int colorMapFlag)
        {
            IntPtr resultHandle = this.leptonicaApi.pixConvertTo8(source.Handle, colorMapFlag);

            if (resultHandle == IntPtr.Zero) throw new LeptonicaException("Failed to convert image to 8 bpp.");
            return new Pix(this.leptonicaApi, resultHandle);
        }
    }
}