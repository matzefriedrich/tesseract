namespace Tesseract.Rendering
{
    using System;

    using Interop.Abstractions;

    public sealed class TextResultRenderer : ResultRenderer
    {
        public TextResultRenderer(ITessApiSignatures native, string outputFilename) : base(native)
        {
            IntPtr rendererHandle = native.TextRendererCreate(outputFilename);
            this.Initialise(rendererHandle);
        }
    }
}