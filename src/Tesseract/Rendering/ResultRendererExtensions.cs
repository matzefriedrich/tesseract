namespace Tesseract.Rendering
{
    using System;
    using Abstractions;
    using JetBrains.Annotations;

    public static class ResultRendererExtensions
    {
        public static AggregateResultRenderer AsDocumentRenderer([NotNull] this IResultRenderer renderer)
        {
            if (renderer == null) throw new ArgumentNullException(nameof(renderer));
            return new AggregateResultRenderer(new[] { renderer });
        }
    }
}