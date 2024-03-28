namespace Tesseract
{
    // The .NET equivalent of the ccstruct/fontinfo.h
    // FontInfo struct. It's missing spacing info
    // since we don't have any way of getting it (and
    // it's probably not all that useful anyway)
    public class FontInfo
    {
        internal FontInfo(
            string name, int id,
            bool isItalic, bool isBold, bool isFixedPitch,
            bool isSerif, bool isFraktur = false
        )
        {
            this.Name = name;
            this.Id = id;

            this.IsItalic = isItalic;
            this.IsBold = isBold;
            this.IsFixedPitch = isFixedPitch;
            this.IsSerif = isSerif;
            this.IsFraktur = isFraktur;
        }

        public string Name { get; private set; }

        public int Id { get; private set; }
        public bool IsItalic { get; private set; }
        public bool IsBold { get; private set; }
        public bool IsFixedPitch { get; private set; }
        public bool IsSerif { get; private set; }
        public bool IsFraktur { get; private set; }
    }
}