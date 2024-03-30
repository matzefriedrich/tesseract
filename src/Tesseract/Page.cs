namespace Tesseract
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    using Abstractions;

    using Interop;
    using Interop.Abstractions;

    using JetBrains.Annotations;

    public sealed class Page : DisposableBase
    {
        private readonly IManagedTesseractApi api;
        private readonly ITessApiSignatures nativeApi;
        private readonly ILeptonicaApiSignatures leptonicaNativeApi;
        private readonly IPixFactory pixFactory;
        private static readonly TraceSource trace = new("Tesseract");
        private Rect regionOfInterest;

        private bool runRecognitionPhase;

        internal Page(
            [NotNull] TesseractEngine engine, 
            [NotNull] IManagedTesseractApi api, 
            [NotNull] ITessApiSignatures nativeApi,
            [NotNull] ILeptonicaApiSignatures leptonicaNativeApi,
            [NotNull] IPixFactory pixFactory,
            Pix image, string imageName, Rect regionOfInterest, PageSegMode pageSegmentMode)
        {
            this.api = api ?? throw new ArgumentNullException(nameof(api));
            this.nativeApi = nativeApi ?? throw new ArgumentNullException(nameof(nativeApi));
            this.leptonicaNativeApi = leptonicaNativeApi ?? throw new ArgumentNullException(nameof(leptonicaNativeApi));
            this.pixFactory = pixFactory ?? throw new ArgumentNullException(nameof(pixFactory));
            this.Engine = engine ?? throw new ArgumentNullException(nameof(engine));
            this.Image = image;
            this.ImageName = imageName;
            this.RegionOfInterest = regionOfInterest;
            this.PageSegmentMode = pageSegmentMode;
        }

        public TesseractEngine Engine { get; }

        /// <summary>
        ///     Gets the <see cref="Pix" /> that is being ocr'd.
        /// </summary>
        public Pix Image { get; }

        /// <summary>
        ///     Gets the name of the image being ocr'd.
        /// </summary>
        /// <remarks>
        ///     This is also used for some of the more advanced functionality such as identifying the associated UZN file if
        ///     present.
        /// </remarks>
        public string ImageName { get; private set; }

        /// <summary>
        ///     Gets the page segmentation mode used to OCR the specified image.
        /// </summary>
        public PageSegMode PageSegmentMode { get; }

        /// <summary>
        ///     The current region of interest being parsed.
        /// </summary>
        [Obsolete("This property will be removed soon.")]
        public Rect RegionOfInterest
        {
            get => this.regionOfInterest;
            set
            {
                if (value.X1 < 0 || value.Y1 < 0 || value.X2 > this.Image.Width || value.Y2 > this.Image.Height)
                    throw new ArgumentException("The region of interest to be processed must be within the image bounds.", "value");

                if (this.regionOfInterest != value)
                {
                    this.regionOfInterest = value;

                    // update region of interest in image
                    this.nativeApi.BaseApiSetRectangle(this.Engine.Handle, this.regionOfInterest.X1, this.regionOfInterest.Y1, this.regionOfInterest.Width, this.regionOfInterest.Height);

                    // request rerun of recognition on the next call that requires recognition
                    this.runRecognitionPhase = false;
                }
            }
        }

        /// <summary>
        ///     Gets the thresholded image that was OCR'd.
        /// </summary>
        /// <returns></returns>
        public Pix GetThresholdedImage()
        {
            this.Recognize();

            IntPtr pixHandle = this.nativeApi.BaseAPIGetThresholdedImage(this.Engine.Handle);
            if (pixHandle == IntPtr.Zero) throw new TesseractException("Failed to get thresholded image.");

            return this.pixFactory.Create(pixHandle);
        }

        /// <summary>
        ///     Creates a <see cref="PageIterator" /> object that is used to iterate over the page's layout as defined by the
        ///     current <see cref="Page.RegionOfInterest" />.
        /// </summary>
        /// <returns></returns>
        public PageIterator AnalyseLayout()
        {
            if (this.PageSegmentMode == PageSegMode.OsdOnly) throw new ArgumentException("Cannot analyse image layout when using OSD only page segmentation, please use DetectBestOrientation instead.");

            IntPtr resultIteratorHandle = this.nativeApi.BaseAPIAnalyseLayout(this.Engine.Handle);
            return new PageIterator(this.nativeApi, this.pixFactory, this, resultIteratorHandle);
        }

        /// <summary>
        ///     Creates a <see cref="ResultIterator" /> object that is used to iterate over the page as defined by the current
        ///     <see cref="Page.RegionOfInterest" />.
        /// </summary>
        /// <returns></returns>
        public ResultIterator GetIterator()
        {
            this.Recognize();
            IntPtr resultIteratorHandle = this.nativeApi.BaseApiGetIterator(this.Engine.Handle);
            return new ResultIterator(this.api, this.nativeApi, this.pixFactory, this, resultIteratorHandle);
        }

        /// <summary>
        ///     Gets the page's content as plain text.
        /// </summary>
        /// <returns></returns>
        public string GetText(Rect regionOfInterest = default)
        {
            this.Recognize();
            return this.api.BaseAPIGetUTF8Text(this.Engine.Handle);
        }

        /// <summary>
        ///     Gets the page's content as an HOCR text.
        /// </summary>
        /// <param name="pageNum">The page number (zero based).</param>
        /// <param name="useXHtml">True to use XHTML Output, False to HTML Output</param>
        /// <returns>The OCR'd output as an HOCR text string.</returns>
        public string GetHOCRText(int pageNum, bool useXHtml = false)
        {
            if (pageNum < 0) throw new ArgumentException("Page number must be greater than or equal to zero (0).");
            this.Recognize();
            if (useXHtml)
                return this.api.BaseAPIGetHOCRText2(this.Engine.Handle, pageNum);
            return this.api.BaseAPIGetHOCRText(this.Engine.Handle, pageNum);
        }

        /// <summary>
        ///     Gets the page's content as an Alto text.
        /// </summary>
        /// <param name="pageNum">The page number (zero based).</param>
        /// <returns>The OCR'd output as an Alto text string.</returns>
        public string GetAltoText(int pageNum)
        {
            if (pageNum < 0) throw new ArgumentException("Page number must be greater than or equal to zero (0).");
            this.Recognize();
            return this.api.BaseAPIGetAltoText(this.Engine.Handle, pageNum);
        }

        /// <summary>
        ///     Gets the page's content as a Tsv text.
        /// </summary>
        /// <param name="pageNum">The page number (zero based).</param>
        /// <returns>The OCR'd output as a Tsv text string.</returns>
        public string GetTsvText(int pageNum)
        {
            if (pageNum < 0) throw new ArgumentException("Page number must be greater than or equal to zero (0).");
            this.Recognize();
            return this.api.BaseAPIGetTsvText(this.Engine.Handle, pageNum);
        }

        /// <summary>
        ///     Gets the page's content as a Box text.
        /// </summary>
        /// <param name="pageNum">The page number (zero based).</param>
        /// <returns>The OCR'd output as a Box text string.</returns>
        public string GetBoxText(int pageNum)
        {
            if (pageNum < 0) throw new ArgumentException("Page number must be greater than or equal to zero (0).");
            this.Recognize();
            return this.api.BaseAPIGetBoxText(this.Engine.Handle, pageNum);
        }

        /// <summary>
        ///     Gets the page's content as a LSTMBox text.
        /// </summary>
        /// <param name="pageNum">The page number (zero based).</param>
        /// <returns>The OCR'd output as a LSTMBox text string.</returns>
        public string GetLSTMBoxText(int pageNum)
        {
            if (pageNum < 0) throw new ArgumentException("Page number must be greater than or equal to zero (0).");
            this.Recognize();
            return this.api.BaseAPIGetLSTMBoxText(this.Engine.Handle, pageNum);
        }

        /// <summary>
        ///     Gets the page's content as a WordStrBox text.
        /// </summary>
        /// <param name="pageNum">The page number (zero based).</param>
        /// <returns>The OCR'd output as a WordStrBox text string.</returns>
        public string GetWordStrBoxText(int pageNum)
        {
            if (pageNum < 0) throw new ArgumentException("Page number must be greater than or equal to zero (0).");
            this.Recognize();
            return this.api.BaseAPIGetWordStrBoxText(this.Engine.Handle, pageNum);
        }

        /// <summary>
        ///     Gets the page's content as an UNLV text.
        /// </summary>
        /// <param name="pageNum">The page number (zero based).</param>
        /// <returns>The OCR'd output as an UNLV text string.</returns>
        public string GetUNLVText()
        {
            this.Recognize();
            return this.api.BaseAPIGetUNLVText(this.Engine.Handle);
        }

        /// <summary>
        ///     Get's the mean confidence that as a percentage of the recognized text.
        /// </summary>
        /// <returns></returns>
        public float GetMeanConfidence()
        {
            this.Recognize();
            return this.nativeApi.BaseAPIMeanTextConf(this.Engine.Handle) / 100.0f;
        }

        /// <summary>
        ///     Get segmented regions at specified page iterator level.
        /// </summary>
        /// <param name="pageIteratorLevel">PageIteratorLevel enum</param>
        /// <returns></returns>
        public List<Rectangle> GetSegmentedRegions(PageIteratorLevel pageIteratorLevel)
        {
            IntPtr boxArray = this.nativeApi.BaseAPIGetComponentImages(this.Engine.Handle, pageIteratorLevel, Constants.TRUE, IntPtr.Zero, IntPtr.Zero);
            int boxCount = this.leptonicaNativeApi.boxaGetCount(new HandleRef(this, boxArray));

            var boxList = new List<Rectangle>();

            for (var i = 0; i < boxCount; i++)
            {
                IntPtr box = this.leptonicaNativeApi.boxaGetBox(new HandleRef(this, boxArray), i, PixArrayAccessType.Clone);
                if (box == IntPtr.Zero) continue;

                this.leptonicaNativeApi.boxGetGeometry(new HandleRef(this, box), out int px, out int py, out int pw, out int ph);
                boxList.Add(new Rectangle(px, py, pw, ph));
                this.leptonicaNativeApi.boxDestroy(ref box);
            }

            this.leptonicaNativeApi.boxaDestroy(ref boxArray);

            return boxList;
        }

        /// <summary>
        ///     Detects the page orientation, with corresponding confidence when using <see cref="PageSegMode.OsdOnly" />.
        /// </summary>
        /// <remarks>
        ///     If using full page segmentation mode (i.e. AutoOsd) then consider using <see cref="AnalyseLayout" /> instead as
        ///     this also provides a
        ///     deskew angle which isn't available when just performing orientation detection.
        /// </remarks>
        /// <param name="orientation">The page orientation.</param>
        /// <param name="confidence">The confidence level of the orientation (15 is reasonably confident).</param>
        [Obsolete("Use DetectBestOrientation(int orientationDegrees, float confidence) that returns orientation in degrees instead.")]
        public void DetectBestOrientation(out Orientation orientation, out float confidence)
        {
            this.DetectBestOrientation(out int orientationDegrees, out float orientationConfidence);

            // convert angle to 0-360 (shouldn't be required but do it just o be safe).
            orientationDegrees = orientationDegrees % 360;
            if (orientationDegrees < 0) orientationDegrees += 360;

            if (orientationDegrees > 315 || orientationDegrees <= 45)
                orientation = Orientation.PageUp;
            else if (orientationDegrees > 45 && orientationDegrees <= 135)
                orientation = Orientation.PageRight;
            else if (orientationDegrees > 135 && orientationDegrees <= 225)
                orientation = Orientation.PageDown;
            else
                orientation = Orientation.PageLeft;

            confidence = orientationConfidence;
        }

        /// <summary>
        ///     Detects the page orientation, with corresponding confidence when using <see cref="PageSegMode.OsdOnly" />.
        /// </summary>
        /// <remarks>
        ///     If using full page segmentation mode (i.e. AutoOsd) then consider using <see cref="AnalyseLayout" /> instead as
        ///     this also provides a
        ///     deskew angle which isn't available when just performing orientation detection.
        /// </remarks>
        /// <param name="orientation">The detected clockwise page rotation in degrees (0, 90, 180, or 270).</param>
        /// <param name="confidence">The confidence level of the orientation (15 is reasonably confident).</param>
        public void DetectBestOrientation(out int orientation, out float confidence)
        {
            this.DetectBestOrientationAndScript(out orientation, out confidence, out string _, out float _);
        }

        /// <summary>
        ///     Detects the page orientation, with corresponding confidence when using <see cref="PageSegMode.OsdOnly" />.
        /// </summary>
        /// <remarks>
        ///     If using full page segmentation mode (i.e. AutoOsd) then consider using <see cref="AnalyseLayout" /> instead as
        ///     this also provides a
        ///     deskew angle which isn't available when just performing orientation detection.
        /// </remarks>
        /// <param name="orientation">The detected clockwise page rotation in degrees (0, 90, 180, or 270).</param>
        /// <param name="confidence">The confidence level of the orientation (15 is reasonably confident).</param>
        /// <param name="scriptName">
        ///     The name of the script (e.g. Latin)
        ///     <param>
        ///         <param name="scriptConfidence">The confidence level in the script</param>
        public void DetectBestOrientationAndScript(out int orientation, out float confidence, out string scriptName, out float scriptConfidence)
        {
            int orient_deg;
            float orient_conf;
            IntPtr script_nameHandle;
            float script_conf;

            if (this.nativeApi.TessBaseAPIDetectOrientationScript(this.Engine.Handle, out orient_deg, out orient_conf, out script_nameHandle, out script_conf) != 0)
            {
                orientation = orient_deg;
                confidence = orient_conf;
                if (script_nameHandle != IntPtr.Zero)
                    scriptName = MarshalHelper.PtrToString(script_nameHandle, Encoding.ASCII);
                // Don't delete script_nameHandle as it points to internal memory managed by Tesseract.
                else
                    scriptName = null;
                scriptConfidence = script_conf;
            }
            else
            {
                throw new TesseractException("Failed to detect image orientation.");
            }
        }

        internal void Recognize()
        {
            if (this.PageSegmentMode == PageSegMode.OsdOnly) throw new InvalidOperationException("Cannot OCR image when using OSD only page segmentation, please use DetectBestOrientation instead.");
            if (!this.runRecognitionPhase)
            {
                if (this.nativeApi.BaseApiRecognize(this.Engine.Handle, new HandleRef(this, IntPtr.Zero)) != 0) throw new InvalidOperationException("Recognition of image failed.");

                this.runRecognitionPhase = true;

                // now write out the thresholded image if required to do so
                if (this.Engine.TryGetBoolVariable("tessedit_write_images", out bool tesseditWriteImages) && tesseditWriteImages)
                {
                    using Pix thresholdedImage = this.GetThresholdedImage();
                    string filePath = Path.Combine(Environment.CurrentDirectory, "tessinput.tif");
                    try
                    {
                        const ImageFormat imageFormat = ImageFormat.TiffG4;
                        thresholdedImage.Save(filePath, imageFormat);
                        trace.TraceEvent(TraceEventType.Information, 2, "Successfully saved the thresholded image to '{0}'", filePath);
                    }
                    catch (Exception error)
                    {
                        trace.TraceEvent(TraceEventType.Error, 2, "Failed to save the thresholded image to '{0}'.\nError: {1}", filePath, error.Message);
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) this.nativeApi.BaseAPIClear(this.Engine.Handle);
        }
    }
}