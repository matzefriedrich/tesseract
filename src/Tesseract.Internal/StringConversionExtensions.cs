namespace Tesseract.Internal
{
    using System.Globalization;

    /// <summary>
    ///     Utility helpers to handle converting variable values.
    /// </summary>
    internal static class StringConversionExtensions
    {
        public static bool TryFormatAsString(this object? value, out string? result)
        {
            result = null;
            if (value == null) return false;

            switch (value)
            {
                case bool b:
                    result = FormatAsString(b);
                    break;
                case decimal f16:
                    result = FormatAsString(f16);
                    break;
                case double f8:
                    result = FormatAsString(f8);
                    break;
                case float f4:
                    result = FormatAsString(f4);
                    break;
                case short i2:
                    result = FormatAsString(i2);
                    break;
                case int i4:
                    result = FormatAsString(i4);
                    break;
                case long i8:
                    result = FormatAsString(i8);
                    break;
                case ushort u2:
                    result = FormatAsString(u2);
                    break;
                case uint u4:
                    result = FormatAsString(u4);
                    break;
                case ulong u8:
                    result = FormatAsString(u8);
                    break;
                case string s:
                    result = s;
                    break;
                default:
                    result = null;
                    return false;
            }

            return true;
        }

        private static string FormatAsString(this bool value)
        {
            return value ? "TRUE" : "FALSE";
        }

        private static string FormatAsString(this decimal value)
        {
            return value.ToString("R", CultureInfo.InvariantCulture.NumberFormat);
        }

        private static string FormatAsString(this double value)
        {
            return value.ToString("R", CultureInfo.InvariantCulture.NumberFormat);
        }

        private static string FormatAsString(this float value)
        {
            return value.ToString("R", CultureInfo.InvariantCulture.NumberFormat);
        }

        private static string FormatAsString(this short value)
        {
            return value.ToString("D", CultureInfo.InvariantCulture.NumberFormat);
        }

        private static string FormatAsString(this int value)
        {
            return value.ToString("D", CultureInfo.InvariantCulture.NumberFormat);
        }

        private static string FormatAsString(this long value)
        {
            return value.ToString("D", CultureInfo.InvariantCulture.NumberFormat);
        }

        private static string FormatAsString(this ushort value)
        {
            return value.ToString("D", CultureInfo.InvariantCulture.NumberFormat);
        }

        private static string FormatAsString(this uint value)
        {
            return value.ToString("D", CultureInfo.InvariantCulture.NumberFormat);
        }

        private static string FormatAsString(this ulong value)
        {
            return value.ToString("D", CultureInfo.InvariantCulture.NumberFormat);
        }
    }
}