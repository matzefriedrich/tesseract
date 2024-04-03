namespace Tesseract.ImageProcessing
{
    using System;
    using Abstractions;
    using Interop.Abstractions;
    using Resources;
    using Tesseract.Abstractions;

    public class GrayscaleConverter : IGrayscaleConverter
    {
        private readonly ILeptonicaApiSignatures leptonicaApi;

        public GrayscaleConverter(ILeptonicaApiSignatures leptonicaApi)
        {
            this.leptonicaApi = leptonicaApi ?? throw new ArgumentNullException(nameof(leptonicaApi));
        }

        /// <inheritdoc />
        public Pix ConvertRgbToGray(Pix source, float weightRed = 0, float weightGreen = 0, float weightBlue = 0)
        {
            if (source.Depth != 32) throw new InvalidOperationException("The source image must have a depth of 32 bits per pixel.");
            if (weightRed < 0) throw new ArgumentException(string.Format(Resources.GrayscaleConverter_ConvertRgbToGray_All_weights_must_be_greater_than_or_equal_to_zero___0__was_not_, nameof(weightRed)), nameof(weightRed));
            if (weightGreen < 0) throw new ArgumentException(string.Format(Resources.GrayscaleConverter_ConvertRgbToGray_All_weights_must_be_greater_than_or_equal_to_zero___0__was_not_, nameof(weightGreen)), nameof(weightGreen));
            if (weightBlue < 0) throw new ArgumentException(string.Format(Resources.GrayscaleConverter_ConvertRgbToGray_All_weights_must_be_greater_than_or_equal_to_zero___0__was_not_, nameof(weightBlue)), nameof(weightBlue));

            IntPtr resultPixHandle = this.leptonicaApi.pixConvertRGBToGray(source.Handle, weightRed, weightGreen, weightBlue);
            if (resultPixHandle == IntPtr.Zero) throw new TesseractException(Resources.GrayscaleConverter_ConvertRgbToGray_Failed_to_convert_to_grayscale_);
            return new Pix(this.leptonicaApi, resultPixHandle);
        }
    }
}