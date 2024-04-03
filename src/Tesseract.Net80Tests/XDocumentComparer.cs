namespace Tesseract.Tests
{
    using System.Xml.Linq;

    internal static class XDocumentComparer
    {
        public static bool AreEqual(XDocument doc1, XDocument doc2)
        {
            if (doc1 == null) throw new ArgumentNullException(nameof(doc1));
            if (doc2 == null) throw new ArgumentNullException(nameof(doc2));

            XElement? left = doc1.Root;
            XElement? right = doc2.Root;

            return AreElementsEqual(left, right) && AreDescendantsEqual(left, right);
        }

        private static bool AreDescendantsEqual(XContainer? left, XContainer? right)
        {
            List<XElement> childElems1 = left?.Elements().OrderBy(QualifiedElementName).ToList() ?? [];
            List<XElement> childElems2 = right?.Elements().OrderBy(QualifiedElementName).ToList() ?? [];

            if (childElems1.Count != childElems2.Count) return false;

            for (var i = 0; i < childElems1.Count; i++)
            {
                XElement childLeft = childElems1[i];
                XElement childRight = childElems2[i];
                if (!AreElementsEqual(childLeft, childRight)) return false;
                if (!AreDescendantsEqual(childLeft, childRight)) return false;
            }

            return true;

            // Compare child elements
            string QualifiedElementName(XElement element)
            {
                return $"{element.Name.Namespace}:{element.Name.LocalName}";
            }
        }

        private static bool AreElementsEqual(XElement? left, XElement? right)
        {
            if (left?.Name != right?.Name) return false;

            List<XAttribute> attributesLeft = left?.Attributes().OrderBy(QualifiedAttributeName).ToList() ?? [];
            List<XAttribute> attributesRight = right?.Attributes().OrderBy(QualifiedAttributeName).ToList() ?? [];
            if (!attributesLeft.SequenceEqual(attributesRight, new XAttributeEqualityComparer())) return false;

            string? valueLeft = left?.Value.Trim();
            string? valueRight = right?.Value.Trim();

            return string.Equals(valueLeft, valueRight, StringComparison.OrdinalIgnoreCase);

            string QualifiedAttributeName(XAttribute attribute)
            {
                return $"{attribute.Name.NamespaceName}:{attribute.Name.LocalName}";
            }
        }

        private class XAttributeEqualityComparer : IEqualityComparer<XAttribute>
        {
            public bool Equals(XAttribute? x, XAttribute? y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.BaseUri == y.BaseUri && x.IsNamespaceDeclaration == y.IsNamespaceDeclaration && x.Name.Equals(y.Name) && x.NodeType == y.NodeType && x.Value == y.Value;
            }

            public int GetHashCode(XAttribute obj)
            {
                return HashCode.Combine(obj.BaseUri, obj.IsNamespaceDeclaration, obj.Name, (int)obj.NodeType, obj.Value);
            }
        }
    }
}