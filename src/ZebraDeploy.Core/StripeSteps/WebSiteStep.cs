using System;
using System.Threading;
using Microsoft.Web.Administration;
using Serilog;
using ZebraDeploy.Core.Configuration;

namespace ZebraDeploy.Core.StripeSteps {
    public class WebsiteStep : StripeStep {
        private readonly ILogger _log = Log.ForContext<WebsiteStep>();
        private readonly WebsiteStepConfiguration _configuration;
        private static readonly object _lock = new object();

        public WebsiteStep(WebsiteStepConfiguration configuration) {
            _configuration = configuration;
        }

        public override string ToString() {
            if(_configuration.Action == "start")
                return "Start Website " + _configuration.Name;

            return "Stop Website " + _configuration.Name;
        }

        public override void Invoke(Stripe stripe, string zipPath) {
            // Server manager seems to be using some COM behind the scenes
            // that doesn't like it multithreaded..
            lock (_lock) {
                try {
                    var manager = new ServerManager();
                    var site = manager.Sites[_configuration.Name];
                    if(site == null)
                        throw new InvalidOperationException($"Failed to locate website named {_configuration.Name}.");


                    var timestamp = DateTime.Now;
                    if(_configuration.Action == "start") {
                        if(site.State == ObjectState.Started || site.State == ObjectState.Starting)
                            return;

                        _log.Debug("Starting application pool {poolName}", _configuration.Name);

                        var state = ObjectState.Unknown;
                        while(state != ObjectState.Started) {
                            try {
                                site.Start();
                            } catch(Exception) {
                                // Here there be dragons, beware..
                            }

                            if(timestamp.AddSeconds(20) < DateTime.Now)
                                break;

                            Thread.Sleep(200);
                            state = site.State;
                        }
                    } else {
                        if(site.State == ObjectState.Stopped || site.State == ObjectState.Stopping)
                            return;

                        _log.Debug("Stopping application pool {poolName}", _configuration.Name);

                        var state = ObjectState.Unknown;
                        while(state != ObjectState.Stopped) {
                            try {
                                site.Stop();
                            } catch(Exception) {
                                // Here there be dragons, beware..
                            }

                            if(timestamp.AddSeconds(20) < DateTime.Now)
                                break;

                            Thread.Sleep(200);
                            state = site.State;
                        }
                    }
                } catch(Exception e) {
                    _log.Error(e, "Failed while working with website {poolName}", _configuration.Name);
                }
            }
        }
    }
}