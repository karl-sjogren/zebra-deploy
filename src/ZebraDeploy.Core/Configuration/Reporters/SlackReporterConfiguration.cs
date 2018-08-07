using System.Xml.Linq;
using ZebraDeploy.Core.Extensions;

namespace ZebraDeploy.Core.Configuration.Reporters {
    public class SlackReporterConfiguration : StripeReporterConfiguration {
        public string HookUrl { get; private set; }

        public SlackReporterConfiguration(XElement element) : base(element) {
            HookUrl = element.Attribute("hookUrl").ValueOrDefault();
        }
    }
}