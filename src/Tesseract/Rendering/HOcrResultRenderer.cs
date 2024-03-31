namespace Tesseract.Rendering
{
    using System;
    using Interop.Abstractions;
    using JetBrains.Annotations;

    public sealed class HOcrResultRenderer : ResultRenderer
    {
        public HOcrResultRenderer(ITessApiSignatures native, [NotNull] string outputFilename, bool fontInfo = false) : base(native)
        {
            if (string.IsNullOrWhiteSpace(outputFilename)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(outputFilename));

            int info = fontInfo ? 1 : 0;
            IntPtr handle = native.HOcrRendererCreate2(outputFilename, info);
            this.AssignHandle(handle);
        }
    }
}