using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Web.Administration;
using Serilog;
using ZebraDeploy.Core.Configuration;
using ZebraDeploy.Core.Extensions;

namespace ZebraDeploy.Core.StripeSteps {
    public class AppPoolStep : StripeStep {
        private readonly ILogger _log = Log.ForContext<AppPoolStep>();
        private readonly AppPoolStepConfiguration _configuration;
        private static readonly object _lock = new object();

        public AppPoolStep(AppPoolStepConfiguration configuration) {
            _configuration = configuration;
        }
        
        public override void Invoke(Stripe stripe, Dictionary<string, string> matchValues, string zipPath) {
            // Server manager seems to be using some COM behind the scenes
            // that doesn't like it multithreaded..
            lock(_lock) {
                var configName = _configuration.Name.ReplaceMatchedValues(matchValues);

                if(_configuration.Action == "start")
                    StripeDescription = "Start application pool " + configName;
                else
                    StripeDescription = "Stop application pool " + configName;

                try {
                    var manager = new ServerManager();
                    var pool = manager.ApplicationPools[configName];
                    if(pool == null)
                        throw new InvalidOperationException($"Failed to locate application pool named {configName}.");


                    var timestamp = DateTime.Now;
                    if(_configuration.Action == "start") {
                        if(pool.State == ObjectState.Started || pool.State == ObjectState.Starting)
                            return;

                        _log.Debug("Starting application pool {poolName}", configName);

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

                        _log.Debug("Stopping application pool {poolName}", configName);

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
                    _log.Error(e, "Failed while working with application pool {poolName}", configName);
                }
            }
        }
    }
}