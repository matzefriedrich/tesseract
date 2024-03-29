namespace TesseractExamples;

using Stream fs = new FileStream(filename, FileMode.Open, file_access);
using var ms = new MemoryStream();
fs.CopyTo(ms);
fs.Flush();
bytes[] fileBytes = ms.ToArray();

using var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
using var img = Pix.LoadFromMemory(fileBytes);
using var page = engine.Process(img);
var txt = page.GetText();
