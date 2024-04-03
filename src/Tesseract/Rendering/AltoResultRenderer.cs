namespace Tesseract.Rendering
{
    using System;
    using Interop.Abstractions;
    using Resources;

    public sealed class AltoResultRenderer : ResultRenderer
    {
        public AltoResultRenderer(ITessApiSignatures native, string outputFilename) : base(native)
        {
            if (string.IsNullOrWhiteSpace(outputFilename)) throw new ArgumentException(Resources.Value_cannot_be_null_or_whitespace, nameof(outputFilename));
            IntPtr handle = native.AltoRendererCreate(outputFilename);
            this.AssignHandle(handle);
        }
    }
}