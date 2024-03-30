namespace Tesseract.Interop.Abstractions
{
    using System.Runtime.InteropServices;

    public interface IManagedTesseractApi
    {
        int? BaseApiInit(HandleRef handle, string datapath, string language, int mode, IEnumerable<string> configFiles, IDictionary<string, object> initialValues, bool setOnlyNonDebugParams);
        int? BaseApiSetDebugVariable(HandleRef handle, string name, string value);
        int? BaseApiSetVariable(HandleRef handle, string name, string value);
        string? BaseAPIGetAltoText(HandleRef handle, int pageNum);
        string? BaseAPIGetBoxText(HandleRef handle, int pageNum);
        string? BaseAPIGetHOCRText(HandleRef handle, int pageNum);
        string? BaseAPIGetHOCRText2(HandleRef handle, int pageNum);
        string? BaseAPIGetLSTMBoxText(HandleRef handle, int pageNum);
        string? BaseAPIGetTsvText(HandleRef handle, int pageNum);
        string? BaseAPIGetUNLVText(HandleRef handle);
        string? BaseAPIGetUTF8Text(HandleRef handle);
        string? BaseAPIGetWordStrBoxText(HandleRef handle, int pageNum);
        string? BaseApiGetStringVariable(HandleRef handle, string name);
        string? BaseApiGetVersion();
        string? ChoiceIteratorGetUTF8Text(HandleRef choiceIteratorHandle);
        string? ResultIteratorGetUTF8Text(HandleRef handle, PageIteratorLevel level);
        string? ResultIteratorWordRecognitionLanguage(HandleRef handle);
    }
}