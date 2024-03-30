namespace Tesseract.Abstractions
{
    using System;

    public interface IPixFactory
    {
        Pix Create(int width, int height, int depth);
        Pix Create(IntPtr handle);
        Pix LoadFromFile(string filename);
        unsafe Pix LoadFromMemory(byte[] bytes);
        unsafe Pix LoadTiffFromMemory(byte[] bytes);
        Pix pixReadFromMultipageTiff(string filename, ref int offset);
    }
}