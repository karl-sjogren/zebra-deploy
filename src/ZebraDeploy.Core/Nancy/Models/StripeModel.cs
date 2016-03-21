using System;

namespace ZebraDeploy.Core.Nancy.Models {
    public class StripeModel {
        public string File { get; set; }
        public double Progress { get; set; }
        public string CurrentStep { get; set; }
        public DateTime LastDeploy { get; set; }
        public bool Failed { get; set; }
    }
}
