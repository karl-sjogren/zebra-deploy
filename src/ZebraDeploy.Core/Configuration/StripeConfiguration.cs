using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using ZebraDeploy.Core.Configuration.Reporters;
using ZebraDeploy.Core.Extensions;

namespace ZebraDeploy.Core.Configuration {
    public class StripeConfiguration {
        public string File { get; private set; }

        private readonly List<StripeStepConfiguration> _steps;
        private readonly List<StripeReporterConfiguration> _reporters;

        public IReadOnlyCollection<StripeStepConfiguration> Steps => new ReadOnlyCollection<StripeStepConfiguration>(_steps);
        public IReadOnlyCollection<StripeReporterConfiguration> Reporters => new ReadOnlyCollection<StripeReporterConfiguration>(_reporters);

        internal StripeConfiguration(XElement element) {
            File = element.Attribute("file").ValueOrDefault();
            _steps = element.Elements().Select(StripeStepConfiguration.FromXElement).Where(x => x != null).ToList();
            _reporters = element.Element("reporters")?.Elements().Select(StripeReporterConfiguration.FromXElement).Where(x => x != null).ToList() ?? new List<StripeReporterConfiguration>();
        }
    }
}