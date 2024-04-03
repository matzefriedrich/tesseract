namespace Tesseract.Rendering
{
    using System;
    using Interop.Abstractions;
    using Resources;

    public sealed class TsvResultRenderer : ResultRenderer
    {
        public TsvResultRenderer(ITessApiSignatures native, string outputFilename) : base(native)
        {
            if (string.IsNullOrWhiteSpace(outputFilename)) throw new ArgumentException(Resources.Value_cannot_be_null_or_whitespace, nameof(outputFilename));
            IntPtr handle = native.TsvRendererCreate(outputFilename);
            this.AssignHandle(handle);
        }
    }
}