namespace Tesseract.Rendering
{
    using System;

    using Interop.Abstractions;

    public sealed class LSTMBoxResultRenderer : ResultRenderer
    {
        public LSTMBoxResultRenderer(ITessApiSignatures native, string outputFilename) : base(native)
        {
            IntPtr rendererHandle = native.LSTMBoxRendererCreate(outputFilename);
            this.Initialise(rendererHandle);
        }
    }
}