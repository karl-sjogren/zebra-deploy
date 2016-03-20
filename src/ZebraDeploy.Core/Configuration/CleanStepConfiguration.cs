using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using ZebraDeploy.Core.Extensions;

namespace ZebraDeploy.Core.Configuration {
    public class CleanStepConfiguration : StripeStepConfiguration {
        public string Path { get; internal set; }

        private readonly List<string> _excludes;
        public IReadOnlyCollection<string> Excludes => new ReadOnlyCollection<string>(_excludes);

        internal CleanStepConfiguration(XElement element) {
            Path = element.Attribute("path").ValueOrDefault();
            _excludes = element.Descendants("exclude").Select(el => el.Attribute("name").ValueOrDefault()?.ToLowerInvariant()).Where(ex => ex != null).ToList();
        }
    }
}