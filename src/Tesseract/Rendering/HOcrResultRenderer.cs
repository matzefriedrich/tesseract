namespace Tesseract.Rendering
{
    using System;
    using Interop.Abstractions;
    using Resources;

    public sealed class HOcrResultRenderer : ResultRenderer
    {
        public HOcrResultRenderer(ITessApiSignatures native, string outputFilename, bool fontInfo = false) : base(native)
        {
            if (string.IsNullOrWhiteSpace(outputFilename)) throw new ArgumentException(Resources.Value_cannot_be_null_or_whitespace, nameof(outputFilename));

            int info = fontInfo ? 1 : 0;
            IntPtr handle = native.HOcrRendererCreate2(outputFilename, info);
            this.AssignHandle(handle);
        }
    }
}