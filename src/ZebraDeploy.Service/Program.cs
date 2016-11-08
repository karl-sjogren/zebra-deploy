using System;
using System.Configuration.Install;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using JetBrains.Annotations;

namespace ZebraDeploy.Service {
    static class Program {
        static void Main(string[] args) {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if(Environment.UserInteractive) {
                string parameter = string.Concat(args);
                switch(parameter) {
                    case "--install":
                        ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
                        break;
                    case "--uninstall":
                        ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
                        break;
                }
            } else {
                ServiceBase[] servicesToRun = {
                    new ZebraDeploy()
                };
                ServiceBase.Run(servicesToRun);
            }
        }


        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            var ex = e.ExceptionObject as Exception;
            using(var writer = new StreamWriter(GetPathToFile($"UnhandledException-{DateTime.Now:yyyy-MM-dd HHmmss}.txt"), false)) {
                WriteException(writer, ex);
            }
        }

        [NotNull]
        private static String GetPathToFile([NotNull] String file) {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var executingAssemblyPath = executingAssembly.Location;
            var directoryName = new FileInfo(executingAssemblyPath).DirectoryName;
            var result = Path.Combine(directoryName, file);

            return result;
        }

        private static void WriteException([NotNull] TextWriter writer, [CanBeNull] Exception ex) {
            if(ex == null) {
                writer.WriteLine("Application crashed but no exception was provided.");
                writer.Flush();
                return;
            }
            writer.WriteLine("Type: {0}", ex.GetType().Name);
            writer.WriteLine("Message: {0}", ex.Message);
            writer.WriteLine("Stacktrace:");
            writer.WriteLine(ex.StackTrace);
            writer.Flush();

            if(ex.InnerException != null) {
                writer.WriteLine();
                WriteException(writer, ex.InnerException);
            }
        }
    }
}
