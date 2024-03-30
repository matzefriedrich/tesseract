namespace Tesseract.Abstractions
{
    public interface IPixColorMapFactory
    {
        PixColormap Create(int depth);
        PixColormap CreateLinear(int depth, int levels);
        PixColormap CreateLinear(int depth, bool firstIsBlack, bool lastIsWhite);
    }
}