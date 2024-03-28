namespace Tesseract
{
    using System;
    using System.Diagnostics;

    public abstract class DisposableBase : IDisposable
    {
        private static readonly TraceSource trace = new("Tesseract");

        protected DisposableBase()
        {
            this.IsDisposed = false;
        }

        public bool IsDisposed { get; private set; }


        public void Dispose()
        {
            this.Dispose(true);

            this.IsDisposed = true;
            GC.SuppressFinalize(this);

            if (this.Disposed != null) this.Disposed(this, EventArgs.Empty);
        }

        ~DisposableBase()
        {
            this.Dispose(false);
            trace.TraceEvent(TraceEventType.Warning, 0, "{0} was not disposed off.", this);
        }

        public event EventHandler<EventArgs> Disposed;


        protected virtual void VerifyNotDisposed()
        {
            if (this.IsDisposed) throw new ObjectDisposedException(this.ToString());
        }

        protected abstract void Dispose(bool disposing);
    }
}