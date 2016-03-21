using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ZebraDeploy.Core.Configuration;
using ZebraDeploy.Core.StripeSteps;

namespace ZebraDeploy.Core {
    public class Stripe {
        private readonly StripeConfiguration _configuration;
        private readonly List<StripeStep> _steps;

        internal Stripe(StripeConfiguration configuration) {
            _configuration = configuration;

            _steps = _configuration.Steps.Select(StripeStep.CreateStep).ToList();
        }

        internal string File => _configuration.File;
        internal IReadOnlyCollection<StripeStep> Steps => new ReadOnlyCollection<StripeStep>(_steps);
        internal string CurrentStep { get; set; }
        internal double Progress { get; set; }
        internal DateTime LastDeploy { get; set; }
        internal bool Failed { get; set; }
    }
}