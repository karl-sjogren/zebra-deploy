using System.Xml.Linq;
using ZebraDeploy.Core.Extensions;

namespace ZebraDeploy.Core.Configuration {
    public class AppPoolStepConfiguration : StripeStepConfiguration {
        public string Name { get; internal set; }
        public string Action { get; internal set; }
        
        internal AppPoolStepConfiguration(XElement element) {
            Name = element.Attribute("name").ValueOrDefault();
            Action = element.Name == "startAppPool" ? "start" : "stop";
        }
    }
}
