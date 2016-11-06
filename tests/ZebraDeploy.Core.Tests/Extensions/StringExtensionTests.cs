using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using ZebraDeploy.Core.Extensions;

namespace ZebraDeploy.Core.Tests.Extensions {
    [TestFixture, ExcludeFromCodeCoverage]
    public class StringExtensionTests {
        [Test]
        public void ReplaceMatchingValuesHandlesNullDictionary() {
            var result = "test".ReplaceMatchedValues(null);

            Assert.AreEqual("test", result);
        }

        [Test]
        public void ReplaceMatchingValuesHandlesEmptyDictionary() {
            var result = "test".ReplaceMatchedValues(new Dictionary<string, string>());

            Assert.AreEqual("test", result);
        }

        [Test]
        public void ReplaceMatchingValuesDoesntCrashOnNullString() {
            string source = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            var result = source.ReplaceMatchedValues(new Dictionary<string, string>());

            Assert.IsNull(result);
        }
        
        [Test]
        public void ReplaceMatchingValuesReplaceStuff() {
            var source = @"C:\inetput\documentation\${version}\";
            var dictionary = new Dictionary<string, string> {
                { "version", "1.0" }
            };
            var result = source.ReplaceMatchedValues(dictionary);

            Assert.AreEqual(@"C:\inetput\documentation\1.0\", result);
        }

        [Test]
        public void ReplaceMatchingValuesReplaceStuffWithCaseInsensitiveKeys() {
            var source = @"C:\inetput\documentation\${VeRsIoN}\";
            var dictionary = new Dictionary<string, string> {
                { "vErSiOn", "1.0" }
            };
            var result = source.ReplaceMatchedValues(dictionary);

            Assert.AreEqual(@"C:\inetput\documentation\1.0\", result);
        }
    }
}
