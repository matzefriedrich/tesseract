namespace Tesseract.Rendering
{
    using System;
    using Interop.Abstractions;
    using JetBrains.Annotations;

    public sealed class TsvResultRenderer : ResultRenderer
    {
        public TsvResultRenderer(ITessApiSignatures native, [NotNull] string outputFilename) : base(native)
        {
            if (string.IsNullOrWhiteSpace(outputFilename)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(outputFilename));
            IntPtr handle = native.TsvRendererCreate(outputFilename);
            this.AssignHandle(handle);
        }
    }
}