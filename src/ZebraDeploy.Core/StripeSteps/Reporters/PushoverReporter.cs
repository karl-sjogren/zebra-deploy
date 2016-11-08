using System;
using System.Collections.Specialized;
using System.Net;
using Serilog;
using ZebraDeploy.Core.Configuration.Reporters;

namespace ZebraDeploy.Core.StripeSteps.Reporters {
    public class PushoverReporter : StripeReporter {
        private readonly ILogger _log = Log.ForContext<PushoverReporter>();
        private readonly PushoverReporterConfiguration _configuration;

        public PushoverReporter(PushoverReporterConfiguration configuration) : base(configuration) {
            _configuration = configuration;
        }

        public override void Invoke(Stripe stripe, string zipName) {

            foreach(var userKey in _configuration.UserKeys) {

                var values = new NameValueCollection();
                values.Add("token", _configuration.ApplicationKey);
                values.Add("user", userKey);
                values.Add("title", "Zebra Deploy");
                values.Add("message", stripe.Failed ? $"Failed to deploy stripe {zipName} at {Environment.MachineName}. It failed at step \"{stripe.CurrentStep}\"." : $"Successfully deployed stripe {zipName} at {Environment.MachineName}.");
                values.Add("priority", stripe.Failed ? "0" : "-1");

                using(var wc = new WebClient()) {
                    try {
                        wc.UploadValues("https://api.pushover.net/1/messages.json", values);
                    } catch(Exception e) {
                        _log.Error(e, "Failed to report {file} to Pusohver {user}.", zipName, userKey);
                    }
                }
            }
        }
    }
}