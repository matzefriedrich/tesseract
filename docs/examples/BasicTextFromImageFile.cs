namespace TesseractExamples;

using var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
using var img = Pix.LoadFromFile(testImagePath);
using var page = engine.Process(img);

var text = page.GetText();

Console.WriteLine("Mean confidence: {0}", page.GetMeanConfidence());
Console.WriteLine("Text (GetText): \r\n{0}", text);
Console.WriteLine("Text (iterator):");
