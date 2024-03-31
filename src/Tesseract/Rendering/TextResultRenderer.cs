namespace Tesseract.Rendering
{
    using System;
    using Interop.Abstractions;
    using JetBrains.Annotations;

    public sealed class TextResultRenderer : ResultRenderer
    {
        public TextResultRenderer(ITessApiSignatures native, [NotNull] string outputFilename) : base(native)
        {
            if (string.IsNullOrWhiteSpace(outputFilename)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(outputFilename));
            IntPtr handle = native.TextRendererCreate(outputFilename);
            this.AssignHandle(handle);
        }
    }
}