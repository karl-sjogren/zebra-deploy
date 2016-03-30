using System.Xml.Linq;
using ZebraDeploy.Core.Extensions;

namespace ZebraDeploy.Core.Configuration {
    public class ServiceStepConfiguration : StripeStepConfiguration {
        public string Name { get; private set; }
        public string Action { get; private set; }
        
        internal ServiceStepConfiguration(XElement element) {
            Name = element.Attribute("name").ValueOrDefault();
            Action = element.Name == "startService" ? "start" : "stop";
        }
    }
}
