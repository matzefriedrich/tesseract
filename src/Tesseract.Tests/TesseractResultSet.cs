namespace Tesseract.Tests
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class TesseractResultSet
    {
        [DataContract]
        public class Page
        {
            public Rect? Region { get; set; }
            public List<Block> Blocks { get; set; } = new List<Block>();
        }

        [DataContract]
        public class Block
        {
            public Rect? Region { get; set; }
            public float Confidence { get; set; }
            public string? Text { get; set; }

            public List<Line> Lines { get; set; } = new List<Line>();
        }

        [DataContract]
        public class Line
        {
            public Rect? Region { get; set; }
            public float? Confidence { get; set; }
            public string? Text { get; set; }

            public List<Word> Words { get; set; } = new List<Word>();
        }

        [DataContract]
        public class Word
        {
            public Rect? Region { get; set; }
            public float Confidence { get; set; }
            public string? Text { get; set; }

            public List<Symbol> Words { get; set; } = new List<Symbol>();
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