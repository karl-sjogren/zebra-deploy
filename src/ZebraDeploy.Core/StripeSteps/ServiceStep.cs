using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading;
using Serilog;
using ZebraDeploy.Core.Configuration;
using ZebraDeploy.Core.Extensions;

namespace ZebraDeploy.Core.StripeSteps {
    public class ServiceStep : StripeStep {
        private readonly ILogger _log = Log.ForContext<ServiceStep>();
        private readonly ServiceStepConfiguration _configuration;

        public ServiceStep(ServiceStepConfiguration configuration) {
            _configuration = configuration;
        }
        
        public override void Invoke(Stripe stripe, Dictionary<string, string> matchValues, string zipPath) {
            var configName = _configuration.Name.ReplaceMatchedValues(matchValues);
            
            if(_configuration.Action == "start")
                StripeDescription = "Start service " + configName;
            else
                StripeDescription = "Stop service " + configName;

            try {
                var service = new ServiceController(configName);

                try {
                    service.Refresh();
                } catch(Exception) {
                    throw new InvalidOperationException($"Failed to locate refresh values for service {configName}.");
                }

                var timestamp = DateTime.Now;
                if(_configuration.Action == "start") {
                    if(service.Status == ServiceControllerStatus.Running)
                        return;

                    _log.Debug("Starting service {serviceName}", configName);

                    var state = service.Status;
                    while(state != ServiceControllerStatus.Running) {
                        try {
                            service.Start();
                        } catch(Exception) {
                            // Here there be dragons, beware..
                        }

                        if(timestamp.AddSeconds(20) < DateTime.Now)
                            break;

                        Thread.Sleep(200);
                        service.Refresh();
                        state = service.Status;
                    }
                } else {
                    if(service.Status == ServiceControllerStatus.Stopped)
                        return;

                    _log.Debug("Stopping service {serviceName}", configName);

                    var state = service.Status;
                    while(state != ServiceControllerStatus.Stopped) {
                        try {
                            service.Stop();
                        } catch(Exception) {
                            // Here there be dragons, beware..
                        }

                        if(timestamp.AddSeconds(20) < DateTime.Now)
                            break;

                        Thread.Sleep(200);
                        service.Refresh();
                        state = service.Status;
                    }
                }
            } catch(Exception e) {
                _log.Error(e, "Failed while working with service {serviceName}", configName);
            }
        }
    }
}