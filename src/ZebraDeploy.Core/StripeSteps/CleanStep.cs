using System.IO;
using System.Linq;
using Serilog;
using ZebraDeploy.Core.Configuration;

namespace ZebraDeploy.Core.StripeSteps {
    public class CleanStep : StripeStep {
        private readonly ILogger _log = Log.ForContext<CleanStep>();
        private readonly CleanStepConfiguration _configuration;

        public CleanStep(CleanStepConfiguration configuration) {
            _configuration = configuration;
        }

        public override void Invoke(Stripe stripe, string zipPath) {
            _log.Debug("Cleaning path {path}, excluding {excludes}.", _configuration.Path, string.Join(", ", _configuration.Excludes));

            foreach (var directory in Directory.EnumerateDirectories(_configuration.Path)) {
                var info = new DirectoryInfo(directory);

                if (_configuration.Excludes.Contains(info.Name.ToLowerInvariant()))
                    continue;

                Directory.Delete(info.FullName, true);
            }

            foreach(var file in Directory.EnumerateFiles(_configuration.Path)) {
                var info = new FileInfo(file);

                if(_configuration.Excludes.Contains(info.Name.ToLowerInvariant()))
                    continue;

                File.Delete(info.FullName);
            }
        }
    }
}