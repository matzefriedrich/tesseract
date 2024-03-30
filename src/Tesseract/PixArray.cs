namespace Tesseract
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    using Abstractions;

    using Interop.Abstractions;

    using JetBrains.Annotations;

    /// <summary>
    ///     Represents an array of <see cref="Pix" />.
    /// </summary>
    public class PixArray : DisposableBase, IEnumerable<Pix>
    {
        private readonly ILeptonicaApiSignatures leptonicaApi;
        private readonly IPixFactory pixFactory;
        private readonly int version;

        /// <summary>
        ///     Gets the handle to the underlying PixA structure.
        /// </summary>
        private HandleRef _handle;

        private int count;

        internal PixArray([NotNull] ILeptonicaApiSignatures leptonicaApi, [NotNull] IPixFactory pixFactory, IntPtr handle)
        {
            this.leptonicaApi = leptonicaApi ?? throw new ArgumentNullException(nameof(leptonicaApi));
            this.pixFactory = pixFactory ?? throw new ArgumentNullException(nameof(pixFactory));

            this._handle = new HandleRef(this, handle);
            this.version = 1;

            // These will need to be updated whenever the PixA structure changes (i.e. a Pix is added or removed) though at the moment that isn't a problem.
            this.count = this.leptonicaApi.pixaGetCount(this._handle);
        }

        /// <summary>
        ///     Gets the number of <see cref="Pix" /> contained in the array.
        /// </summary>
        public int Count
        {
            get
            {
                this.ThrowIfDisposed();
                return this.count;
            }
        }

        /// <summary>
        ///     Returns a <see cref="IEnumerator{Pix}" /> that iterates the the array of <see cref="Pix" />.
        /// </summary>
        /// <remarks>
        ///     When done with the enumerator you must call <see cref="Dispose" /> to release any unmanaged resources.
        ///     However if your using the enumerator in a foreach loop, this is done for you automatically by .Net. This also means
        ///     that any <see cref="Pix" /> returned from the enumerator cannot safely be used outside a foreach loop (or after
        ///     Dispose has been
        ///     called on the enumerator). If you do indeed need the pix after the enumerator has been disposed of you must clone
        ///     it using
        ///     <see cref="Pix.Clone()" />.
        /// </remarks>
        /// <returns>A <see cref="IEnumerator{Pix}" /> that iterates the the array of <see cref="Pix" />.</returns>
        public IEnumerator<Pix> GetEnumerator()
        {
            return new PixArrayEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new PixArrayEnumerator(this);
        }

        /// <summary>
        ///     Add the specified pix to the end of the pix array.
        /// </summary>
        /// <remarks>
        ///     PixArrayAccessType.Insert is not supported as the managed Pix object will attempt to release the pix when
        ///     it goes out of scope creating an access exception.
        /// </remarks>
        /// <param name="pix">The pix to add.</param>
        /// <param name="copyflag">Determines if a clone or copy of the pix is inserted into the array.</param>
        /// <returns></returns>
        public bool Add(Pix pix, PixArrayAccessType copyflag = PixArrayAccessType.Clone)
        {
            ArgumentNullException.ThrowIfNull(pix);
            if (copyflag != PixArrayAccessType.Clone && copyflag != PixArrayAccessType.Copy) throw new ArgumentException($"Copy flag must be either copy or clone but was {copyflag}.");

            int result = this.leptonicaApi.pixaAddPix(this._handle, pix.Handle, copyflag);
            if (result == 0) this.count = this.leptonicaApi.pixaGetCount(this._handle);
            return result == 0;
        }

        /// <summary>
        ///     Removes the pix located at index.
        /// </summary>
        /// <remarks>
        ///     Notes:
        ///     * This shifts pixa[i] --> pixa[i - 1] for all i > index.
        ///     * Do not use on large arrays as the functionality is O(n).
        ///     * The corresponding box is removed as well, if it exists.
        /// </remarks>
        /// <param name="index">The index of the pix to remove.</param>
        public void Remove(int index)
        {
            if (index < 0 || index >= this.Count) throw new ArgumentOutOfRangeException(nameof(index), $"The index {index} must be between 0 and {this.Count}.");

            this.ThrowIfDisposed();

            int d = this.leptonicaApi.pixaRemovePix(this._handle, index);
            if (d == 0) this.count = this.leptonicaApi.pixaGetCount(this._handle);
        }

        /// <summary>
        ///     Destroys ever pix in the array.
        /// </summary>
        public void Clear()
        {
            this.ThrowIfDisposed();
            if (this.leptonicaApi.pixaClear(this._handle) == 0) this.count = this.leptonicaApi.pixaGetCount(this._handle);
        }

        /// <summary>
        ///     Gets the <see cref="Pix" /> located at <paramref name="index" /> using the specified <paramref name="accessType" />
        ///     .
        /// </summary>
        /// <param name="index">The index of the pix (zero based).</param>
        /// <param name="accessType">
        ///     The <see cref="PixArrayAccessType" /> used to retrieve the <see cref="Pix" />, only Clone or
        ///     Copy are allowed.
        /// </param>
        /// <returns>The retrieved <see cref="Pix" />.</returns>
        public Pix GetPix(int index, PixArrayAccessType accessType = PixArrayAccessType.Clone)
        {
            if (accessType != PixArrayAccessType.Clone && accessType != PixArrayAccessType.Copy) throw new ArgumentException($"Access type must be either copy or clone but was {accessType}.");
            if (index < 0 || index >= this.Count) throw new ArgumentOutOfRangeException(nameof(index), $"The index {index} must be between 0 and {this.Count}.");

            this.ThrowIfDisposed();

            IntPtr pixHandle = this.leptonicaApi.pixaGetPix(this._handle, index, accessType);
            if (pixHandle == IntPtr.Zero) throw new InvalidOperationException($"Failed to retrieve pix {pixHandle}.");
            return this.pixFactory.Create(pixHandle);
        }

        protected override void Dispose(bool disposing)
        {
            IntPtr handle = this._handle.Handle;
            this.leptonicaApi.pixaDestroy(ref handle);
            this._handle = new HandleRef(this, handle);
        }

        /// <summary>
        ///     Handles enumerating through the <see cref="Pix" /> in the PixArray.
        /// </summary>
        private class PixArrayEnumerator : DisposableBase, IEnumerator<Pix>
        {
            #region Constructor

            public PixArrayEnumerator(PixArray array)
            {
                this.array = array;
                this.version = array.version;
                this.items = new Pix[array.Count];
                this.index = 0;
                this.current = null;
            }

            #endregion

            #region Disposal

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                    for (var i = 0; i < this.items.Length; i++)
                    {
                        if (this.items[i] != null)
                        {
                            this.items[i].Dispose();
                            this.items[i] = null;
                        }
                    }
            }

            #endregion

            #region Fields

            private readonly PixArray array;
            private readonly Pix[] items;
            private Pix current;
            private int index;
            private readonly int version;

            #endregion

            #region Enumerator Implementation

            /// <inheritdoc />
            public bool MoveNext()
            {
                this.VerifyArrayUnchanged();
                this.ThrowIfDisposed();

                if (this.index < this.items.Length)
                {
                    if (this.items[this.index] == null) this.items[this.index] = this.array.GetPix(this.index);
                    this.current = this.items[this.index];
                    this.index++;
                    return true;
                }

                this.index = this.items.Length + 1;
                this.current = null;
                return false;
            }

            /// <inheritdoc />
            public Pix Current
            {
                get
                {
                    this.VerifyArrayUnchanged();
                    this.ThrowIfDisposed();

                    return this.current;
                }
            }

            // IEnumerator imp

            /// <inheritdoc />
            void IEnumerator.Reset()
            {
                this.VerifyArrayUnchanged();
                this.ThrowIfDisposed();

                this.index = 0;
                this.current = null;
            }

            /// <inheritdoc />
            object IEnumerator.Current
            {
                get
                {
                    // note: Only the non-generic requires an exception check according the MSDN docs (Generic version just undefined if it's not currently pointing to an item). Go figure.
                    if (this.index == 0 || this.index == this.items.Length + 1) throw new InvalidOperationException("The enumerator is positioned either before the first item or after the last item .");

                    return this.Current;
                }
            }

            // Helpers

            /// <inheritdoc />
            private void VerifyArrayUnchanged()
            {
                if (this.version != this.array.version) throw new InvalidOperationException("PixArray was modified; enumeration operation may not execute.");
            }

            #endregion
        }
    }
}