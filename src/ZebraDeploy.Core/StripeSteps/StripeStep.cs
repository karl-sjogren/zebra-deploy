using System.Collections.Generic;
using ZebraDeploy.Core.Configuration;

namespace ZebraDeploy.Core.StripeSteps {
    public abstract class StripeStep {
        protected string StripeDescription { get; set; } = "Unknown stripe (or waiting for first run)";

        public abstract void Invoke(Stripe stripe, Dictionary<string, string> matchValues, string zipPath);

        public static StripeStep CreateStep(StripeStepConfiguration configuration) {
            if(configuration.GetType() == typeof(CleanStepConfiguration))
                return new CleanStep(configuration as CleanStepConfiguration);

            if(configuration.GetType() == typeof(AppPoolStepConfiguration))
                return new AppPoolStep(configuration as AppPoolStepConfiguration);

            if(configuration.GetType() == typeof(WebsiteStepConfiguration))
                return new WebsiteStep(configuration as WebsiteStepConfiguration);

            if(configuration.GetType() == typeof(ServiceStepConfiguration))
                return new ServiceStep(configuration as ServiceStepConfiguration);

            if(configuration.GetType() == typeof(OutputStepConfiguration))
                return new OutputStep(configuration as OutputStepConfiguration);

            return null;
        }

        public override string ToString() {
            return StripeDescription;
        }
    }
}
