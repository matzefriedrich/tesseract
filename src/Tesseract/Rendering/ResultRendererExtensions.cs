namespace Tesseract.Rendering
{
    using System;
    using Abstractions;

    public static class ResultRendererExtensions
    {
        public static AggregateResultRenderer AsDocumentRenderer(this IResultRenderer renderer)
        {
            ArgumentNullException.ThrowIfNull(renderer);
            return new AggregateResultRenderer(new[] { renderer });
        }
    }
}