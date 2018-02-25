using System;
using System.Security.Cryptography;
using System.Text;

namespace ZebraDeploy.Core.Nancy.Models {
    public class StripeModel {
        public string Id {
            get {
                var bytes = Encoding.UTF8.GetBytes(File);

                using(var md5Hasher = MD5.Create()) {
                    var data = md5Hasher.ComputeHash(bytes);

                    return new Guid(data).ToString("N");
                }
            }
        }

        public string File { get; set; }
        public double Progress { get; set; }
        public string CurrentStep { get; set; }
        public DateTime LastDeploy { get; set; }
        public bool Failed { get; set; }
    }
}
