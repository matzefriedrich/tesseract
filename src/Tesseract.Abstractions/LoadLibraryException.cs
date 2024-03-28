namespace Tesseract.Abstractions
{
    [Serializable]
    public class LoadLibraryException : SystemException
    {
        public LoadLibraryException()
        {
        }

        public LoadLibraryException(string message) : base(message)
        {
        }

        public LoadLibraryException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}