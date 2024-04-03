namespace Tesseract.Tests
{
    using System.Runtime.Serialization;
    using Abstractions;

    [DataContract]
    public class TesseractResultSet
    {
        [DataContract]
        public class Page
        {
            public Rect? Region { get; set; }
            public List<Block> Blocks { get; set; } = new();
        }

        [DataContract]
        public class Block
        {
            public Rect? Region { get; set; }
            public float Confidence { get; set; }
            public string? Text { get; set; }

            public List<Line> Lines { get; set; } = new();
        }

        [DataContract]
        public class Line
        {
            public Rect? Region { get; set; }
            public float? Confidence { get; set; }
            public string? Text { get; set; }

            public List<Word> Words { get; set; } = new();
        }

        [DataContract]
        public class Word
        {
            public Rect? Region { get; set; }
            public float Confidence { get; set; }
            public string? Text { get; set; }

            public List<Symbol> Words { get; set; } = new();
        }

        [DataContract]
        public class Symbol
        {
            public Rect Region { get; set; }
            public float Confidence { get; set; }
            public char Char { get; set; }
        }
    }
}