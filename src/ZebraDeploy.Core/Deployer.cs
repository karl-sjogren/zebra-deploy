using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Serilog;
using ZebraDeploy.Core.Configuration;

namespace ZebraDeploy.Core {
    public class Deployer : IDisposable {
        private readonly ILogger _log;
        private readonly ZebraConfiguration _configuration;
        private readonly DebouncingFileSystemWatcher _watcher;
        private readonly List<Stripe> _stripes;
        private readonly Dictionary<string, Thread> _threads;
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
        }

        private void WatcherFileCreated(object sender, FileSystemEventArgs fileSystemEventArgs) {
            var file = fileSystemEventArgs.Name;

            var stripe = _stripes.FirstOrDefault(s => s.File == file);
            if(stripe == null)
                return;

            lock (_lock) {
                var threadsToRemove = _threads.Where(kvp => !kvp.Value.IsAlive).Select(kvp => kvp.Key).ToArray();
                foreach(var key in threadsToRemove) {
                    _threads.Remove(key);
                }

                if(_threads.ContainsKey(file))
                    return;

                ThreadStart starter = () => ExecuteStripe(stripe);
                var thread = new Thread(starter);
                thread.Start();

                _threads.Add(file, thread);
            }
        }
        
        private void ExecuteStripe(Stripe stripe) {
            var zipPath = Path.Combine(_configuration.BasePath, stripe.File);
            _log.Information("Executing stripe for {file}.", zipPath);

            foreach(var step in stripe.Steps) {
                step.Invoke(stripe, zipPath);
            }

            try {
                File.Delete(zipPath);
            } catch(Exception) {
                Thread.Sleep(5000);
                File.Delete(zipPath);
            }

            Thread.CurrentThread.Abort();
        }

        public void Start() {
            _log.Information("Starting Zebra Deploy.");
            _watcher.Start();
        }

        public void Stop() {
            _log.Information("Stopping Zebra Deploy.");
            _watcher.Stop();
            lock (_lock) {
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
