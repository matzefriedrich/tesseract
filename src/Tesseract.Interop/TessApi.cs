namespace Tesseract.Interop
{
    using System.Runtime.InteropServices;
    using System.Text;

    using Abstractions;

    using Internal;

    using InteropDotNet;

    internal static class TessApi
    {
        //XHTML Begin Tag:
        public const string xhtmlBeginTag =
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n"
            + "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\"\n"
            + "    \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n"
            + "<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\" "
            + "lang=\"en\">\n <head>\n  <title></title>\n"
            + "<meta http-equiv=\"Content-Type\" content=\"text/html;"
            + "charset=utf-8\" />\n"
            + "  <meta name='ocr-system' content='tesseract' />\n"
            + "  <meta name='ocr-capabilities' content='ocr_page ocr_carea ocr_par"
            + " ocr_line ocrx_word"
            + "'/>\n"
            + "</head>\n<body>\n";

        //XHTML End Tag:
        public const string xhtmlEndTag = " </body>\n</html>\n";

        public const string htmlBeginTag =
            "<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\""
            + " \"http://www.w3.org/TR/html4/loose.dtd\">\n"
            + "<html>\n<head>\n<title></title>\n"
            + "<meta http-equiv=\"Content-Type\" content=\"text/html;"
            + "charset=utf-8\" />\n<meta name='ocr-system' content='tesseract'/>\n"
            + "</head>\n<body>\n";

        public const string htmlEndTag = "</body>\n</html>\n";

        private static ITessApiSignatures? native;

        public static ITessApiSignatures? Native
        {
            get
            {
                if (native == null)
                    Initialize();
                return native;
            }
        }

        public static string? BaseApiGetVersion()
        {
            IntPtr? versionHandle = Native?.GetVersion();
            if (versionHandle != null && versionHandle.Value != IntPtr.Zero) return MarshalHelper.PtrToString(versionHandle, Encoding.UTF8);

            return null;
        }

        public static string? BaseAPIGetHOCRText(HandleRef handle, int pageNum)
        {
            IntPtr? txtHandle = Native?.BaseApiGetHOCRTextInternal(handle, pageNum);
            if (txtHandle != null && txtHandle.Value != IntPtr.Zero)
            {
                string? result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
                Native?.DeleteText(txtHandle.Value);
                return htmlBeginTag + result + htmlEndTag;
            }

            return null;
        }

        //Just Copied:
        public static string? BaseAPIGetHOCRText2(HandleRef handle, int pageNum)
        {
            IntPtr? txtHandle = Native?.BaseApiGetHOCRTextInternal(handle, pageNum);
            if (txtHandle != null && txtHandle != IntPtr.Zero)
            {
                string? result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
                Native?.DeleteText(txtHandle.Value);
                return xhtmlBeginTag + result + xhtmlEndTag;
            }

            return null;
        }

        public static string? BaseAPIGetAltoText(HandleRef handle, int pageNum)
        {
            IntPtr? txtHandle = Native?.BaseApiGetAltoTextInternal(handle, pageNum);
            if (txtHandle != null && txtHandle != IntPtr.Zero)
            {
                string? result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
                Native?.DeleteText(txtHandle.Value);
                return result;
            }

            return null;
        }

        public static string? BaseAPIGetTsvText(HandleRef handle, int pageNum)
        {
            IntPtr? txtHandle = Native?.BaseApiGetTsvTextInternal(handle, pageNum);
            if (txtHandle != null && txtHandle != IntPtr.Zero)
            {
                string? result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
                Native?.DeleteText(txtHandle.Value);
                return result;
            }

            return null;
        }

        public static string? BaseAPIGetBoxText(HandleRef handle, int pageNum)
        {
            IntPtr? txtHandle = Native?.BaseApiGetBoxTextInternal(handle, pageNum);
            if (txtHandle != null && txtHandle != IntPtr.Zero)
            {
                string? result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
                Native?.DeleteText(txtHandle.Value);
                return result;
            }

            return null;
        }

        public static string? BaseAPIGetLSTMBoxText(HandleRef handle, int pageNum)
        {
            IntPtr? txtHandle = Native?.BaseApiGetLSTMBoxTextInternal(handle, pageNum) ?? throw new ArgumentNullException("Native?.BaseApiGetLSTMBoxTextInternal(handle, pageNum)");
            if (txtHandle != IntPtr.Zero)
            {
                string? result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
                Native.DeleteText(txtHandle.Value);
                return result;
            }

            return null;
        }

        public static string? BaseAPIGetWordStrBoxText(HandleRef handle, int pageNum)
        {
            IntPtr? txtHandle = Native?.BaseApiGetWordStrBoxTextInternal(handle, pageNum);
            if (txtHandle != null && txtHandle != IntPtr.Zero)
            {
                string? result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
                Native?.DeleteText(txtHandle.Value);
                return result;
            }

            return null;
        }

        public static string? BaseAPIGetUNLVText(HandleRef handle)
        {
            IntPtr? txtHandle = Native?.BaseApiGetUNLVTextInternal(handle);
            if (txtHandle != null && txtHandle != IntPtr.Zero)
            {
                string? result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
                Native?.DeleteText(txtHandle.Value);
                return result;
            }

            return null;
        }

        public static string? BaseApiGetStringVariable(HandleRef handle, string name)
        {
            IntPtr? resultHandle = Native?.BaseApiGetStringVariableInternal(handle, name);
            if (resultHandle != null && resultHandle != IntPtr.Zero)
                return MarshalHelper.PtrToString(resultHandle, Encoding.UTF8);
            return null;
        }

        public static string? BaseAPIGetUTF8Text(HandleRef handle)
        {
            IntPtr? txtHandle = Native?.BaseAPIGetUTF8TextInternal(handle);
            if (txtHandle != null && txtHandle != IntPtr.Zero)
            {
                string? result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
                Native?.DeleteText(txtHandle.Value);
                return result;
            }

            return null;
        }

        public static int? BaseApiInit(HandleRef handle, string datapath, string language, int mode, IEnumerable<string> configFiles, IDictionary<string, object> initialValues, bool setOnlyNonDebugParams)
        {
            if (handle.Handle == IntPtr.Zero) throw new ArgumentException("Handle for BaseApi, created through BaseApiCreate is required.");
            if (string.IsNullOrWhiteSpace(language)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(language));
            if (configFiles == null) throw new ArgumentNullException(nameof(configFiles));
            if (initialValues == null) throw new ArgumentNullException(nameof(initialValues));

            string[] configFilesArray = new List<string>(configFiles).ToArray();

            var varNames = new string[initialValues.Count];
            var varValues = new string[initialValues.Count];
            var i = 0;
            foreach (KeyValuePair<string, object> pair in initialValues)
            {
                if (string.IsNullOrWhiteSpace(pair.Key)) throw new ArgumentException("Variable must have a name.");
                if (pair.Value == null) throw new ArgumentException($"Variable '{pair.Key}': The type '{pair.Value?.GetType()}' is not supported.");
                varNames[i] = pair.Key;
                if (TessConvert.TryToString(pair.Value, out string? varValue))
                    varValues[i] = varValue!;
                else
                    throw new ArgumentException($"Variable '{pair.Key}': The type '{pair.Value.GetType()}' is not supported.", "initialValues");
                i++;
            }

            var varsVecSize = new UIntPtr((uint)varNames.Length);
            return Native?.BaseApiInit(handle, datapath, language, mode,
                configFilesArray, configFilesArray.Length,
                varNames, varValues,
                varsVecSize, setOnlyNonDebugParams);
        }

        public static int? BaseApiSetDebugVariable(HandleRef handle, string name, string value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = MarshalHelper.StringToPtr(value, Encoding.UTF8);
                return Native?.BaseApiSetDebugVariable(handle, name, valuePtr);
            }
            finally
            {
                if (valuePtr != IntPtr.Zero) Marshal.FreeHGlobal(valuePtr);
            }
        }

        public static int? BaseApiSetVariable(HandleRef handle, string name, string value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = MarshalHelper.StringToPtr(value, Encoding.UTF8);
                return Native?.BaseApiSetVariable(handle, name, valuePtr);
            }
            finally
            {
                if (valuePtr != IntPtr.Zero) Marshal.FreeHGlobal(valuePtr);
            }
        }

        public static void Initialize()
        {
            if (native == null)
            {
                LeptonicaApi.Initialize();
                native = InteropRuntimeImplementer.CreateInstance<ITessApiSignatures>();
            }
        }

        public static string? ResultIteratorWordRecognitionLanguage(HandleRef handle)
        {
            // per docs (ltrresultiterator.h:118 as of 4897796 in github:tesseract-ocr/tesseract)
            // this return value should *NOT* be deleted.
            IntPtr? txtHandle = Native?.ResultIteratorWordRecognitionLanguageInternal(handle);

            return txtHandle != null && txtHandle != IntPtr.Zero
                ? MarshalHelper.PtrToString(txtHandle, Encoding.UTF8)
                : null;
        }

        public static string? ResultIteratorGetUTF8Text(HandleRef handle, PageIteratorLevel level)
        {
            IntPtr? txtHandle = Native?.ResultIteratorGetUTF8TextInternal(handle, level);
            if (txtHandle != null && txtHandle != IntPtr.Zero)
            {
                string? result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
                Native?.DeleteText(txtHandle.Value);
                return result;
            }

            return null;
        }

        /// <summary>
        ///     Returns the null terminated UTF-8 encoded text string for the current choice
        /// </summary>
        /// <remarks>
        ///     NOTE: Unlike LTRResultIterator::GetUTF8Text, the return points to an
        ///     internal structure and should NOT be delete[]ed to free after use.
        /// </remarks>
        /// <param name="choiceIteratorHandle"></param>
        /// <returns>string</returns>
        internal static string? ChoiceIteratorGetUTF8Text(HandleRef choiceIteratorHandle)
        {
            if (choiceIteratorHandle.Handle == IntPtr.Zero) throw new ArgumentException("ChoiceIterator Handle cannot be a null IntPtr and is required");
            IntPtr? txtChoiceHandle = Native?.ChoiceIteratorGetUTF8TextInternal(choiceIteratorHandle);
            return MarshalHelper.PtrToString(txtChoiceHandle, Encoding.UTF8);
        }

        // hOCR Extension
    }
}