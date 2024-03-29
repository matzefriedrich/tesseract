namespace Tesseract
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Abstractions;

    using Interop;

    public sealed class ResultIterator : PageIterator
    {
        private readonly Dictionary<int, FontInfo> _fontInfoCache = new();

        internal ResultIterator(Page page, IntPtr handle)
            : base(page, handle)
        {
        }

        public float GetConfidence(PageIteratorLevel level)
        {
            this.ThrowIfDisposed();
            if (this.handle.Handle == IntPtr.Zero)
                return 0f;

            return TessApi.Native.ResultIteratorGetConfidence(this.handle, level);
        }

        public string GetText(PageIteratorLevel level)
        {
            this.ThrowIfDisposed();
            if (this.handle.Handle == IntPtr.Zero) return string.Empty;

            return TessApi.ResultIteratorGetUTF8Text(this.handle, level);
        }

        public FontAttributes GetWordFontAttributes()
        {
            this.ThrowIfDisposed();
            if (this.handle.Handle == IntPtr.Zero) return null;

            // per docs (ltrresultiterator.h:104 as of 4897796 in github:tesseract-ocr/tesseract)
            // this return value points to an internal table and should not be deleted.
            IntPtr nameHandle = TessApi.Native.ResultIteratorWordFontAttributes(this.handle, out bool isBold, out bool isItalic, out bool isUnderlined, out bool isMonospace, out bool isSerif, out bool isSmallCaps, out int pointSize, out int fontId);

            // This can happen in certain error conditions
            if (nameHandle == IntPtr.Zero) return null;

            if (!this._fontInfoCache.TryGetValue(fontId, out FontInfo fontInfo))
            {
                string fontName = MarshalHelper.PtrToString(nameHandle, Encoding.UTF8);
                fontInfo = new FontInfo(fontName, fontId, isItalic, isBold, isMonospace, isSerif);
                this._fontInfoCache.Add(fontId, fontInfo);
            }

            return new FontAttributes(fontInfo, isUnderlined, isSmallCaps, pointSize);
        }

        public string GetWordRecognitionLanguage()
        {
            this.ThrowIfDisposed();
            if (this.handle.Handle == IntPtr.Zero) return null;

            return TessApi.ResultIteratorWordRecognitionLanguage(this.handle);
        }

        public bool GetWordIsFromDictionary()
        {
            this.ThrowIfDisposed();
            if (this.handle.Handle == IntPtr.Zero) return false;

            return TessApi.Native.ResultIteratorWordIsFromDictionary(this.handle);
        }

        public bool GetWordIsNumeric()
        {
            this.ThrowIfDisposed();
            if (this.handle.Handle == IntPtr.Zero) return false;

            return TessApi.Native.ResultIteratorWordIsNumeric(this.handle);
        }

        public bool GetSymbolIsSuperscript()
        {
            this.ThrowIfDisposed();
            if (this.handle.Handle == IntPtr.Zero) return false;

            return TessApi.Native.ResultIteratorSymbolIsSuperscript(this.handle);
        }

        public bool GetSymbolIsSubscript()
        {
            this.ThrowIfDisposed();
            if (this.handle.Handle == IntPtr.Zero) return false;

            return TessApi.Native.ResultIteratorSymbolIsSubscript(this.handle);
        }

        public bool GetSymbolIsDropcap()
        {
            this.ThrowIfDisposed();
            if (this.handle.Handle == IntPtr.Zero) return false;

            return TessApi.Native.ResultIteratorSymbolIsDropcap(this.handle);
        }

        /// <summary>
        ///     Gets an instance of a choice iterator using the current symbol of interest. The ChoiceIterator allows a one-shot
        ///     iteration over the
        ///     choices for this symbol and after that is is useless.
        /// </summary>
        /// <returns>an instance of a Choice Iterator</returns>
        public ChoiceIterator GetChoiceIterator()
        {
            IntPtr choiceIteratorHandle = TessApi.Native.ResultIteratorGetChoiceIterator(this.handle);
            if (choiceIteratorHandle == IntPtr.Zero)
                return null;
            return new ChoiceIterator(choiceIteratorHandle);
        }
    }
}