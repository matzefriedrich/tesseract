namespace Tesseract.ImageProcessing
{
    using System;
    using Abstractions;
    using Interop.Abstractions;
    using Resources;
    using Tesseract.Abstractions;

    public class ColorQuantizer : IColorQuantizer
    {
        private readonly ILeptonicaApiSignatures leptonicaApi;

        public ColorQuantizer(ILeptonicaApiSignatures leptonicaApi)
        {
            this.leptonicaApi = leptonicaApi ?? throw new ArgumentNullException(nameof(leptonicaApi));
        }

        /// <inheritdoc />
        public Pix ConvertTo8Bpp(Pix source, int colorMapFlag)
        {
            IntPtr resultHandle = this.leptonicaApi.pixConvertTo8(source.Handle, colorMapFlag);

            if (resultHandle == IntPtr.Zero) throw new LeptonicaException(Resources.ColorQuantizer_ConvertTo8Bpp_Failed_to_convert_image_to_8_bpp_);
            return new Pix(this.leptonicaApi, resultHandle);
        }
    }
}