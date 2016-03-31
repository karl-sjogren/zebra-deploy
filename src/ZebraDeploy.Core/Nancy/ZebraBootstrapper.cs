using System.Collections.Generic;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;

namespace ZebraDeploy.Core.Nancy {
    public class ZebraBootstrapper : DefaultNancyBootstrapper {
        private readonly List<Stripe> _stripes;

        public ZebraBootstrapper(List<Stripe> stripes) {
            _stripes = stripes;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container) {
            base.ConfigureApplicationContainer(container);

            container.Register<IList<Stripe>>(_stripes);
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context) {
            pipelines.AfterRequest.AddItemToEndOfPipeline(ctx => {
                ctx.Response.WithHeader("Access-Control-Allow-Origin", "*")
                    .WithHeader("Access-Control-Allow-Methods", "GET")
                    .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type");
            });

            base.RequestStartup(container, pipelines, context);
        }

        protected override void ConfigureConventions(NancyConventions conventions) {
            base.ConfigureConventions(conventions);

            conventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("assets", @"WebAssets")
            );
        }
    }
}
