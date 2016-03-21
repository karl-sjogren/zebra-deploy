using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using ZebraDeploy.Core.Extensions;

namespace ZebraDeploy.Core.Configuration {
    public class ZebraConfiguration {
        public string BasePath { get; private set; }

        private readonly List<StripeConfiguration> _stripes;
        public IReadOnlyCollection<StripeConfiguration> Stripes => new ReadOnlyCollection<StripeConfiguration>(_stripes);

        internal ZebraConfiguration(XElement element) {
            BasePath = element.Element("basePath").ValueOrDefault();
            _stripes = element.Descendants("stripe").Select(el => new StripeConfiguration(el)).ToList();
        }

        #region Static load methods

        public static ZebraConfiguration Load(string xml) {
            var doc = XDocument.Parse(xml);

            return LoadFromXDocument(doc);
        }

        public static ZebraConfiguration LoadFromFile(string filePath) {
            var doc = XDocument.Load(filePath);

            return LoadFromXDocument(doc);
        }

        private static ZebraConfiguration LoadFromXDocument(XDocument doc) {
            return new ZebraConfiguration(doc.Root);
        }

        #endregion
    }
}
