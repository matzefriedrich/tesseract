namespace Tesseract.Interop
{
    using System.Runtime.InteropServices;
    using System.Text;

    using Abstractions;

    using Internal;
    
    internal sealed class TessApi : IManagedTesseractApi
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

        private readonly ITessApiSignatures tesseractApiSignatures;

        public TessApi(
            ITessApiSignatures tesseractApiSignature)
        {
            this.tesseractApiSignatures = tesseractApiSignature ?? throw new ArgumentNullException(nameof(tesseractApiSignature));
        }

        public string? BaseApiGetVersion()
        {
            IntPtr versionHandle = this.tesseractApiSignatures.GetVersion();
            if (versionHandle != IntPtr.Zero) return MarshalHelper.PtrToString(versionHandle, Encoding.UTF8);

            return null;
        }

        public string? BaseAPIGetHOCRText(HandleRef handle, int pageNum)
        {
            IntPtr txtHandle = this.tesseractApiSignatures.BaseApiGetHOCRTextInternal(handle, pageNum);
            if (txtHandle == IntPtr.Zero) return null;

            string? result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
            this.tesseractApiSignatures.DeleteText(txtHandle);
            return htmlBeginTag + result + htmlEndTag;
        }

        //Just Copied:
        public string? BaseAPIGetHOCRText2(HandleRef handle, int pageNum)
        {
            IntPtr txtHandle = this.tesseractApiSignatures.BaseApiGetHOCRTextInternal(handle, pageNum);
            if (txtHandle == IntPtr.Zero) return null;

            string? result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
            this.tesseractApiSignatures.DeleteText(txtHandle);
            return xhtmlBeginTag + result + xhtmlEndTag;
        }

        public string? BaseAPIGetAltoText(HandleRef handle, int pageNum)
        {
            IntPtr txtHandle = this.tesseractApiSignatures.BaseApiGetAltoTextInternal(handle, pageNum);
            if (txtHandle == IntPtr.Zero) return null;
            
            string? result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
            this.tesseractApiSignatures.DeleteText(txtHandle);
            return result;
        }

        public string? BaseAPIGetTsvText(HandleRef handle, int pageNum)
        {
            IntPtr txtHandle = this.tesseractApiSignatures.BaseApiGetTsvTextInternal(handle, pageNum);
            if (txtHandle == IntPtr.Zero) return null;
            
            string? result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
            this.tesseractApiSignatures.DeleteText(txtHandle);
            return result;
        }

        public string? BaseAPIGetBoxText(HandleRef handle, int pageNum)
        {
            IntPtr txtHandle = this.tesseractApiSignatures.BaseApiGetBoxTextInternal(handle, pageNum);
            if (txtHandle == IntPtr.Zero) return null;
            
            string? result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
            this.tesseractApiSignatures.DeleteText(txtHandle);
            return result;
        }

        public string? BaseAPIGetLSTMBoxText(HandleRef handle, int pageNum)
        {
            IntPtr txtHandle = this.tesseractApiSignatures.BaseApiGetLSTMBoxTextInternal(handle, pageNum);
            if (txtHandle == IntPtr.Zero) return null;
            
            string? result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
            this.tesseractApiSignatures.DeleteText(txtHandle);
            return result;
        }

        public string? BaseAPIGetWordStrBoxText(HandleRef handle, int pageNum)
        {
            IntPtr txtHandle = this.tesseractApiSignatures.BaseApiGetWordStrBoxTextInternal(handle, pageNum);
            if (txtHandle == IntPtr.Zero) return null;
            
            string? result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
            this.tesseractApiSignatures.DeleteText(txtHandle);
            return result;
        }

        public string? BaseAPIGetUNLVText(HandleRef handle)
        {
            IntPtr txtHandle = this.tesseractApiSignatures.BaseApiGetUNLVTextInternal(handle);
            if (txtHandle == IntPtr.Zero) return null;
            
            string? result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
            this.tesseractApiSignatures.DeleteText(txtHandle);
            return result;
        }

        public string? BaseApiGetStringVariable(HandleRef handle, string name)
        {
            IntPtr resultHandle = this.tesseractApiSignatures.BaseApiGetStringVariableInternal(handle, name);
            if (resultHandle != IntPtr.Zero)
                return MarshalHelper.PtrToString(resultHandle, Encoding.UTF8);
            return null;
        }

        public string? BaseAPIGetUTF8Text(HandleRef handle)
        {
            IntPtr txtHandle = this.tesseractApiSignatures.BaseAPIGetUTF8TextInternal(handle);
            if (txtHandle == IntPtr.Zero) return null;
            
            string? result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
            this.tesseractApiSignatures.DeleteText(txtHandle);
            return result;
        }

        public int? BaseApiInit(HandleRef handle, string datapath, string language, int mode, IEnumerable<string> configFiles, IDictionary<string, object> initialValues, bool setOnlyNonDebugParams)
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

                object? pairValue = pair.Value;
                if (pairValue == null) throw new ArgumentException($"Variable '{pair.Key}': The type '{pairValue?.GetType()}' is not supported.");
                varNames[i] = pair.Key;
                if (TessConvert.TryToString(pairValue, out string? varValue))
                    varValues[i] = varValue!;
                else
                    throw new ArgumentException($"Variable '{pair.Key}': The type '{pairValue.GetType()}' is not supported.", nameof(initialValues));
                i++;
            }

            var varsVecSize = new UIntPtr((uint)varNames.Length);
            return this.tesseractApiSignatures.BaseApiInit(handle, datapath, language, mode,
                configFilesArray, configFilesArray.Length,
                varNames, varValues,
                varsVecSize, setOnlyNonDebugParams);
        }

        public int? BaseApiSetDebugVariable(HandleRef handle, string name, string value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = MarshalHelper.StringToPtr(value, Encoding.UTF8);
                return this.tesseractApiSignatures.BaseApiSetDebugVariable(handle, name, valuePtr);
            }
            finally
            {
                if (valuePtr != IntPtr.Zero) Marshal.FreeHGlobal(valuePtr);
            }
        }

        public int? BaseApiSetVariable(HandleRef handle, string name, string value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = MarshalHelper.StringToPtr(value, Encoding.UTF8);
                return this.tesseractApiSignatures.BaseApiSetVariable(handle, name, valuePtr);
            }
            finally
            {
                if (valuePtr != IntPtr.Zero) Marshal.FreeHGlobal(valuePtr);
            }
        }

        public string? ResultIteratorWordRecognitionLanguage(HandleRef handle)
        {
            // per docs (ltrresultiterator.h:118 as of 4897796 in github:tesseract-ocr/tesseract)
            // this return value should *NOT* be deleted.
            IntPtr txtHandle = this.tesseractApiSignatures.ResultIteratorWordRecognitionLanguageInternal(handle);
            return txtHandle != IntPtr.Zero
                ? MarshalHelper.PtrToString(txtHandle, Encoding.UTF8)
                : null;
        }

        public string? ResultIteratorGetUTF8Text(HandleRef handle, PageIteratorLevel level)
        {
            IntPtr txtHandle = this.tesseractApiSignatures.ResultIteratorGetUTF8TextInternal(handle, level);
            if (txtHandle == IntPtr.Zero) return null;

            string? result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
            this.tesseractApiSignatures.DeleteText(txtHandle);
            return result;
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
        public string? ChoiceIteratorGetUTF8Text(HandleRef choiceIteratorHandle)
        {
            if (choiceIteratorHandle.Handle == IntPtr.Zero) throw new ArgumentException("ChoiceIterator Handle cannot be a null IntPtr and is required");
            IntPtr txtChoiceHandle = this.tesseractApiSignatures.ChoiceIteratorGetUTF8TextInternal(choiceIteratorHandle);
            return MarshalHelper.PtrToString(txtChoiceHandle, Encoding.UTF8);
        }

        // hOCR Extension
    }
}