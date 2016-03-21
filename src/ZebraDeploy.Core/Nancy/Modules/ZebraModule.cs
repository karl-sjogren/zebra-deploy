using System.Collections.Generic;
using System.Linq;
using Nancy;
using ZebraDeploy.Core.Nancy.Models;

namespace ZebraDeploy.Core.Nancy.Modules {
    public class ZebraModule : NancyModule {
        public ZebraModule(IList<Stripe> stripes) : base("/") {

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
        }
    }
}
