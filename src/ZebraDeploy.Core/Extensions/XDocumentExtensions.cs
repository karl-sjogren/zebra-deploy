using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ZebraDeploy.Core.Extensions {
    public static class XDocumentExtensions {
        public static string FirstValueOrDefault(this IEnumerable<XElement> enumerable, Func<XElement, bool> predicate) {
            var first = enumerable.FirstOrDefault(predicate);
            return first?.Value;
        }

        public static string FirstValueOrDefault(this IEnumerable<XElement> enumerable) {
            var first = enumerable.FirstOrDefault();
            return first?.Value;
        }

        public static string FirstValueOrDefault(this IEnumerable<XAttribute> enumerable, Func<XAttribute, bool> predicate) {
            var first = enumerable.FirstOrDefault(predicate);
            return first?.Value;
        }

        public static string FirstValueOrDefault(this IEnumerable<XAttribute> enumerable) {
            var first = enumerable.FirstOrDefault();
            return first?.Value;
        }

        public static string ValueOrDefault(this XAttribute attribute) {
            return attribute?.Value;
        }

        public static string ValueOrDefault(this XElement element) {
            return element?.Value;
        }
    }
}