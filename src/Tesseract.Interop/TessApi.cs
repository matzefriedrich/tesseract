namespace Tesseract.Interop
{
    using System.Runtime.InteropServices;
    using System.Text;
    using Abstractions;
    using Internal;
    using Resources;

    internal sealed class TessApi : IManagedTesseractApi
    {
        private readonly ITessApiSignatures tesseractApiSignatures;

        public TessApi(
            ITessApiSignatures tesseractApiSignature)
        {
            this.tesseractApiSignatures = tesseractApiSignature ?? throw new ArgumentNullException(nameof(tesseractApiSignature));
        }

        public string? GetVersion()
        {
            IntPtr versionHandle = this.tesseractApiSignatures.GetVersion();
            return versionHandle != IntPtr.Zero ? MarshalHelper.PtrToString(versionHandle, Encoding.UTF8) : null;
        }

        public string? GetHocrText(HandleRef handle, int pageNum, HocrTextFormat format)
        {
            string? result = this.InvokeGetText(this.tesseractApiSignatures.GetHOCRTextInternal, handle, pageNum);
            if (result == null) return null;

            switch (format)
            {
                case HocrTextFormat.Html:
                    return HocrTextBuilder.MakeHtmlDocument(result);
                case HocrTextFormat.XHtml:
                    return HocrTextBuilder.MakeXhtmlDocument(result);
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }

        public string? GetAltoText(HandleRef handle, int pageNum)
        {
            return this.InvokeGetText(this.tesseractApiSignatures.GetAltoTextInternal, handle, pageNum);
        }

        public string? GetTsvText(HandleRef handle, int pageNum)
        {
            return this.InvokeGetText(this.tesseractApiSignatures.GetTsvTextInternal, handle, pageNum);
        }

        public string? GetBoxText(HandleRef handle, int pageNum)
        {
            return this.InvokeGetText(this.tesseractApiSignatures.GetBoxTextInternal, handle, pageNum);
        }

        public string? GetLstmBoxText(HandleRef handle, int pageNum)
        {
            return this.InvokeGetText(this.tesseractApiSignatures.GetLSTMBoxTextInternal, handle, pageNum);
        }

        public string? GetWordStrBoxText(HandleRef handle, int pageNum)
        {
            return this.InvokeGetText(this.tesseractApiSignatures.GetWordStrBoxTextInternal, handle, pageNum);
        }

        public string? GetUnlvText(HandleRef handle)
        {
            IntPtr txtHandle = this.tesseractApiSignatures.GetUNLVTextInternal(handle);
            if (txtHandle == IntPtr.Zero) return null;

            string? result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
            this.tesseractApiSignatures.DeleteText(txtHandle);
            return result;
        }

        public string? GetStringVariable(HandleRef handle, string name)
        {
            IntPtr resultHandle = this.tesseractApiSignatures.GetStringVariableInternal(handle, name);
            if (resultHandle != IntPtr.Zero)
                return MarshalHelper.PtrToString(resultHandle, Encoding.UTF8);
            return null;
        }

        public string? GetUtf8Text(HandleRef handle)
        {
            IntPtr txtHandle = this.tesseractApiSignatures.GetUTF8TextInternal(handle);
            if (txtHandle == IntPtr.Zero) return null;

            string? result = MarshalHelper.PtrToString(txtHandle, Encoding.UTF8);
            this.tesseractApiSignatures.DeleteText(txtHandle);
            return result;
        }

        public int? Initialize(HandleRef handle, string dataPath, string language, int mode, IEnumerable<string> configFiles, IDictionary<string, object> initialValues, bool setOnlyNonDebugParams)
        {
            if (handle.Handle == IntPtr.Zero) throw new ArgumentException($"The given handle is invalid. Use {nameof(this.tesseractApiSignatures.Create)} to get one.");
            if (string.IsNullOrWhiteSpace(language)) throw new ArgumentException(Resources.Value_cannot_be_null_or_whitespace, nameof(language));
            ArgumentNullException.ThrowIfNull(configFiles);
            ArgumentNullException.ThrowIfNull(initialValues);

            string[] configFilesArray = new List<string>(configFiles).ToArray();

            var varNames = new string[initialValues.Count];
            var varValues = new string[initialValues.Count];
            var i = 0;
            foreach ((string? key, object? value) in initialValues)
            {
                if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Variable must have a name.");

                if (value == null) throw new ArgumentException($"Variable '{key}': The type '{value?.GetType()}' is not supported.");
                varNames[i] = key;
                if (value.TryFormatAsString(out string? varValue))
                    varValues[i] = varValue!;
                else
                    throw new ArgumentException($"Variable '{key}': The type '{value.GetType()}' is not supported.", nameof(initialValues));
                i++;
            }

            var varsVecSize = new UIntPtr((uint)varNames.Length);
            return this.tesseractApiSignatures.Init4(handle, dataPath, language, mode,
                configFilesArray, configFilesArray.Length,
                varNames, varValues,
                varsVecSize, setOnlyNonDebugParams);
        }

        public int? SetDebugVariable(HandleRef handle, string name, string value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = MarshalHelper.StringToPtr(value, Encoding.UTF8);
                return this.tesseractApiSignatures.SetDebugVariable(handle, name, valuePtr);
            }
            finally
            {
                if (valuePtr != IntPtr.Zero) Marshal.FreeHGlobal(valuePtr);
            }
        }

        public int? SetVariable(HandleRef handle, string name, string value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = MarshalHelper.StringToPtr(value, Encoding.UTF8);
                return this.tesseractApiSignatures.SetVariable(handle, name, valuePtr);
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

        public string? ResultIteratorGetUtf8Text(HandleRef handle, PageIteratorLevel level)
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
        public string? ChoiceIteratorGetUtf8Text(HandleRef choiceIteratorHandle)
        {
            if (choiceIteratorHandle.Handle == IntPtr.Zero) throw new ArgumentException("Invalid iterator handle.");
            IntPtr txtChoiceHandle = this.tesseractApiSignatures.ChoiceIteratorGetUTF8TextInternal(choiceIteratorHandle);
            return MarshalHelper.PtrToString(txtChoiceHandle, Encoding.UTF8);
        }

        private string? InvokeGetText(GetTextFunc func, HandleRef handle, int pageNum, Encoding? encoding = null)
        {
            IntPtr txtHandle = func(handle, pageNum);
            if (txtHandle == IntPtr.Zero) return null;

            string? result = MarshalHelper.PtrToString(txtHandle, encoding ?? Encoding.UTF8);
            this.tesseractApiSignatures.DeleteText(txtHandle);
            return result;
        }

        private delegate IntPtr GetTextFunc(HandleRef handle, int pageNumber);
    }
}