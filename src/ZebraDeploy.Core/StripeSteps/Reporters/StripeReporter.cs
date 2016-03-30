﻿using ZebraDeploy.Core.Configuration.Reporters;

namespace ZebraDeploy.Core.StripeSteps.Reporters {
    public abstract class StripeReporter {
        private readonly StripeReporterConfiguration _configuration;

        public bool ReportSuccess => _configuration.ReportSuccess;
        public bool ReportFailure => _configuration.ReportFailure;

        public abstract void Invoke(Stripe stripe);

        protected StripeReporter(StripeReporterConfiguration configuration) {
            this._configuration = configuration;
        }

        public static StripeReporter CreateStep(StripeReporterConfiguration configuration) {
            if(configuration.GetType() == typeof(HipChatReporterConfiguration))
                return new HipChatReporter(configuration as HipChatReporterConfiguration);

            return null;
        }
    }
}
