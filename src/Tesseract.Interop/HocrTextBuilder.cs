namespace Tesseract.Interop
{
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    internal static class HocrTextBuilder
    {
        public static string MakeXhtmlDocument(string rawBody)
        {
            XNamespace xmlns = "http://www.w3.org/1999/xhtml";

            var ocrCapabilities = new[] { "ocr_page", "ocr_carea", "ocr_par", "ocr_line", "ocrx_word" };

            var headElt = new XElement(xmlns + "head",
                new XElement(xmlns + "title"),
                new XElement(xmlns + "meta", new XAttribute("http-equiv", "Content-Type"), new XAttribute("content", "text/html;charset=utf-8")),
                new XElement(xmlns + "meta", new XAttribute("name", "ocr-system"), new XAttribute("content", "tesseract")),
                new XElement(xmlns + "meta", new XAttribute("name", "ocr-capabilities"), new XAttribute("content", string.Join(" ", ocrCapabilities))));

            XElement content = XElement.Parse(rawBody, LoadOptions.None);
            AdjustNamespace(content, xmlns);

            var bodyElt = new XElement(xmlns + "body", content);

            var doc = new XDocument(
                new XDocumentType("html", "-//W3C//DTD XHTML 1.0 Transitional//EN", "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd", null),
                new XElement(xmlns + "html", new XAttribute(XNamespace.Xml + "lang", "en"), new XAttribute("lang", "en"),
                    headElt,
                    bodyElt));

            const bool omitXmlDeclaration = false;
            return doc.GetXmlStringFrom(omitXmlDeclaration);

            void AdjustNamespace(XElement e, XNamespace ns)
            {
                var stack = new Stack<XElement>();
                stack.Push(e);
                while (stack.Any())
                {
                    XElement next = stack.Pop();
                    next.Name = ns + next.Name.LocalName;
                    foreach (XElement child in next.Elements()) stack.Push(child);
                }
            }
        }

        public static string MakeHtmlDocument(string? rawBody)
        {
            var headElt = new XElement("head",
                new XElement("title", ""),
                new XElement("meta", new XAttribute("http-equiv", "Content-Type"), new XAttribute("content", "text/html;charset=utf-8")),
                new XElement("meta", new XAttribute("name", "ocr-system"), new XAttribute("content", "tesseract")));

            object content = string.IsNullOrWhiteSpace(rawBody) ? "" : XElement.Parse(rawBody, LoadOptions.None);
            var bodyElt = new XElement("body", content);

            var doc = new XDocument(
                new XDocumentType("html", "-//W3C//DTD HTML 4.01 Transitional//EN", "http://www.w3.org/TR/html4/loose.dtd", null),
                new XElement("html",
                    headElt,
                    bodyElt));

            return doc.GetXmlStringFrom(true);
        }

        private static string GetXmlStringFrom(this XDocument doc, bool omitXmlDeclaration)
        {
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

            var writerSettings = new XmlWriterSettings
            {
                ConformanceLevel = ConformanceLevel.Document,
                Indent = true,
                NamespaceHandling = NamespaceHandling.OmitDuplicates,
                CloseOutput = false,
                OmitXmlDeclaration = omitXmlDeclaration
            };

            using var xmlWriter = XmlWriter.Create(writer, writerSettings);
            doc.Save(xmlWriter);
            xmlWriter.Flush();

            stream.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}