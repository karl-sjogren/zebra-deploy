using System.Collections.Generic;
using Ionic.Zip;
using Serilog;
using ZebraDeploy.Core.Configuration;
using ZebraDeploy.Core.Extensions;

namespace ZebraDeploy.Core.StripeSteps {
    public class OutputStep : StripeStep {
        private readonly ILogger _log = Log.ForContext<OutputStep>();
        private readonly OutputStepConfiguration _configuration;

        public OutputStep(OutputStepConfiguration configuration) {
            _configuration = configuration;
        }
        
        public override void Invoke(Stripe stripe, Dictionary<string, string> matchValues, string zipPath) {
            var path = _configuration.Path.ReplaceMatchedValues(matchValues);
            _log.Debug("Extracting {zipFile}, to {path}.", zipPath, path);

            StripeDescription = "Extract content to " + path;

            using(var zip = ZipFile.Read(zipPath)) {
                zip.ExtractAll(path, ExtractExistingFileAction.OverwriteSilently);
            }
        }
    }
}