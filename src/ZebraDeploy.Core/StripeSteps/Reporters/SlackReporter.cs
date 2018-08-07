using System;
using System.Net;
using Newtonsoft.Json;
using Serilog;
using ZebraDeploy.Core.Configuration.Reporters;

namespace ZebraDeploy.Core.StripeSteps.Reporters {
    public class SlackReporter : StripeReporter {
        private readonly ILogger _log = Log.ForContext<SlackReporter>();
        private readonly SlackReporterConfiguration _configuration;

        public SlackReporter(SlackReporterConfiguration configuration) : base(configuration) {
            _configuration = configuration;
        }

        public override void Invoke(Stripe stripe, string zipName) {
            var message = stripe.Failed
                ? $"Failed to deploy stripe {zipName}. It failed at step \"{stripe.CurrentStep}\"."
                : $"Successfully deployed stripe {zipName}.";
            var obj = new {
                attachments = new[] {
                    new {
                        author_name = Environment.MachineName,
                        fallback = message,
                        color = stripe.Failed ? "#d50200" : "#2fa44f",
                        text = message
                    }
                }
            };

            using(var wc = new WebClient()) {
                wc.Headers.Add("Content-Type", "application/json");

                try {
                    wc.UploadString(_configuration.HookUrl, JsonConvert.SerializeObject(obj));
                } catch(Exception e) {
                    _log.Error(e, "Failed to report {file} to Slack.", zipName);
                }
            }
        }
    }
}