using System.Xml.Linq;
using ZebraDeploy.Core.Extensions;

namespace ZebraDeploy.Core.Configuration {
    public class WebsiteStepConfiguration : StripeStepConfiguration {
        public string Name { get; private set; }
        public string Action { get; private set; }
        
        internal WebsiteStepConfiguration(XElement element) {
            Name = element.Attribute("name").ValueOrDefault();
            Action = element.Name == "startWebsite" ? "start" : "stop";
        }
    }
}
