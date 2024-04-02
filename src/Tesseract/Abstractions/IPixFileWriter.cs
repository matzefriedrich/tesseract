namespace Tesseract.Abstractions
{
    using System.IO;
    using JetBrains.Annotations;
    using Tesseract.Interop.Abstractions;

    public interface IPixFileWriter
    {
        /// <summary>
        ///     Saves the image to the specified file.
        /// </summary>
        /// <param name="image">The image to save.</param>
        /// <param name="filename">The path to the file.</param>
        /// <param name="format">
        ///     The format to use when saving the image, if not specified the file extension is used to guess
        ///     the  format.
        /// </param>
        void Save([NotNull] Pix image, [NotNull] string filename, ImageFormat? format = null);

        /// <summary>
        /// Saves the image to the specified stream.
        /// </summary>
        /// <param name="image">The image to save.</param>
        /// <param name="target">A <see cref="Stream"/> object representing the target file.</param>
        /// <param name="format"></param>
        void Save(Pix image, Stream target, ImageFormat format = ImageFormat.Default);
    }
}