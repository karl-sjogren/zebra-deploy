using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Nancy.Hosting.Self;
using Serilog;
using ZebraDeploy.Core.Configuration;
using ZebraDeploy.Core.Nancy;
using ZebraDeploy.Core.StripeSteps.Reporters;

namespace ZebraDeploy.Core {
    public class Deployer : IDisposable {
        private readonly ILogger _log;
        private readonly ZebraConfiguration _configuration;
        private readonly DebouncingFileSystemWatcher _watcher;
        private readonly List<Stripe> _stripes;
        private readonly List<StripeReporter> _globalReporters;
        private readonly Dictionary<string, Thread> _threads;
        private readonly NancyHost _host;
        private readonly object _lock = new object();

        public Deployer(string configurationFile = "config.xml") {

            if(!Directory.Exists("logs"))
                Directory.CreateDirectory("logs");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.ColoredConsole()
                .WriteTo.RollingFile(@"logs\zebra-deploy-{Date}.log")
                .CreateLogger();

            _log = Log.ForContext<Deployer>(); // Recreate this so we can log properly

            _configuration = ZebraConfiguration.LoadFromFile(configurationFile);
            _watcher = new DebouncingFileSystemWatcher(_configuration.BasePath, "*.zip");
            _threads = new Dictionary<string, Thread>();

            _watcher.FileCreated += WatcherFileCreated;
            _stripes = _configuration.Stripes.Select(c => new Stripe(c)).ToList();
            _globalReporters = _configuration.Reporters.Select(StripeReporter.CreateStep).Where(x => x != null).ToList();

            var nancyPort = 7777;
            try {
                var bootstrapper = new ZebraBootstrapper(_stripes);
                _host = new NancyHost(new Uri("http://localhost:" + nancyPort), bootstrapper, new HostConfiguration {
                    RewriteLocalhost = true
                });
            } catch(HttpListenerException) {
                _log.Warning("Failed to start NancyHost for port {port}.", nancyPort);
            }
        }

        private void WatcherFileCreated(object sender, FileSystemEventArgs fileSystemEventArgs) {
            var file = fileSystemEventArgs.Name;

            var stripe = _stripes
                .Select(s => new { StripeInstance = s, MatchValues = s.ExecuteFor(file) })
                .FirstOrDefault(s => s.MatchValues != null);

            if(stripe == null)
                return;

            lock(_lock) {
                var threadsToRemove = _threads.Where(kvp => !kvp.Value.IsAlive).Select(kvp => kvp.Key).ToArray();
                foreach(var key in threadsToRemove) {
                    _threads.Remove(key);
                }

                if(_threads.ContainsKey(file))
                    return;

                ThreadStart starter = () => ExecuteStripe(stripe.StripeInstance, stripe.MatchValues);
                var thread = new Thread(starter);
                thread.Start();

                _threads.Add(file, thread);
            }
        }

        private void ExecuteStripe(Stripe stripe, Dictionary<string, string> matchValues) {
            var zipPath = Path.Combine(_configuration.BasePath, stripe.File);
            _log.Information("Executing stripe for {file}.", zipPath);

            stripe.Progress = 0;
            stripe.Failed = false;

            foreach(var step in stripe.Steps) {
                try {
                    stripe.CurrentStep = step.ToString();
                    stripe.Progress = (double)stripe.Steps.ToList().IndexOf(step) / stripe.Steps.Count * 100;

                    step.Invoke(stripe, matchValues, zipPath);
                } catch(Exception e) {
                    stripe.Failed = true;
                    _log.Error(e, "Failed to execute step {type}.", step.GetType().Name);

                    foreach(var reporter in stripe.Reporters.Concat(_globalReporters).Where(r => r.ReportFailure)) {
                        try {
                            reporter.Invoke(stripe);
                        } catch(Exception ex) {
                            _log.Error(ex, "Failed to execute reporter {type}.", reporter.GetType().Name);
                        }
                    }

                    return;
                }
            }

            try {
                File.Delete(zipPath);
            } catch(Exception) {
                Thread.Sleep(5000);
                File.Delete(zipPath);
            }

            stripe.LastDeploy = DateTime.Now;
            stripe.Progress = 100;
            stripe.CurrentStep = "Done";

            foreach(var reporter in stripe.Reporters.Concat(_globalReporters).Where(r => r.ReportSuccess)) {
                try {
                    reporter.Invoke(stripe);
                } catch(Exception e) {
                    _log.Error(e, "Failed to execute reporter {type}.", reporter.GetType().Name);
                }
            }

            _log.Information("Executed stripe for {file}.", zipPath);
            Thread.CurrentThread.Abort();
        }

        public void Start() {
            _log.Information("Starting Zebra Deploy.");
            _watcher.Start();
            _host.Start();

        }

        public void Stop() {
            _log.Information("Stopping Zebra Deploy.");
            _watcher.Stop();
            _host.Stop();
            _host.Dispose();

            lock(_lock) {
                foreach(var kvp in _threads) {
                    if(kvp.Value.IsAlive)
                        kvp.Value.Abort();
                }
            }
        }

        public void Dispose() {
            Stop();
            _watcher.Dispose();
        }
    }
}
