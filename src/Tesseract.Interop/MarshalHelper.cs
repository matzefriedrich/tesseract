namespace Tesseract.Interop
{
    using System.Runtime.InteropServices;
    using System.Text;

    internal static unsafe class MarshalHelper
    {
        public static IntPtr StringToPtr(string value, Encoding encoding)
        {
            int length = encoding.GetByteCount(value);
            // The encoded value is null terminated that's the reason for the '+1'.
            var encodedValue = new byte[length + 1];
            encoding.GetBytes(value, 0, value.Length, encodedValue, 0);
            IntPtr handle = Marshal.AllocHGlobal(new IntPtr(encodedValue.Length));
            Marshal.Copy(encodedValue, 0, handle, encodedValue.Length);
            return handle;
        }

        public static string? PtrToString(IntPtr? handle, Encoding encoding)
        {
            ArgumentNullException.ThrowIfNull(encoding);
            if (handle == null) return null;

            int length = StrLength(handle.Value);
            return new string((sbyte*)handle.Value.ToPointer(), 0, length, encoding);
        }

        /// <summary>
        ///     Gets the number of bytes in a null terminated byte array.
        /// </summary>
        private static int StrLength(IntPtr handle)
        {
            if (handle == IntPtr.Zero) return 0;

            var ptr = (byte*)handle.ToPointer();
            var length = 0;
            while (*(ptr + length) != 0) length++;
            return length;
        }
    }
}