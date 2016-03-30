using System.Xml.Linq;
using ZebraDeploy.Core.Extensions;

namespace ZebraDeploy.Core.Configuration.Reporters {
    public class HipChatReporterConfiguration : StripeReporterConfiguration {
        public string Room { get; private set; }

        public HipChatReporterConfiguration(XElement element) : base(element) {
            Room = element.Attribute("room").ValueOrDefault();
        }
    }
}