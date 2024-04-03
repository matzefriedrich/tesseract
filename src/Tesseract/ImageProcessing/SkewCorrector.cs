namespace Tesseract.ImageProcessing
{
    using System;
    using Abstractions;
    using Interop.Abstractions;
    using Resources;
    using Tesseract.Abstractions;

    public class SkewCorrector : ISkewCorrector
    {
        private readonly ILeptonicaApiSignatures leptonicaApi;

        public SkewCorrector(ILeptonicaApiSignatures leptonicaApi)
        {
            this.leptonicaApi = leptonicaApi ?? throw new ArgumentNullException(nameof(leptonicaApi));
        }

        /// <inheritdoc />
        public Pix DeskewImage(Pix source, ScewSweep sweep, int redSearch, int thresh, out Scew scew)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            IntPtr resultPixHandle = this.leptonicaApi.pixDeskewGeneral(source.Handle, sweep.Reduction, sweep.Range, sweep.Delta, redSearch, thresh, out float pAngle, out float pConf);
            if (resultPixHandle == IntPtr.Zero) throw new TesseractException(Resources.SkewCorrector_DeskewImage_Failed_to_deskew_image_);
            scew = new Scew(pAngle, pConf);
            return new Pix(this.leptonicaApi, resultPixHandle);
        }
    }
}