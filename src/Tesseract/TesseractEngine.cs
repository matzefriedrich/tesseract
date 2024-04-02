namespace Tesseract
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Security;
    using Abstractions;
    using Internal;
    using Interop.Abstractions;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    ///     The tesseract OCR engine.
    /// </summary>
    public class TesseractEngine : DisposableBase, ITesseractEngine
    {
        private static readonly TraceSource trace = new("Tesseract");
        private readonly IManagedTesseractApi api;
        private readonly ILeptonicaApiSignatures leptonicaNativeApi;
        private readonly ITessApiSignatures native;
        private readonly TesseractEngineOptions options;
        private readonly IPixFactory pixFactory;
        private readonly IPixFileWriter pixFileWriter;

        private HandleRef handle;

        private int processCount;

        /// <summary>
        ///     Creates a new instance of <see cref="TesseractEngine" /> with the specified <paramref name="engineMode" /> and
        ///     <paramref name="configFiles" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The <paramref name="datapath" /> parameter should point to the directory that contains the 'tessdata' folder
        ///         for example if your tesseract language data is installed in <c>C:\Tesseract\tessdata</c> the value of datapath
        ///         should
        ///         be <c>C:\Tesseract</c>. Note that tesseract will use the value of the <c>TESSDATA_PREFIX</c> environment
        ///         variable if defined,
        ///         effectively ignoring the value of <paramref name="datapath" /> parameter.
        ///     </para>
        /// </remarks>
        /// <param name="api"></param>
        /// <param name="native"></param>
        /// <param name="pixFactory"></param>
        /// <param name="options">An <see cref="TesseractEngineOptions" /> value representing the Tesseract engine configuration.</param>
        public TesseractEngine(
            [NotNull] IManagedTesseractApi api,
            [NotNull] ITessApiSignatures native,
            [NotNull] ILeptonicaApiSignatures leptonicaNativeApi,
            [NotNull] IPixFactory pixFactory,
            [NotNull] IPixFileWriter pixFileWriter,
            TesseractEngineOptions options)
        {
            this.api = api ?? throw new ArgumentNullException(nameof(api));
            this.native = native ?? throw new ArgumentNullException(nameof(native));
            this.leptonicaNativeApi = leptonicaNativeApi ?? throw new ArgumentNullException(nameof(leptonicaNativeApi));
            this.pixFactory = pixFactory ?? throw new ArgumentNullException(nameof(pixFactory));
            this.pixFileWriter = pixFileWriter ?? throw new ArgumentNullException(nameof(pixFileWriter));
            this.options = options;

            this.DefaultPageSegMode = PageSegMode.Auto;
            this.handle = new HandleRef(this, this.native.BaseApiCreate());

            this.Initialize();
        }

        internal HandleRef Handle => this.handle;

        public string Version =>
            // Get version doesn't work for x64, might be compilation related for now just
            // return constant so we don't crash.
            this.api.BaseApiGetVersion();

        /// <summary>
        ///     Gets or sets default <see cref="PageSegMode" /> mode used by
        ///     <see cref="Tesseract.TesseractEngine.Process(Pix, Rect, PageSegMode?)" />.
        /// </summary>
        public PageSegMode DefaultPageSegMode { get; set; }

        /// <summary>
        ///     Processes the specific image.
        /// </summary>
        /// <remarks>
        ///     You can only have one result iterator open at any one time.
        /// </remarks>
        /// <param name="image">The image to process.</param>
        /// <param name="pageSegMode">The page layout analysis method to use.</param>
        public Page Process(Pix image, PageSegMode? pageSegMode = null)
        {
            return this.Process(image, null, new Rect(0, 0, image.Width, image.Height), pageSegMode);
        }

        /// <summary>
        ///     Processes a specified region in the image using the specified page layout analysis mode.
        /// </summary>
        /// <remarks>
        ///     You can only have one result iterator open at any one time.
        /// </remarks>
        /// <param name="image">The image to process.</param>
        /// <param name="region">The image region to process.</param>
        /// <param name="pageSegMode">The page layout analyasis method to use.</param>
        /// <returns>A result iterator</returns>
        public Page Process(Pix image, Rect region, PageSegMode? pageSegMode = null)
        {
            return this.Process(image, null, region, pageSegMode);
        }

        /// <summary>
        ///     Processes the specific image.
        /// </summary>
        /// <remarks>
        ///     You can only have one result iterator open at any one time.
        /// </remarks>
        /// <param name="image">The image to process.</param>
        /// <param name="inputName">Sets the input file's name, only needed for training or loading a uzn file.</param>
        /// <param name="pageSegMode">The page layout analyasis method to use.</param>
        public Page Process(Pix image, string inputName, PageSegMode? pageSegMode = null)
        {
            return this.Process(image, inputName, new Rect(0, 0, image.Width, image.Height), pageSegMode);
        }

        /// <summary>
        ///     Processes a specified region in the image using the specified page layout analysis mode.
        /// </summary>
        /// <remarks>
        ///     You can only have one result iterator open at any one time.
        /// </remarks>
        /// <param name="image">The image to process.</param>
        /// <param name="inputName">Sets the input file's name, only needed for training or loading a uzn file.</param>
        /// <param name="region">The image region to process.</param>
        /// <param name="pageSegMode">The page layout analyasis method to use.</param>
        /// <returns>A result iterator</returns>
        public Page Process(Pix image, string inputName, Rect region, PageSegMode? pageSegMode = null)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            if (region.X1 < 0 || region.Y1 < 0 || region.X2 > image.Width || region.Y2 > image.Height)
                throw new ArgumentException("The image region to be processed must be within the image bounds.", nameof(region));
            if (this.processCount > 0) throw new InvalidOperationException("Only one image can be processed at once. Please make sure you dispose of the page once your finished with it.");

            this.processCount++;

            PageSegMode actualPageSegmentMode = pageSegMode ?? this.DefaultPageSegMode;
            this.native.BaseAPISetPageSegMode(this.handle, actualPageSegmentMode);
            this.native.BaseApiSetImage(this.handle, image.Handle);
            if (!string.IsNullOrEmpty(inputName)) this.native.BaseApiSetInputName(this.handle, inputName);
            // TODO: do not create Page instance here; move this code into a factory (because of the dependencies which are owned by the current engine object)
            var page = new Page(this, this.api, this.native, this.leptonicaNativeApi, this.pixFactory, this.pixFileWriter, new NullLogger<Page>(), image, inputName, region, actualPageSegmentMode);
            page.Disposed += this.OnIteratorDisposed;
            return page;
        }

        public bool SetDebugVariable(string name, string value)
        {
            return this.api.BaseApiSetDebugVariable(this.handle, name, value) != 0;
        }

        /// <summary>
        ///     Sets the value of a string variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The new value of the variable.</param>
        /// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
        public bool SetVariable(string name, string value)
        {
            return this.api.BaseApiSetVariable(this.handle, name, value) != 0;
        }

        /// <summary>
        ///     Sets the value of a boolean variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The new value of the variable.</param>
        /// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
        public bool SetVariable(string name, bool value)
        {
            string strEncodedValue = value ? "TRUE" : "FALSE";
            return this.api.BaseApiSetVariable(this.handle, name, strEncodedValue) != 0;
        }

        /// <summary>
        ///     Sets the value of a integer variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The new value of the variable.</param>
        /// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
        public bool SetVariable(string name, int value)
        {
            var strEncodedValue = value.ToString("D", CultureInfo.InvariantCulture.NumberFormat);
            return this.api.BaseApiSetVariable(this.handle, name, strEncodedValue) != 0;
        }

        /// <summary>
        ///     Sets the value of a double variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The new value of the variable.</param>
        /// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
        public bool SetVariable(string name, double value)
        {
            var strEncodedValue = value.ToString("R", CultureInfo.InvariantCulture.NumberFormat);
            return this.api.BaseApiSetVariable(this.handle, name, strEncodedValue) != 0;
        }

        /// <summary>
        ///     Attempts to retrieve the value for a boolean variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The current value of the variable.</param>
        /// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
        public bool TryGetBoolVariable(string name, out bool value)
        {
            if (this.native.BaseApiGetBoolVariable(this.handle, name, out int val) != 0)
            {
                value = val != 0;
                return true;
            }

            value = false;
            return false;
        }

        /// <summary>
        ///     Attempts to retrieve the value for a double variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The current value of the variable.</param>
        /// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
        public bool TryGetDoubleVariable(string name, out double value)
        {
            return this.native.BaseApiGetDoubleVariable(this.handle, name, out value) != 0;
        }

        /// <summary>
        ///     Attempts to retrieve the value for an integer variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The current value of the variable.</param>
        /// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
        public bool TryGetIntVariable(string name, out int value)
        {
            return this.native.BaseApiGetIntVariable(this.handle, name, out value) != 0;
        }

        /// <summary>
        ///     Attempts to retrieve the value for a string variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The current value of the variable.</param>
        /// <returns>Returns <c>True</c> if successful; otherwise <c>False</c>.</returns>
        public bool TryGetStringVariable(string name, out string value)
        {
            value = this.api.BaseApiGetStringVariable(this.handle, name);
            return value != null;
        }

        /// <summary>
        ///     Attempts to print the variables to the file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool TryPrintVariablesToFile(string filename)
        {
            return this.native.BaseApiPrintVariablesToFile(this.handle, filename) != 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (this.handle.Handle != IntPtr.Zero)
            {
                this.native.BaseApiDelete(this.handle);
                this.handle = new HandleRef(this, IntPtr.Zero);
            }
        }

        private string GetTessDataPrefix()
        {
            try
            {
                return Environment.GetEnvironmentVariable("TESSDATA_PREFIX");
            }
            catch (SecurityException e)
            {
                trace.TraceEvent(TraceEventType.Error, 0, "Failed to detect if the environment variable 'TESSDATA_PREFIX' is set: {0}", e.Message);
                return null;
            }
        }

        private void Initialize()
        {
            // do some minor processing on datapath to fix some common errors (this basically mirrors what tesseract does as of 3.04)
            string dataPath = this.options.DataPath;
            if (!string.IsNullOrEmpty(dataPath))
            {
                // remove any excess whitespace
                dataPath = dataPath.Trim();

                // remove any trialing '\' or '/' characters
                if (dataPath.EndsWith("\\", StringComparison.Ordinal) || dataPath.EndsWith("/", StringComparison.Ordinal))
                    dataPath = dataPath.Substring(0, dataPath.Length - 1);
            }

            var engineMode = (int)this.options.Mode;
            string language = this.options.Language;
            IEnumerable<string> configurationFiles = this.options.ConfigurationFiles;
            IDictionary<string, object> initialOptions = this.options.InitialOptions;
            bool setOnlyNonDebugVariables = this.options.SetOnlyNonDebugVariables;

            if (this.api.BaseApiInit(this.handle, dataPath, language, engineMode, configurationFiles, initialOptions, setOnlyNonDebugVariables) != 0)
            {
                // Special case logic to handle cleaning up as init has already released the handle if it fails.
                this.handle = new HandleRef(this, IntPtr.Zero);
                GC.SuppressFinalize(this);

                throw new TesseractException(ErrorMessage.Format(1, "Failed to initialise tesseract engine."));
            }
        }

        private void OnIteratorDisposed(object sender, EventArgs e)
        {
            this.processCount--;
        }

        /// <summary>
        ///     Ties the specified pix to the lifecycle of a page.
        /// </summary>
        public class PageDisposalHandle
        {
            private readonly Page page;
            private readonly Pix pix;

            public PageDisposalHandle(Page page, Pix pix)
            {
                this.page = page;
                this.pix = pix;
                page.Disposed += this.OnPageDisposed;
            }

            private void OnPageDisposed(object sender, EventArgs e)
            {
                this.page.Disposed -= this.OnPageDisposed;
                // dispose the pix when the page is disposed.
                this.pix.Dispose();
            }
        }
    }
}