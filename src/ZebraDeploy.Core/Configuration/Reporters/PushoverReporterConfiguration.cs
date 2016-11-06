using System.Linq;
using System.Xml.Linq;
using ZebraDeploy.Core.Extensions;

namespace ZebraDeploy.Core.Configuration.Reporters {
    public class PushoverReporterConfiguration : StripeReporterConfiguration {
        public string ApplicationKey { get; private set; }
        public string[] UserKeys { get; private set; }

        public PushoverReporterConfiguration(XElement element) : base(element) {
            ApplicationKey = element.Attribute("application-key").ValueOrDefault();

            var userNodes = element.Descendants("user");
            UserKeys = userNodes.Select(n => n.Attribute("key").ValueOrDefault()).ToArray();
        }
    }
}