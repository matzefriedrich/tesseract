namespace Tesseract.Rendering
{
    using System;
    using Interop.Abstractions;
    using JetBrains.Annotations;

    public sealed class WordStrBoxResultRenderer : ResultRenderer
    {
        public WordStrBoxResultRenderer(ITessApiSignatures native, [NotNull] string outputFilename) : base(native)
        {
            if (string.IsNullOrWhiteSpace(outputFilename)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(outputFilename));
            IntPtr handle = native.WordStrBoxRendererCreate(outputFilename);
            this.AssignHandle(handle);
        }
    }
}