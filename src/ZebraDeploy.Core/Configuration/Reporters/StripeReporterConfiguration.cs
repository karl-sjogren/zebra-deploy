using System.Xml.Linq;
using ZebraDeploy.Core.Extensions;

namespace ZebraDeploy.Core.Configuration.Reporters {
    public abstract class StripeReporterConfiguration {
        public bool ReportSuccess { get; set; }
        public bool ReportFailure { get; set; }

        protected StripeReporterConfiguration(XElement element) {
            var success = element.Attribute("success").ValueOrDefault();
            var failure = element.Attribute("failure").ValueOrDefault();

            bool tmp;

            bool.TryParse(success, out tmp);
            ReportSuccess = tmp;

            bool.TryParse(failure, out tmp);
            ReportFailure = tmp;

        }

        internal static StripeReporterConfiguration FromXElement(XElement element) {
            if(element.Name == "hipchat")
                return new HipChatReporterConfiguration(element);

            if(element.Name == "pushover")
                return new PushoverReporterConfiguration(element);

            if(element.Name == "slack")
                return new SlackReporterConfiguration(element);

            return null;
        }
    }
}
