﻿namespace Tesseract
{
    using System.Runtime.CompilerServices;

    /// <summary>
    ///     Description of BitmapHelper.
    /// </summary>
    public static unsafe class BitmapHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ConvertRgb555ToRgba(uint val)
        {
            uint red = (val & 0x7C00) >> 10;
            uint green = (val & 0x3E0) >> 5;
            uint blue = val & 0x1F;

            return (((red << 3) | (red >> 2)) << 24) | (((green << 3) | (green >> 2)) << 16) | (((blue << 3) | (blue >> 2)) << 8) | 0xFF;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ConvertRgb565ToRgba(uint val)
        {
            uint red = (val & 0xF800) >> 11;
            uint green = (val & 0x7E0) >> 5;
            uint blue = val & 0x1F;

            return (((red << 3) | (red >> 2)) << 24) | (((green << 2) | (green >> 4)) << 16) | (((blue << 3) | (blue >> 2)) << 8) | 0xFF;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ConvertArgb1555ToRgba(uint val)
        {
            uint alpha = (val & 0x8000) >> 15;
            uint red = (val & 0x7C00) >> 10;
            uint green = (val & 0x3E0) >> 5;
            uint blue = val & 0x1F;

            return (((red << 3) | (red >> 2)) << 24) | (((green << 3) | (green >> 2)) << 16) | (((blue << 3) | (blue >> 2)) << 8) | ((alpha << 8) - alpha); // effectively alpha * 255, only works as alpha will be either 0 or 1
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint EncodeAsRgba(byte red, byte green, byte blue, byte alpha)
        {
            return (uint)((red << 24) | (green << 16) | (blue << 8) | alpha);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetDataBit(byte* data, int index)
        {
            return (byte)((*(data + (index >> 3)) >> (index & 0x7)) & 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDataBit(byte* data, int index, byte value)
        {
            byte* wordPtr = data + (index >> 3);
            *wordPtr &= (byte)~(0x80 >> (index & 7)); // clear bit, note first pixel in the byte is most significant (1000 0000)
            *wordPtr |= (byte)((value & 1) << (7 - (index & 7))); // set bit, if value is 1
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetDataQBit(byte* data, int index)
        {
            return (byte)((*(data + (index >> 1)) >> (4 * (index & 1))) & 0xF);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDataQBit(byte* data, int index, byte value)
        {
            byte* wordPtr = data + (index >> 1);
            *wordPtr &= (byte)~(0xF0 >> (4 * (index & 1))); // clears qbit located at index, note like bit the qbit corresponding to the first pixel is the most significant (0xF0)
            *wordPtr |= (byte)((value & 0x0F) << (4 - 4 * (index & 1))); // applys qbit to n
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetDataByte(byte* data, int index)
        {
            return *(data + index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDataByte(byte* data, int index, byte value)
        {
            *(data + index) = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort GetDataUInt16(ushort* data, int index)
        {
            return *(data + index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDataUInt16(ushort* data, int index, ushort value)
        {
            *(data + index) = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetDataUInt32(uint* data, int index)
        {
            return *(data + index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetDataUInt32(uint* data, int index, uint value)
        {
            *(data + index) = value;
        }
    }
}