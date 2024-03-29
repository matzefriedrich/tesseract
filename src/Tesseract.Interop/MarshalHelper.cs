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
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));
            if (handle != null)
            {
                int length = StrLength(handle.Value);
                return new string((sbyte*)handle.Value.ToPointer(), 0, length, encoding);
            }

            return null;
        }

        /// <summary>
        ///     Gets the number of bytes in a null terminated byte array.
        /// </summary>
        public static int StrLength(IntPtr handle)
        {
            if (handle == IntPtr.Zero) return 0;

            var ptr = (byte*)handle.ToPointer();
            var length = 0;
            while (*(ptr + length) != 0) length++;
            return length;
        }
    }
}