namespace Tesseract.Rendering
{
    using System;

    using Interop.Abstractions;

    public sealed class WordStrBoxResultRenderer : ResultRenderer
    {
        public WordStrBoxResultRenderer(ITessApiSignatures native, string outputFilename) : base(native)
        {
            IntPtr rendererHandle = native.WordStrBoxRendererCreate(outputFilename);
            this.Initialise(rendererHandle);
        }
    }
}