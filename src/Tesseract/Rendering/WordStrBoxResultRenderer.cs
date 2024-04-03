namespace Tesseract.Rendering
{
    using System;
    using Interop.Abstractions;
    using Resources;

    public sealed class WordStrBoxResultRenderer : ResultRenderer
    {
        public WordStrBoxResultRenderer(ITessApiSignatures native, string outputFilename) : base(native)
        {
            if (string.IsNullOrWhiteSpace(outputFilename)) throw new ArgumentException(Resources.Value_cannot_be_null_or_whitespace, nameof(outputFilename));
            IntPtr handle = native.WordStrBoxRendererCreate(outputFilename);
            this.AssignHandle(handle);
        }
    }
}