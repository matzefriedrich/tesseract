namespace Tesseract.Rendering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Abstractions;
    using JetBrains.Annotations;
    using Tesseract.Abstractions;

    public class Document : DisposableBase
    {
        private readonly IList<UnmanagedDocument> children = new List<UnmanagedDocument>();
        private readonly ReaderWriterLockSlim lockSlim = new();
        
        public int NumPages { get; private set; } = -1;

        /// <summary>
        ///     Adds a page to each of the child result renderers.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public bool AddPage(Page page)
        {
            if (page == null) throw new ArgumentNullException(nameof(page));
            this.ThrowIfDisposed();

            this.NumPages++;
            foreach (UnmanagedDocument document in this.children)
            {
                bool success = document.AddPage(page);
                if (!success)
                    return false;
            }

            return true;
        }

        public void AddChild([NotNull] UnmanagedDocument child)
        {
            if (child == null) throw new ArgumentNullException(nameof(child));

            this.ExecuteProtectedWrite(ProtectedAppendChild);
            return;

            void ProtectedAppendChild()
            {
                this.children.Add(child);
            }
        }

        private void ExecuteProtectedWrite(Action cb)
        {
            this.lockSlim.EnterWriteLock();
            try
            {
                cb();
            }
            finally
            {
                this.lockSlim.ExitWriteLock();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (this.IsDisposed == false && disposing)
            {
                this.ExecuteProtectedWrite(this.DisposeChildren);
            }
        }

        private void DisposeChildren()
        {
            while (this.children.Any())
            {
                UnmanagedDocument next = this.children[0];
                next.Dispose();
                this.children.RemoveAt(0);
            }
        }
    }
}