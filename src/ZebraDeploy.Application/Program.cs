using System;
using ZebraDeploy.Core;

namespace ZebraDeploy.Application {
    class Program {
        static void Main(string[] args) {
            var deployer = new Deployer();
            deployer.Start();
            
            Console.ReadKey();

            deployer.Stop();
        }
    }
}
