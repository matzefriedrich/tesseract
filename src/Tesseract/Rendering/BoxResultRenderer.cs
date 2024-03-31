namespace Tesseract.Rendering
{
    using System;
    using Interop.Abstractions;
    using JetBrains.Annotations;

    public sealed class BoxResultRenderer : ResultRenderer
    {
        public BoxResultRenderer(ITessApiSignatures native, [NotNull] string outputFilename) : base(native)
        {
            if (string.IsNullOrWhiteSpace(outputFilename)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(outputFilename));
            IntPtr handle = native.BoxTextRendererCreate(outputFilename);
            this.AssignHandle(handle);
        }
    }
}