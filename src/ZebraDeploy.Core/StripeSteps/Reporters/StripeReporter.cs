using ZebraDeploy.Core.Configuration.Reporters;

namespace ZebraDeploy.Core.StripeSteps.Reporters {
    public abstract class StripeReporter {
        private readonly StripeReporterConfiguration _configuration;

        public bool ReportSuccess => _configuration.ReportSuccess;
        public bool ReportFailure => _configuration.ReportFailure;

        public abstract void Invoke(Stripe stripe, string zipName);

        protected StripeReporter(StripeReporterConfiguration configuration) {
            this._configuration = configuration;
        }

        public static StripeReporter CreateStep(StripeReporterConfiguration configuration) {
            if(configuration.GetType() == typeof(HipChatReporterConfiguration))
                return new HipChatReporter(configuration as HipChatReporterConfiguration);

            if(configuration.GetType() == typeof(PushoverReporterConfiguration))
                return new PushoverReporter(configuration as PushoverReporterConfiguration);

            if(configuration.GetType() == typeof(SlackReporterConfiguration))
                return new SlackReporter(configuration as SlackReporterConfiguration);

            return null;
        }
    }
}
