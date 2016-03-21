using System;
using System.Threading;
using Microsoft.Web.Administration;
using Serilog;
using ZebraDeploy.Core.Configuration;

namespace ZebraDeploy.Core.StripeSteps {
    public class AppPoolStep : StripeStep {
        private readonly ILogger _log = Log.ForContext<AppPoolStep>();
        private readonly AppPoolStepConfiguration _configuration;
        private static readonly object _lock = new object();

        public AppPoolStep(AppPoolStepConfiguration configuration) {
            _configuration = configuration;
        }

        public override void Invoke(Stripe stripe, string zipPath) {
            // Server manager seems to be using some COM behind the scenes
            // that doesn't like it multithreaded..
            lock (_lock) {
                try {
                    var manager = new ServerManager();
                    var pool = manager.ApplicationPools[_configuration.Name];
                    if(pool == null)
                        throw new InvalidOperationException($"Failed to locate application pool named {_configuration.Name}.");


                    var timestamp = DateTime.Now;
                    if(_configuration.Action == "start") {
                        if(pool.State == ObjectState.Started || pool.State == ObjectState.Starting)
                            return;

                        _log.Debug("Starting application pool {poolName}", _configuration.Name);

                        var state = ObjectState.Unknown;
                        while(state != ObjectState.Started) {
                            try {
                                pool.Start();
                            } catch(Exception) {
                                // Here there be dragons, beware..
                            }

                            if(timestamp.AddSeconds(20) < DateTime.Now)
                                break;

                            Thread.Sleep(200);
                            state = pool.State;
                        }
                    } else {
                        if(pool.State == ObjectState.Stopped || pool.State == ObjectState.Stopping)
                            return;

                        _log.Debug("Stopping application pool {poolName}", _configuration.Name);

                        var state = ObjectState.Unknown;
                        while(state != ObjectState.Stopped) {
                            try {
                                pool.Stop();
                            } catch(Exception) {
                                // Here there be dragons, beware..
                            }

                            if(timestamp.AddSeconds(20) < DateTime.Now)
                                break;

                            Thread.Sleep(200);
                            state = pool.State;
                        }
                    }
                } catch(Exception e) {
                    _log.Error(e, "Failed while working with application pool {poolName}", _configuration.Name);
                }
            }
        }
    }
}