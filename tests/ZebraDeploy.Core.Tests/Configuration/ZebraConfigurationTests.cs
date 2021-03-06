﻿using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;
using ZebraDeploy.Core.Configuration;
using ZebraDeploy.Core.Configuration.Reporters;

namespace ZebraDeploy.Core.Tests.Configuration {
    [TestFixture, ExcludeFromCodeCoverage]
    public class ZebraConfigurationTests {
        [Test]
        public void LoadSampleConfiguration() {
            var config = ZebraConfiguration.Load(Resources.SampleConfiguration);

            Assert.IsNotNull(config);
            Assert.AreEqual(@"D:\zebra-deploy\", config.BasePath);
            Assert.AreEqual(1, config.Stripes.Count);

            var stripe = config.Stripes.First();
            Assert.AreEqual("website.zip", stripe.File);
            Assert.AreEqual(1, stripe.Steps.Count(step => step.GetType() == typeof(CleanStepConfiguration)));
            Assert.AreEqual(1, stripe.Steps.Count(step => step.GetType() == typeof(OutputStepConfiguration)));
            Assert.AreEqual(2, stripe.Steps.Count(step => step.GetType() == typeof(AppPoolStepConfiguration)));
            Assert.AreEqual(2, stripe.Steps.Count(step => step.GetType() == typeof(WebsiteStepConfiguration)));
            Assert.AreEqual(2, stripe.Steps.Count(step => step.GetType() == typeof(ServiceStepConfiguration)));
        }

        [Test]
        public void CleanStepHasExcludes() {
            var config = ZebraConfiguration.Load(Resources.SampleConfiguration);
            
            var stripe = config.Stripes.First();
            var cleanStep = stripe.Steps.Where(step => step.GetType() == typeof (CleanStepConfiguration)).Cast<CleanStepConfiguration>().First();
            Assert.AreEqual(1, cleanStep.Excludes.Count);
            Assert.AreEqual("images", cleanStep.Excludes.First());
        }

        [Test]
        public void LoadSampleConfigurationWithLocalReporters() {
            var config = ZebraConfiguration.Load(Resources.SampleConfigurationWithReporters);

            Assert.IsNotNull(config);
            Assert.AreEqual(1, config.Stripes.Count);

            var stripe = config.Stripes.First();
            Assert.AreEqual(1, stripe.Reporters.Count);
            Assert.AreEqual(1, stripe.Reporters.Count(step => step.GetType() == typeof(HipChatReporterConfiguration)));
            Assert.IsTrue(stripe.Reporters.First().ReportSuccess);
            Assert.IsTrue(stripe.Reporters.First().ReportFailure);
        }

        [Test]
        public void LoadSampleConfigurationWithGlobalReporters() {
            var config = ZebraConfiguration.Load(Resources.SampleConfigurationWithReporters);

            Assert.IsNotNull(config);

            Assert.AreEqual(1, config.Reporters.Count);
            Assert.AreEqual(1, config.Reporters.Count(step => step.GetType() == typeof(HipChatReporterConfiguration)));
            Assert.IsFalse(config.Reporters.First().ReportSuccess);
            Assert.IsFalse(config.Reporters.First().ReportFailure);
        }
    }
}
