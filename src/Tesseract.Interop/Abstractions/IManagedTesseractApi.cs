namespace Tesseract.Interop.Abstractions
{
    using System.Runtime.InteropServices;

    public interface IManagedTesseractApi
    {
        int? Init(HandleRef handle, string dataPath, string language, int mode, IEnumerable<string> configFiles, IDictionary<string, object> initialValues, bool setOnlyNonDebugParams);
        int? SetDebugVariable(HandleRef handle, string name, string value);
        int? SetVariable(HandleRef handle, string name, string value);
        string? GetAltoText(HandleRef handle, int pageNum);
        string? GetBoxText(HandleRef handle, int pageNum);
        
        string GetHOCRText(HandleRef handle, int pageNum, HocrTextFormat format);
        
        string? GetLSTMBoxText(HandleRef handle, int pageNum);
        string? GetTsvText(HandleRef handle, int pageNum);
        string? GetUNLVText(HandleRef handle);
        string? GetUTF8Text(HandleRef handle);
        string? GetWordStrBoxText(HandleRef handle, int pageNum);
        string? GetStringVariable(HandleRef handle, string name);
        string? GetVersion();
        string? ChoiceIteratorGetUTF8Text(HandleRef choiceIteratorHandle);
        string? ResultIteratorGetUTF8Text(HandleRef handle, PageIteratorLevel level);
        string? ResultIteratorWordRecognitionLanguage(HandleRef handle);
    }
}