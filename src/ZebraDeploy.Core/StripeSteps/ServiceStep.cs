using System;
using System.ServiceProcess;
using System.Threading;
using Serilog;
using ZebraDeploy.Core.Configuration;

namespace ZebraDeploy.Core.StripeSteps {
    public class ServiceStep : StripeStep {
        private readonly ILogger _log = Log.ForContext<ServiceStep>();
        private readonly ServiceStepConfiguration _configuration;

        public ServiceStep(ServiceStepConfiguration configuration) {
            _configuration = configuration;
        }

        public override string ToString() {
            if(_configuration.Action == "start")
                return "Start Service " + _configuration.Name;

            return "Stop Service " + _configuration.Name;
        }

        public override void Invoke(Stripe stripe, string zipPath) {
            try {
                var service = new ServiceController(_configuration.Name);

                try {
                    service.Refresh();
                } catch(Exception) {
                    throw new InvalidOperationException($"Failed to locate refresh values for service {_configuration.Name}.");
                }

                var timestamp = DateTime.Now;
                if(_configuration.Action == "start") {
                    if(service.Status == ServiceControllerStatus.Running)
                        return;

                    _log.Debug("Starting service {serviceName}", _configuration.Name);

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

                    _log.Debug("Stopping service {serviceName}", _configuration.Name);

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
                _log.Error(e, "Failed while working with service {serviceName}", _configuration.Name);
            }
        }
    }
}