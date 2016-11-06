using System;
using System.Net;
using Newtonsoft.Json;
using Serilog;
using ZebraDeploy.Core.Configuration.Reporters;

namespace ZebraDeploy.Core.StripeSteps.Reporters {
    public class HipChatReporter : StripeReporter {
        private readonly ILogger _log = Log.ForContext<HipChatReporter>();
        private readonly HipChatReporterConfiguration _configuration;

        public HipChatReporter(HipChatReporterConfiguration configuration) : base(configuration) {
            _configuration = configuration;
        }

        public override void Invoke(Stripe stripe) {
            var obj = new {
                color = stripe.Failed ? "red" : "green",
                message = stripe.Failed ? $"Failed to deploy stripe {stripe.File} at {Environment.MachineName}. It failed at step \"{stripe.CurrentStep}\"." : $"Successfully deployed stripe {stripe.File} at {Environment.MachineName}.",
                notify = stripe.Failed,
                message_format = "text"
            };

            using(var wc = new WebClient()) {
                wc.Headers.Add("Content-Type", "application/json");

                try {
                    wc.UploadString(_configuration.Room, JsonConvert.SerializeObject(obj));
                } catch(Exception e) {
                    _log.Error(e, "Failed to report {file} to HipChat.", stripe.File);
                }
            }
        }
    }
}