using System.Linq;
using NUnit.Framework;
using ZebraDeploy.Core.Configuration;

namespace ZebraDeploy.Core.Tests.Configuration {
    [TestFixture]
    public class ZebraConfigurationTests {
        [Test]
        public void LoadSampleConfiguration() {
            var config = ZebraConfiguration.Load(Resources.SampleConfiguration);

            Assert.IsNotNull(config);
            Assert.AreEqual(@"D:\zebra-deploy\", config.BasePath);
            Assert.AreEqual(1, config.Stripes.Count);

            var stripe = config.Stripes.First();
            Assert.AreEqual("website.zip", stripe.File);
            Assert.AreEqual(4, stripe.Steps.Count);
            Assert.AreEqual(1, stripe.Steps.Count(step => step.GetType() == typeof(CleanStepConfiguration)));
            Assert.AreEqual(1, stripe.Steps.Count(step => step.GetType() == typeof(OutputStepConfiguration)));
            Assert.AreEqual(2, stripe.Steps.Count(step => step.GetType() == typeof(AppPoolStepConfiguration)));
        }

        [Test]
        public void CleanStepHasExcludes() {
            var config = ZebraConfiguration.Load(Resources.SampleConfiguration);
            
            var stripe = config.Stripes.First();
            var cleanStep = stripe.Steps.Where(step => step.GetType() == typeof (CleanStepConfiguration)).Cast<CleanStepConfiguration>().First();
            Assert.AreEqual(1, cleanStep.Excludes.Count);
            Assert.AreEqual("images", cleanStep.Excludes.First());
        }
    }
}
