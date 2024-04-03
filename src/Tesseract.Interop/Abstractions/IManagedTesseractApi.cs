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

        string? GetHocrText(HandleRef handle, int pageNum, HocrTextFormat format);

        string? GetLstmBoxText(HandleRef handle, int pageNum);

        string? GetTsvText(HandleRef handle, int pageNum);

        string? GetUnlvText(HandleRef handle);

        string? GetUtf8Text(HandleRef handle);

        string? GetWordStrBoxText(HandleRef handle, int pageNum);

        string? GetStringVariable(HandleRef handle, string name);

        string? GetVersion();

        string? ChoiceIteratorGetUtf8Text(HandleRef choiceIteratorHandle);

        string? ResultIteratorGetUtf8Text(HandleRef handle, PageIteratorLevel level);

        string? ResultIteratorWordRecognitionLanguage(HandleRef handle);
    }
}