namespace Tesseract.Rendering
{
    using System;
    using Interop.Abstractions;
    using JetBrains.Annotations;

    public sealed class LSTMBoxResultRenderer : ResultRenderer
    {
        public LSTMBoxResultRenderer(ITessApiSignatures native, [NotNull] string outputFilename) : base(native)
        {
            if (string.IsNullOrWhiteSpace(outputFilename)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(outputFilename));
            IntPtr handle = native.LSTMBoxRendererCreate(outputFilename);
            this.AssignHandle(handle);
        }
    }
}