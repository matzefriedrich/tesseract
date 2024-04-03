namespace Tesseract
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using Abstractions;
    using Interop;
    using Interop.Abstractions;

    /// <summary>
    ///     The tesseract OCR engine.
    /// </summary>
    public class TesseractEngine : DisposableBase, ITesseractEngine
    {
        private readonly IManagedTesseractApi api;
        private readonly ITessApiSignatures native;
        private readonly TesseractEngineOptions options;

        private HandleRef handle;
        
        /// <summary>
        ///     Creates a new instance of <see cref="TesseractEngine" /> with the specified <paramref name="options" />.
        /// </summary>
        /// <param name="api">An <see cref="IManagedTesseractApi" /> object providing access to the managed Tesseract API..</param>
        /// <param name="native">An <see cref="ITessApiSignatures" /> object providing access to the raw Tesseract API.</param>
        /// <param name="options">An <see cref="TesseractEngineOptions" /> value representing the Tesseract engine configuration.</param>
        public TesseractEngine(
            IManagedTesseractApi api,
            ITessApiSignatures native,
            TesseractEngineOptions options)
        {
            this.api = api ?? throw new ArgumentNullException(nameof(api));
            this.native = native ?? throw new ArgumentNullException(nameof(native));
            this.options = options;

            this.DefaultPageSegMode = PageSegMode.Auto;
            this.handle = new HandleRef(this, this.native.Create());

            this.Initialize();
        }

        public HandleRef Handle => this.handle;

        public string? Version => this.api.GetVersion();

        /// <inheritdoc />
        public PageSegMode DefaultPageSegMode { get; set; }

        public bool SetDebugVariable(string name, string value)
        {
            return this.api.SetDebugVariable(this.handle, name, value) != 0;
        }

        /// <inheritdoc />
        public bool SetVariable(string name, string value)
        {
            return this.api.SetVariable(this.handle, name, value) != 0;
        }

        /// <inheritdoc />
        public bool SetVariable(string name, bool value)
        {
            string strEncodedValue = value ? Constants.TrueString : Constants.FalseString;
            return this.api.SetVariable(this.handle, name, strEncodedValue) != 0;
        }

        /// <inheritdoc />
        public bool SetVariable(string name, int value)
        {
            var strEncodedValue = value.ToString("D", CultureInfo.InvariantCulture.NumberFormat);
            return this.api.SetVariable(this.handle, name, strEncodedValue) != 0;
        }

        /// <inheritdoc />
        public bool SetVariable(string name, double value)
        {
            var strEncodedValue = value.ToString("R", CultureInfo.InvariantCulture.NumberFormat);
            return this.api.SetVariable(this.handle, name, strEncodedValue) != 0;
        }

        /// <inheritdoc />
        public bool TryGetBoolVariable(string name, out bool value)
        {
            if (this.native.GetBoolVariable(this.handle, name, out int val) != 0)
            {
                value = val != 0;
                return true;
            }

            value = false;
            return false;
        }

        /// <inheritdoc />
        public bool TryGetDoubleVariable(string name, out double value)
        {
            return this.native.GetDoubleVariable(this.handle, name, out value) != 0;
        }

        /// <inheritdoc />
        public bool TryGetIntVariable(string name, out int value)
        {
            return this.native.GetIntVariable(this.handle, name, out value) != 0;
        }

        /// <inheritdoc />
        public bool TryGetStringVariable(string name, out string? value)
        {
            value = this.api.GetStringVariable(this.handle, name);
            return value != null;
        }

        /// <inheritdoc />
        public bool TryPrintVariablesToFile(string filename)
        {
            return this.native.PrintVariablesToFile(this.handle, filename) != 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (this.IsDisposed == false && disposing)
                if (this.handle.Handle != IntPtr.Zero)
                {
                    this.native.Clear(this.handle);
                    this.native.Delete(this.handle);
                    this.handle = new HandleRef(this, IntPtr.Zero);
                }

            base.Dispose(disposing);
        }

        private void Initialize()
        {
            var engineMode = (int)this.options.Mode;
            string language = this.options.Language;
            IEnumerable<string> configurationFiles = this.options.ConfigurationFiles;
            IDictionary<string, object> initialOptions = this.options.InitialOptions;
            bool setOnlyNonDebugVariables = this.options.SetOnlyNonDebugVariables;

            int? result = this.api.Initialize(this.handle, this.options.DataPath, language, engineMode, configurationFiles, initialOptions, setOnlyNonDebugVariables);
            if (result is null or 0) return;
            // Special case logic to handle cleaning up as init has already released the handle if it fails.
            this.handle = new HandleRef(this, IntPtr.Zero);
            GC.SuppressFinalize(this);

            throw new TesseractException(Resources.Resources.TesseractEngine_Initialize_Failed_to_initialize_the_Tesseract_engine_);
        }
    }
}