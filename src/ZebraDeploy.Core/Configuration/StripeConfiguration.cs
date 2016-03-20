using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using ZebraDeploy.Core.Extensions;

namespace ZebraDeploy.Core.Configuration {
    public class StripeConfiguration {
        public string File { get; internal set; }

        private readonly List<StripeStepConfiguration> _steps;
        public IReadOnlyCollection<StripeStepConfiguration> Steps => new ReadOnlyCollection<StripeStepConfiguration>(_steps);

        internal StripeConfiguration(XElement element) {
            File = element.Attribute("file").ValueOrDefault();
            _steps = element.Elements().Select(StripeStepConfiguration.FromXElement).ToList();
        }
    }
}