namespace Tesseract.Abstractions
{
    using System;

    public interface IPixFactory
    {
        Pix Create(int width, int height, int depth);
        Pix Create(IntPtr handle);
        Pix LoadFromFile(string filename);
        Pix LoadFromMemory(byte[] bytes);
        Pix LoadTiffFromMemory(byte[] bytes);
        Pix ReadFromMultiPageTiff(string filename, ref int offset);
        Pix Clone(Pix source);
    }
}