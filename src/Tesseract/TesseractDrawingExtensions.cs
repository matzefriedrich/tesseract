namespace Tesseract
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using Abstractions;

    public static class TesseractDrawingExtensions
    {
        public static Color ToColor(this PixColor color)
        {
            return Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
        }

        public static PixColor ToPixColor(this Color color)
        {
            return new PixColor(color.R, color.G, color.B, color.A);
        }

        /// <summary>
        ///     gets the number of Bits Per Pixel (BPP)
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static int GetBpp(this Bitmap bitmap)
        {
            switch (bitmap.PixelFormat)
            {
                case PixelFormat.Format1bppIndexed: return 1;
                case PixelFormat.Format4bppIndexed: return 4;
                case PixelFormat.Format8bppIndexed: return 8;
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format16bppGrayScale:
                case PixelFormat.Format16bppRgb555:
                case PixelFormat.Format16bppRgb565: return 16;
                case PixelFormat.Format24bppRgb: return 24;
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppRgb: return 32;
                case PixelFormat.Format48bppRgb: return 48;
                case PixelFormat.Format64bppArgb:
                case PixelFormat.Format64bppPArgb: return 64;
                default: throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.Resources.TesseractDrawingExtensions_GetBpp_The_bitmap_s_pixel_format_of__0__was_not_recognised_, bitmap.PixelFormat), nameof(bitmap));
            }
        }
    }
}