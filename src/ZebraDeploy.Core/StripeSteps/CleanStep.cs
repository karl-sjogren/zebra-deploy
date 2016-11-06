using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Serilog;
using ZebraDeploy.Core.Configuration;
using ZebraDeploy.Core.Extensions;

namespace ZebraDeploy.Core.StripeSteps {
    public class CleanStep : StripeStep {
        private readonly ILogger _log = Log.ForContext<CleanStep>();
        private readonly CleanStepConfiguration _configuration;

        public CleanStep(CleanStepConfiguration configuration) {
            _configuration = configuration;
        }
        
        public override void Invoke(Stripe stripe, Dictionary<string, string> matchValues, string zipPath) {
            var path = _configuration.Path.ReplaceMatchedValues(matchValues);
            var excludes = _configuration.Excludes.Select(e => e.ReplaceMatchedValues(matchValues)).ToList();

            StripeDescription = "Clean path " + path;

            _log.Debug("Cleaning path {path}, excluding {excludes}.", path, string.Join(", ", excludes));

            var startTime = DateTime.Now;
            while(true) {
                var failed = false;
                try {
                    Clean(path, excludes);
                } catch(Exception) {
                    if(startTime.AddSeconds(30) > DateTime.Now)
                        break;

                    Thread.Sleep(500);
                    failed = true;
                }

                if(!failed)
                    break;
            }
        }

        private static void Clean(string path, IList<string> excludes) {
            foreach(var directory in Directory.EnumerateDirectories(path)) {
                var info = new DirectoryInfo(directory);

                if(excludes.Contains(info.Name.ToLowerInvariant()))
                    continue;

                Directory.Delete(info.FullName, true);
            }

            foreach(var file in Directory.EnumerateFiles(path)) {
                var info = new FileInfo(file);

                if(excludes.Contains(info.Name.ToLowerInvariant()))
                    continue;

                File.Delete(info.FullName);
            }
        }
    }
}