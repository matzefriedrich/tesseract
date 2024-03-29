namespace TesseractExamples;

using IResultRenderer renderer = Tesseract.PdfResultRenderer.CreatePdfRenderer(@"test.pdf", @"./tessdata", false);
using renderer.BeginDocument("Serachablepdftest")

const string configurationFilePath = @"C:\tessdata";
using TesseractEngine engine = new (configurationFilePath, "eng", EngineMode.TesseractAndLstm);
using var img = Pix.LoadFromFile(@"C:\file-page1.jpg");
using var page = engine.Process(img, "Serachablepdftest");
renderer.AddPage(page);