using Ionic.Zip;
using Serilog;
using ZebraDeploy.Core.Configuration;

namespace ZebraDeploy.Core.StripeSteps {
    public class OutputStep : StripeStep {
        private readonly ILogger _log = Log.ForContext<OutputStep>();
        private readonly OutputStepConfiguration _configuration;

        public OutputStep(OutputStepConfiguration configuration) {
            _configuration = configuration;
        }

        public override void Invoke(Stripe stripe, string zipPath) {
            _log.Debug("Extracting {zipFile}, to {path}.", zipPath, _configuration.Path);

            using(var zip = ZipFile.Read(zipPath)) {
                zip.ExtractAll(_configuration.Path, ExtractExistingFileAction.OverwriteSilently);
            }
        }
    }
}