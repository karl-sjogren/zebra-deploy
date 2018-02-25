using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Nancy;
using ZebraDeploy.Core.Configuration;
using ZebraDeploy.Core.Nancy.Models;

namespace ZebraDeploy.Core.Nancy.Modules {
    public class ZebraModule : NancyModule {
        public ZebraModule(ZebraConfiguration configuration, IList<Stripe> stripes) : base("/") {

            Get["/"] = _ => {
                var model = stripes.Select(s => new StripeModel {
                    File = s.File,
                    CurrentStep = s.CurrentStep ?? "Waiting",
                    LastDeploy = s.LastDeploy,
                    Progress = s.Progress,
                    Failed = s.Failed
                }).ToArray();

                return Negotiate
                        .WithModel(model)
                        .WithView("Index");
            };

            Post["/"] = _ => {
                if(!configuration.AllowHttpUploads)
                    return Negotiate.WithStatusCode(HttpStatusCode.MethodNotAllowed);

                using(var rijAlg = new RijndaelManaged()) {
                    rijAlg.Key = Convert.FromBase64String(configuration.SecurityKey);
                    rijAlg.IV = Convert.FromBase64String(configuration.SecurityIV);
                    
                    var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                    
                    using(var csDecrypt = new CryptoStream(Request.Body, decryptor, CryptoStreamMode.Read)) {
                        using(var target = File.OpenWrite("")) {
                            csDecrypt.CopyTo(target);
                        }
                    }
                }

                return Negotiate
                    .WithStatusCode(HttpStatusCode.NoContent);
            };
        }
    }
}
