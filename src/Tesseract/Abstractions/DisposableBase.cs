namespace Tesseract.Abstractions
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using static System.GC;

    public abstract class DisposableBase : IDisposable
    {
        private static readonly TraceSource Trace = new("Tesseract");
        private static readonly object DisposedEventKey = new();
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
            Trace.TraceEvent(TraceEventType.Warning, 0, "{0} was not disposed off.", this);
        }

        private void InvokeDisposed(EventArgs e)
        {
            lock (this.lockObject)
            {
                var handler = this.events[DisposedEventKey] as EventHandler<EventArgs>;
                handler?.Invoke(this, e);
            }
        }

        public event EventHandler<EventArgs> Disposed
        {
            add
            {
                lock (this.lockObject)
                {
                    this.events.AddHandler(DisposedEventKey, value);
                }
            }
            remove
            {
                lock (this.lockObject)
                {
                    this.events.RemoveHandler(DisposedEventKey, value);
                }
            }
        }

        protected void ThrowIfDisposed()
        {
            if (this.IsDisposed) throw new ObjectDisposedException(this.ToString());
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}