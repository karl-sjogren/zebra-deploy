using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using Serilog;

namespace ZebraDeploy.Core {
    public class DebouncingFileSystemWatcher : IDisposable {
        private readonly ILogger _log = Log.ForContext<DebouncingFileSystemWatcher>();

        public event FileSystemEventHandler FileCreated;
        public event FileSystemEventHandler FileDeleted;
        public event FileSystemEventHandler FileChanged;

        private readonly Dictionary<string, DebouncedFileEvent> _debouncedFileEvents;
        private readonly object _lock = new object();

        private const int MilliSecondsSinceLastChange = 5000;
        private const int TimerInterval = 2000;

        private readonly FileSystemWatcher _watcher;
        private readonly Timer _triggerTimer;
        private readonly string _path;
        private readonly bool _requireExclusiveAccess;

        public DebouncingFileSystemWatcher(string path, string filter = null, bool requireExclusiveAccess = false) {
            _path = path;
            _requireExclusiveAccess = requireExclusiveAccess;
            _watcher = filter != null ? new FileSystemWatcher(path, filter) : new FileSystemWatcher(path);

            _debouncedFileEvents = new Dictionary<string, DebouncedFileEvent>();

            _triggerTimer = new Timer(TimerInterval);
            _triggerTimer.Elapsed += TimerTriggered;
            _triggerTimer.Enabled = true;
            _triggerTimer.Start();

            _watcher.EnableRaisingEvents = true;
            _watcher.Created += WatcherCreated;
            _watcher.Deleted += WatcherDeleted;
            _watcher.Changed += WatcherChanged;
        }

        public void Start() {
            _log.Information("Started watching path {path}", _path);
            _watcher.EnableRaisingEvents = true;
        }

        public void Stop() {
            _log.Information("Stopped watching path {path}", _path);
            _watcher.EnableRaisingEvents = false;
        }

        private void TimerTriggered(object sender, ElapsedEventArgs e) {
            lock (_lock) {
                foreach(var kvp in _debouncedFileEvents) {
                    var fileEvent = kvp.Value;
                    var fileInfo = new FileInfo(kvp.Value.Filename);

                    if(fileEvent.Created.HasValue) {
                        if(fileEvent.Created.Value.AddMilliseconds(MilliSecondsSinceLastChange) > DateTime.Now)
                            break;

                        if(fileEvent.Changed.HasValue && fileEvent.Changed.Value.AddMilliseconds(MilliSecondsSinceLastChange) > DateTime.Now)
                            break;

                        if(_requireExclusiveAccess && FileUtil.WhoIsLocking(fileInfo.FullName).Any()) {
                            _log.Debug("Waiting for exclusive file access before triggering FileCreated for {path}", kvp.Key);
                            break;
                        }

                        var args = new FileSystemEventArgs(WatcherChangeTypes.Created, _path, kvp.Key);
                        fileEvent.Created = null;
                        _log.Debug("Triggering FileCreated for {path}", kvp.Key);
                        FileCreated?.Invoke(this, args);
                    }

                    if(fileEvent.Changed.HasValue) {
                        if(fileEvent.Changed.Value.AddMilliseconds(MilliSecondsSinceLastChange) > DateTime.Now)
                            break;

                        if(_requireExclusiveAccess && FileUtil.WhoIsLocking(fileInfo.FullName).Any()) {
                            _log.Debug("Waiting for exclusive file access before triggering FileChanged for {path}", kvp.Key);
                            break;
                        }

                        var args = new FileSystemEventArgs(WatcherChangeTypes.Created, _path, kvp.Key);
                        fileEvent.Changed = null;
                        _log.Debug("Triggering FileChanged for {path}", kvp.Key);
                        FileChanged?.Invoke(this, args);
                    }
                }

                var keysToRemove = _debouncedFileEvents.Where(kvp => !kvp.Value.Created.HasValue && !kvp.Value.Changed.HasValue).Select(kvp => kvp.Key).ToArray();

                if(keysToRemove.Length > 0)
                    _log.Debug("Removing {count} finished events.", keysToRemove.Length);

                foreach(var key in keysToRemove) {
                    _debouncedFileEvents.Remove(key);
                }
            }
        }

        private void WatcherChanged(object sender, FileSystemEventArgs e) {
            _log.Debug("System triggered {changeType} for {path}.", e.ChangeType, e.FullPath);
            lock (_lock) {
                if(_debouncedFileEvents.ContainsKey(e.Name)) {
                    var fileEvent = _debouncedFileEvents[e.Name];
                    fileEvent.Changed = DateTime.Now;
                    return;
                }

                _debouncedFileEvents.Add(e.Name, new DebouncedFileEvent {
                    Changed = DateTime.Now,
                    Filename = e.FullPath
                });
            }
        }

        private void WatcherDeleted(object sender, FileSystemEventArgs e) {
            _log.Debug("System triggered {changeType} for {path}.", e.ChangeType, e.FullPath);
            FileDeleted?.Invoke(this, e); // No need to debounce deleted events as far as I know
        }

        private void WatcherCreated(object sender, FileSystemEventArgs e) {
            _log.Debug("System triggered {changeType} for {path}.", e.ChangeType, e.FullPath);
            lock (_lock) {
                if(_debouncedFileEvents.ContainsKey(e.Name)) {
                    var fileEvent = _debouncedFileEvents[e.Name];
                    fileEvent.Created = DateTime.Now;
                    return;
                }

                _debouncedFileEvents.Add(e.Name, new DebouncedFileEvent {
                    Created = DateTime.Now,
                    Filename = e.FullPath
                });
            }
        }

        public void Dispose() {
            _watcher.Dispose();
            _triggerTimer.Stop();
            _triggerTimer.Dispose();
        }

        private class DebouncedFileEvent {
            public DateTime? Created { get; set; }
            public DateTime? Changed { get; set; }
            public string Filename { get; set; }
        }
    }
}
