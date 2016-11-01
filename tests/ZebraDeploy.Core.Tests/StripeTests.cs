using System.Xml.Linq;
using NUnit.Framework;
using ZebraDeploy.Core.Configuration;

namespace ZebraDeploy.Core.Tests {
    [TestFixture]
    public class StripeTests {
        [TestCase("website.zip", "website.zip", true)]
        [TestCase("website.zip", "WEBSITE.zip", true)]

        [TestCase("website", "website.zip", false)]
        [TestCase("website.zip", "application.zip", false)]

        [TestCase("website-(?:[\\.\\d]*?).zip", "website-1.0.0.zip", true)]
        [TestCase("website-(?<Version>[\\.\\d]*?).zip", "website-1.0.0.zip", true)]
        public void StripeFileMatches(string filePattern, string fileName, bool expectedResult) {
            var doc = new XElement("stripe", new XAttribute("file", filePattern));
            var config = new StripeConfiguration(doc);
            var stripe = new Stripe(config);

            var result = stripe.ExecuteFor(fileName);

            Assert.AreEqual(expectedResult, result != null);
        }

        [TestCase("website-(?<Version>[\\.\\d]*?).zip", "website-1.0.0.zip", "Version", "1.0.0")]
        [TestCase("website-(?<Version>\\d).(?<Version>\\d).(?<Version>\\d).zip", "website-1.0.0.zip", "Version", "0", Description = "Should use last group match, ie 0.")]
        public void StripeExtractsMatchGroups(string filePattern, string fileName, string groupName, string groupValue) {
            var doc = new XElement("stripe", new XAttribute("file", filePattern));
            var config = new StripeConfiguration(doc);
            var stripe = new Stripe(config);

            var result = stripe.ExecuteFor(fileName);

            Assert.IsNotNull(result);

            var value = result[groupName];

            Assert.AreEqual(groupValue, value);
        }
    }
}
