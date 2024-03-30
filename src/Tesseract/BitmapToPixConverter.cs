namespace Tesseract
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    using Abstractions;

    using JetBrains.Annotations;

    /// <summary>
    ///     Converts a <see cref="Bitmap" /> to a <see cref="Pix" />.
    /// </summary>
    internal sealed class BitmapToPixConverter : IBitmapToPixConverter
    {
        private readonly IPixFactory pixFactory;
        private readonly IPixColorMapFactory pixColorMapFactory;

        public BitmapToPixConverter(
            [NotNull] IPixFactory pixFactory,
            [NotNull] IPixColorMapFactory pixColorMapFactory)
        {
            this.pixFactory = pixFactory ?? throw new ArgumentNullException(nameof(pixFactory));
            this.pixColorMapFactory = pixColorMapFactory ?? throw new ArgumentNullException(nameof(pixColorMapFactory));
        }
        
        /// <summary>
        ///     Converts the specified <paramref name="img" /> to a <see cref="Pix" />.
        /// </summary>
        /// <param name="img">The source image to be converted.</param>
        /// <returns>The converted pix.</returns>
        public Pix Convert(Bitmap img)
        {
            int pixDepth = this.GetPixDepth(img.PixelFormat);
            Pix pix = this.pixFactory.Create(img.Width, img.Height, pixDepth);
            pix.XRes = (int)Math.Round(img.HorizontalResolution);
            pix.YRes = (int)Math.Round(img.VerticalResolution);

            BitmapData imgData = null;
            try
            {
                // TODO: Set X and Y resolution

                if ((img.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed) this.CopyColormap(img, pix);

                // transfer data
                imgData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
                PixData pixData = pix.GetData();

                if (imgData.PixelFormat == PixelFormat.Format32bppArgb)
                    this.TransferDataFormat32bppArgb(imgData, pixData);
                else if (imgData.PixelFormat == PixelFormat.Format32bppRgb)
                    this.TransferDataFormat32bppRgb(imgData, pixData);
                else if (imgData.PixelFormat == PixelFormat.Format24bppRgb)
                    this.TransferDataFormat24bppRgb(imgData, pixData);
                else if (imgData.PixelFormat == PixelFormat.Format8bppIndexed)
                    this.TransferDataFormat8bppIndexed(imgData, pixData);
                else if (imgData.PixelFormat == PixelFormat.Format1bppIndexed) this.TransferDataFormat1bppIndexed(imgData, pixData);
                return pix;
            }
            catch (Exception)
            {
                pix.Dispose();
                throw;
            }
            finally
            {
                if (imgData != null) img.UnlockBits(imgData);
            }
        }

        private void CopyColormap(Bitmap img, Pix pix)
        {
            ColorPalette imgPalette = img.Palette;
            Color[] imgPaletteEntries = imgPalette.Entries;
            PixColormap pixColormap = this.pixColorMapFactory.Create(pix.Depth);
            try
            {
                for (var i = 0; i < imgPaletteEntries.Length; i++)
                {
                    Color paletteEntry = imgPaletteEntries[i];
                    if (!pixColormap.AddColor(paletteEntry.ToPixColor())) throw new InvalidOperationException($"Failed to add colormap entry {i}.");
                }

                pix.Colormap = pixColormap;
            }
            catch (Exception)
            {
                pixColormap.Dispose();
                throw;
            }
        }

        private int GetPixDepth(PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Format1bppIndexed:
                    return 1;

                case PixelFormat.Format8bppIndexed:
                    return 8;

                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format24bppRgb:
                    return 32;

                default:
                    throw new InvalidOperationException($"Source bitmap's pixel format {pixelFormat} is not supported.");
            }
        }

        private unsafe void TransferDataFormat1bppIndexed(BitmapData imgData, PixData pixData)
        {
            int height = imgData.Height;
            int width = imgData.Width / 8;
            for (var y = 0; y < height; y++)
            {
                byte* imgLine = (byte*)imgData.Scan0 + y * imgData.Stride;
                uint* pixLine = (uint*)pixData.Data + y * pixData.WordsPerLine;

                for (var x = 0; x < width; x++)
                {
                    byte pixelVal = BitmapHelper.GetDataByte(imgLine, x);
                    PixData.SetDataByte(pixLine, x, pixelVal);
                }
            }
        }

        private unsafe void TransferDataFormat24bppRgb(BitmapData imgData, PixData pixData)
        {
            PixelFormat imgFormat = imgData.PixelFormat;
            int height = imgData.Height;
            int width = imgData.Width;

            for (var y = 0; y < height; y++)
            {
                byte* imgLine = (byte*)imgData.Scan0 + y * imgData.Stride;
                uint* pixLine = (uint*)pixData.Data + y * pixData.WordsPerLine;

                for (var x = 0; x < width; x++)
                {
                    byte* pixelPtr = imgLine + x * 3;
                    byte blue = pixelPtr[0];
                    byte green = pixelPtr[1];
                    byte red = pixelPtr[2];
                    PixData.SetDataFourByte(pixLine, x, BitmapHelper.EncodeAsRGBA(red, green, blue, 255));
                }
            }
        }

        private unsafe void TransferDataFormat32bppRgb(BitmapData imgData, PixData pixData)
        {
            int height = imgData.Height;
            int width = imgData.Width;

            for (var y = 0; y < height; y++)
            {
                byte* imgLine = (byte*)imgData.Scan0 + y * imgData.Stride;
                uint* pixLine = (uint*)pixData.Data + y * pixData.WordsPerLine;

                for (var x = 0; x < width; x++)
                {
                    byte* pixelPtr = imgLine + (x << 2);
                    byte blue = *pixelPtr;
                    byte green = *(pixelPtr + 1);
                    byte red = *(pixelPtr + 2);
                    PixData.SetDataFourByte(pixLine, x, BitmapHelper.EncodeAsRGBA(red, green, blue, 255));
                }
            }
        }

        private unsafe void TransferDataFormat32bppArgb(BitmapData imgData, PixData pixData)
        {
            int height = imgData.Height;
            int width = imgData.Width;

            for (var y = 0; y < height; y++)
            {
                byte* imgLine = (byte*)imgData.Scan0 + y * imgData.Stride;
                uint* pixLine = (uint*)pixData.Data + y * pixData.WordsPerLine;

                for (var x = 0; x < width; x++)
                {
                    byte* pixelPtr = imgLine + (x << 2);
                    byte blue = *pixelPtr;
                    byte green = *(pixelPtr + 1);
                    byte red = *(pixelPtr + 2);
                    byte alpha = *(pixelPtr + 3);
                    PixData.SetDataFourByte(pixLine, x, BitmapHelper.EncodeAsRGBA(red, green, blue, alpha));
                }
            }
        }

        private unsafe void TransferDataFormat8bppIndexed(BitmapData imgData, PixData pixData)
        {
            int height = imgData.Height;
            int width = imgData.Width;

            for (var y = 0; y < height; y++)
            {
                byte* imgLine = (byte*)imgData.Scan0 + y * imgData.Stride;
                uint* pixLine = (uint*)pixData.Data + y * pixData.WordsPerLine;

                for (var x = 0; x < width; x++)
                {
                    byte pixelVal = *(imgLine + x);
                    PixData.SetDataByte(pixLine, x, pixelVal);
                }
            }
        }
    }
}