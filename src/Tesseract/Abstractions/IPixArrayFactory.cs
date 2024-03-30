namespace Tesseract
{
    using JetBrains.Annotations;

    public interface IPixArrayFactory
    {
        /// <summary>
        ///     Loads the multi-page tiff located at <paramref name="filename" />.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        PixArray LoadMultiPageTiffFromFile([NotNull] string filename);

        PixArray Create(int n);
    }
}