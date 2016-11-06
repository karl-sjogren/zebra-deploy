using System.Collections.Generic;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace ZebraDeploy.Core.Extensions {
    public static class StringExtensions {
        /// <summary>
        /// Replaces any instances of ${key} with the corresponding value from the dictionary.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="matchedValues">The dictionary with keys to use for matching.</param>
        /// <returns>The source string with any instances of ${key} replaced.</returns>
        public static string ReplaceMatchedValues([CanBeNull]this string source, [CanBeNull]Dictionary<string, string> matchedValues) {
            if(matchedValues == null || matchedValues.Count == 0)
                return source;

            if(string.IsNullOrWhiteSpace(source))
                return source;

            foreach(var matchPair in matchedValues) {
                var regex = new Regex(Regex.Escape("${" + matchPair.Key + "}"), RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
                source = regex.Replace(source, matchPair.Value);
            }

            return source;
        }
    }
}