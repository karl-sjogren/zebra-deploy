using System.Linq;
using System.Xml.Linq;
using ZebraDeploy.Core.Extensions;

namespace ZebraDeploy.Core.Configuration.Reporters {
    public class PushoverReporterConfiguration : StripeReporterConfiguration {
        public string ApplicationKey { get; private set; }
        public string[] UserKeys { get; private set; }

        public string SuccessPriority { get; private set; }
        public string FailurePriority { get; private set; }

        public PushoverReporterConfiguration(XElement element) : base(element) {
            ApplicationKey = element.Attribute("application-key").ValueOrDefault();

            SuccessPriority = element.Attribute("success-priority").ValueOrDefault() ?? "-1";
            FailurePriority = element.Attribute("failure-priority").ValueOrDefault() ?? "0";

            var userNodes = element.Descendants("user");
            UserKeys = userNodes.Select(n => n.Attribute("key").ValueOrDefault()).ToArray();
        }
    }
}