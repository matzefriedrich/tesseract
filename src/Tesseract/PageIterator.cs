﻿namespace Tesseract
{
    using System;
    using System.Runtime.InteropServices;

    using Abstractions;

    using Interop;

    /// <summary>
    ///     Represents an object that can iterate over tesseract's page structure.
    /// </summary>
    /// <remarks>
    ///     The iterator points to tesseract's internal page structure and is only valid while the Engine instance that created
    ///     it exists
    ///     and has not been subjected to a call to Recognize since the iterator was created.
    /// </remarks>
    public class PageIterator : DisposableBase
    {
        protected readonly HandleRef handle;
        protected readonly Page page;

        internal PageIterator(Page page, IntPtr handle)
        {
            this.page = page;
            this.handle = new HandleRef(this, handle);
        }

        public PolyBlockType BlockType
        {
            get
            {
                this.ThrowIfDisposed();

                if (this.handle.Handle == IntPtr.Zero)
                    return PolyBlockType.Unknown;
                return TessApi.Native.PageIteratorBlockType(this.handle);
            }
        }

        /// <summary>
        ///     Moves the iterator to the start of the page.
        /// </summary>
        public void Begin()
        {
            this.ThrowIfDisposed();
            if (this.handle.Handle != IntPtr.Zero) TessApi.Native.PageIteratorBegin(this.handle);
        }

        /// <summary>
        ///     Moves to the start of the next element at the given level.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool Next(PageIteratorLevel level)
        {
            this.ThrowIfDisposed();
            if (this.handle.Handle == IntPtr.Zero)
                return false;
            return TessApi.Native.PageIteratorNext(this.handle, level) != 0;
        }

        /// <summary>
        ///     Moves the iterator to the next <paramref name="element" /> iff the iterator is not currently pointing to the last
        ///     <paramref name="element" /> in the specified <paramref name="level" /> (i.e. the last word in the paragraph).
        /// </summary>
        /// <param name="level">The iterator level.</param>
        /// <param name="element">The page level.</param>
        /// <returns>
        ///     <c>True</c> iff there is another <paramref name="element" /> to advance too and the current element is not the
        ///     last element at the given level; otherwise returns <c>False</c>.
        /// </returns>
        public bool Next(PageIteratorLevel level, PageIteratorLevel element)
        {
            this.ThrowIfDisposed();

            bool isAtFinalElement = this.IsAtFinalOf(level, element);
            if (!isAtFinalElement)
                return this.Next(element);
            return false;
        }

        /// <summary>
        ///     Returns <c>True</c> if the iterator is at the first element at the given level.
        /// </summary>
        /// <remarks>
        ///     A possible use is to determin if a call to next(word) moved to the start of a new paragraph.
        /// </remarks>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool IsAtBeginningOf(PageIteratorLevel level)
        {
            this.ThrowIfDisposed();

            if (this.handle.Handle == IntPtr.Zero)
                return false;
            return TessApi.Native.PageIteratorIsAtBeginningOf(this.handle, level) != 0;
        }

        /// <summary>
        ///     Returns <c>True</c> if the iterator is positioned at the last element at the given level.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool IsAtFinalOf(PageIteratorLevel level, PageIteratorLevel element)
        {
            this.ThrowIfDisposed();

            if (this.handle.Handle == IntPtr.Zero)
                return false;
            return TessApi.Native.PageIteratorIsAtFinalElement(this.handle, level, element) != 0;
        }

        public Pix GetBinaryImage(PageIteratorLevel level)
        {
            this.ThrowIfDisposed();
            if (this.handle.Handle == IntPtr.Zero) return null;

            return Pix.Create(TessApi.Native.PageIteratorGetBinaryImage(this.handle, level));
        }

        public Pix GetImage(PageIteratorLevel level, int padding, out int x, out int y)
        {
            this.ThrowIfDisposed();
            if (this.handle.Handle == IntPtr.Zero)
            {
                x = 0;
                y = 0;

                return null;
            }

            IntPtr image = TessApi.Native.PageIteratorGetImage(this.handle, level, padding, this.page.Image.Handle, out x, out y);
            return Pix.Create(image);
        }

        /// <summary>
        ///     Gets the bounding rectangle of the current element at the given level.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public bool TryGetBoundingBox(PageIteratorLevel level, out Rect bounds)
        {
            this.ThrowIfDisposed();
            if (this.handle.Handle != IntPtr.Zero && TessApi.Native.PageIteratorBoundingBox(this.handle, level, out int x1, out int y1, out int x2, out int y2) != 0)
            {
                bounds = Rect.FromCoords(x1, y1, x2, y2);
                return true;
            }

            bounds = Rect.Empty;
            return false;
        }

        /// <summary>
        ///     Gets the baseline of the current element at the given level.
        /// </summary>
        /// <remarks>
        ///     The baseline is the line that passes through (x1, y1) and (x2, y2).
        ///     WARNING: with vertical text, baselines may be vertical! Returns false if there is no baseline at the current
        ///     position.
        /// </remarks>
        /// <param name="level"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public bool TryGetBaseline(PageIteratorLevel level, out Rect bounds)
        {
            this.ThrowIfDisposed();
            if (this.handle.Handle != IntPtr.Zero && TessApi.Native.PageIteratorBaseline(this.handle, level, out int x1, out int y1, out int x2, out int y2) != 0)
            {
                bounds = Rect.FromCoords(x1, y1, x2, y2);
                return true;
            }

            bounds = Rect.Empty;
            return false;
        }

        /// <summary>
        ///     Gets the element orientation information that the iterator currently points too.
        /// </summary>
        public ElementProperties GetProperties()
        {
            this.ThrowIfDisposed();
            if (this.handle.Handle == IntPtr.Zero) return new ElementProperties(Orientation.PageUp, TextLineOrder.TopToBottom, WritingDirection.LeftToRight, 0f);

            WritingDirection writing_direction;
            float deskew_angle;
            TessApi.Native.PageIteratorOrientation(this.handle, out Orientation orientation, out writing_direction, out TextLineOrder textLineOrder, out deskew_angle);

            return new ElementProperties(orientation, textLineOrder, writing_direction, deskew_angle);
        }

        protected override void Dispose(bool disposing)
        {
            if (this.handle.Handle != IntPtr.Zero) TessApi.Native.PageIteratorDelete(this.handle);
        }
    }
}