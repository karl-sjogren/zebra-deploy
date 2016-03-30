using System.Xml.Linq;

namespace ZebraDeploy.Core.Configuration {
    public abstract class StripeStepConfiguration {
        internal static StripeStepConfiguration FromXElement(XElement element) {
            if(element.Name == "clean")
                return new CleanStepConfiguration(element);

            if(element.Name == "startAppPool" || element.Name == "stopAppPool")
                return new AppPoolStepConfiguration(element);

            if(element.Name == "startWebsite" || element.Name == "stopWebsite")
                return new WebsiteStepConfiguration(element);

            if(element.Name == "startService" || element.Name == "stopService")
                return new ServiceStepConfiguration(element);

            if(element.Name == "output")
                return new OutputStepConfiguration(element);

            return null;
        }
    }
}