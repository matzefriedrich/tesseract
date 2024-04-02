namespace Tesseract
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using Abstractions;
    using Interop.Abstractions;
    using JetBrains.Annotations;
    using Microsoft.Win32.SafeHandles;

    public class PixFileWriter : IPixFileWriter
    {
        /// <summary>
        ///     Used to lookup image formats by extension.
        /// </summary>
        private static readonly Dictionary<string, ImageFormat> ImageFormatLookup = new()
        {
            { ".jpg", ImageFormat.JfifJpeg },
            { ".jpeg", ImageFormat.JfifJpeg },
            { ".gif", ImageFormat.Gif },
            { ".tif", ImageFormat.Tiff },
            { ".tiff", ImageFormat.Tiff },
            { ".png", ImageFormat.Png },
            { ".bmp", ImageFormat.Bmp }
        };

        private readonly ILeptonicaApiSignatures leptonicaApi;

        public PixFileWriter([NotNull] ILeptonicaApiSignatures leptonicaApi)
        {
            this.leptonicaApi = leptonicaApi ?? throw new ArgumentNullException(nameof(leptonicaApi));
        }

        /// <summary>
        ///     Saves the image to the specified file.
        /// </summary>
        /// <param name="image">The image to save.</param>
        /// <param name="filename">The path to the file.</param>
        /// <param name="format">
        ///     The format to use when saving the image, if not specified the file extension is used to guess
        ///     the  format.
        /// </param>
        public void Save(Pix image, string filename, ImageFormat? format = null)
        {
            ArgumentNullException.ThrowIfNull(image);
            if (string.IsNullOrWhiteSpace(filename)) throw new ArgumentException(Resources.Resources.Value_cannot_be_null_or_whitespace, nameof(filename));

            ImageFormat actualFormat;
            if (!format.HasValue)
            {
                string extension = Path.GetExtension(filename).ToLowerInvariant();
                // couldn't find matching format, perhaps there is no extension or it's not recognised, fallback to default.
                actualFormat = ImageFormatLookup.GetValueOrDefault(extension, ImageFormat.Default);
            }
            else
            {
                actualFormat = format.Value;
            }

            int pixWrite = this.leptonicaApi.pixWrite(filename, image.Handle, actualFormat);
            if (pixWrite != 0)
                throw new IOException($"Failed to save image '{filename}'.");
        }

        public void Save([NotNull] Pix image, [NotNull] Stream target, ImageFormat format = ImageFormat.Default)
        {
            throw new NotImplementedException("Requires an implementation of: LEPT_DLL l_int32 pixWriteStream\t(\tFILE * \tfp,\nPIX * \tpix,\nl_int32 \tformat \n)\t");
        }
    }
}