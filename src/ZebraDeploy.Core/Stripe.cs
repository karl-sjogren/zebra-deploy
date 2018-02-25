using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using ZebraDeploy.Core.Configuration;
using ZebraDeploy.Core.StripeSteps;
using ZebraDeploy.Core.StripeSteps.Reporters;

namespace ZebraDeploy.Core {
    public class Stripe {
        private readonly StripeConfiguration _configuration;
        private readonly List<StripeStep> _steps;
        private readonly List<StripeReporter> _reporters;
        private readonly Regex _fileRegex;

        internal Stripe(StripeConfiguration configuration) {
            _configuration = configuration;

            _steps = _configuration.Steps.Select(StripeStep.CreateStep).Where(x => x != null).ToList();
            _reporters = _configuration.Reporters.Select(StripeReporter.CreateStep).Where(x => x != null).ToList();

            var pattern = File;

            if(!pattern.StartsWith("^"))
                pattern = "^" + pattern;

            if(!pattern.EndsWith("$"))
                pattern = pattern + "$";

            _fileRegex = new Regex(pattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        internal string File => _configuration.File;
        internal IReadOnlyCollection<StripeStep> Steps => new ReadOnlyCollection<StripeStep>(_steps);
        internal IReadOnlyCollection<StripeReporter> Reporters => new ReadOnlyCollection<StripeReporter>(_reporters);
        internal string CurrentStep { get; set; }
        internal double Progress { get; set; }
        internal DateTime LastDeploy { get; set; }
        internal bool Failed { get; set; }

        internal Dictionary<string, string> ExecuteFor(string file) {
            var match = _fileRegex.Match(file);
            if(!match.Success)
                return null;

            var groups = _fileRegex.GetGroupNames();
            var result = new Dictionary<string, string>();

            foreach(var groupName in groups) {
                var group = match.Groups[groupName];
                if(group != null)
                    result.Add(groupName, group.Value);
            }

            return result;
        }
    }
}