using ZebraDeploy.Core.Configuration;

namespace ZebraDeploy.Core.StripeSteps {
    public abstract class StripeStep {
        public abstract void Invoke(Stripe stripe, string zipPath);

        public static StripeStep CreateStep(StripeStepConfiguration configuration) {
            if(configuration.GetType() == typeof(CleanStepConfiguration))
                return new CleanStep(configuration as CleanStepConfiguration);

            if(configuration.GetType() == typeof(AppPoolStepConfiguration))
                return new AppPoolStep(configuration as AppPoolStepConfiguration);

            if(configuration.GetType() == typeof(OutputStepConfiguration))
                return new OutputStep(configuration as OutputStepConfiguration);

            return null;
        }
    }
}
