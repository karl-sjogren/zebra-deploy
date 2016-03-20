﻿using System.ServiceProcess;
using ZebraDeploy.Core;

namespace ZebraDeploy.Service {
    public partial class ZebraDeploy : ServiceBase {
        private Deployer _deployer;

        public ZebraDeploy() {
            InitializeComponent();
        }

        protected override void OnStart(string[] args) {
            _deployer?.Dispose(); // Shouldn't happen, but let's be sure

            _deployer = new Deployer();
            _deployer.Start();
        }

        protected override void OnStop() {
            _deployer?.Dispose();
        }
    }
}
