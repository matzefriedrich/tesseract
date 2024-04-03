namespace Tesseract.Abstractions
{
    public interface IPixArrayFactory
    {
        /// <summary>
        ///     Loads the multi-page tiff located at <paramref name="filename" />.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        PixArray LoadMultiPageTiffFromFile(string filename);

        PixArray Create(int n);
    }
}