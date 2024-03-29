namespace Tesseract
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;

    using static System.GC;

    public abstract class DisposableBase : IDisposable
    {
        private static readonly TraceSource trace = new("Tesseract");
        private static readonly object disposedEventKey = new();
        private readonly EventHandlerList events = new();
        private readonly object lockObject = new();

        protected bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (this.IsDisposed == false)
            {
                this.Dispose(true);

                this.IsDisposed = true;
                SuppressFinalize(this);

                this.InvokeDisposed(EventArgs.Empty);
                this.events.Dispose();
                return;
            }

            this.Dispose(false);
        }

        ~DisposableBase()
        {
            this.Dispose(false);
            trace.TraceEvent(TraceEventType.Warning, 0, "{0} was not disposed off.", this);
        }

        private void InvokeDisposed(EventArgs e)
        {
            lock (this.lockObject)
            {
                var handler = this.events[disposedEventKey] as EventHandler<EventArgs>;
                handler?.Invoke(this, e);
            }
        }

        public event EventHandler<EventArgs> Disposed
        {
            add
            {
                lock (this.lockObject)
                {
                    this.events.AddHandler(disposedEventKey, value);
                }
            }
            remove
            {
                lock (this.lockObject)
                {
                    this.events.RemoveHandler(disposedEventKey, value);
                }
            }
        }

        protected void ThrowIfDisposed()
        {
            if (this.IsDisposed) throw new ObjectDisposedException(this.ToString());
        }

        protected abstract void Dispose(bool disposing);
    }
}