using System.Xml.Linq;
using ZebraDeploy.Core.Extensions;

namespace ZebraDeploy.Core.Configuration {
    public class OutputStepConfiguration : StripeStepConfiguration {
        public string Path { get; internal set; }

        internal OutputStepConfiguration(XElement element) {
            Path = element.Attribute("path").ValueOrDefault();
        }
    }
}