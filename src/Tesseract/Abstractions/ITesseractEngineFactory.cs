namespace Tesseract.Abstractions
{
    using System;

    public interface ITesseractEngineFactory
    {
        ITesseractEngine CreateEngine(Action<EngineOptionBuilder>? builder = null);
    }
}