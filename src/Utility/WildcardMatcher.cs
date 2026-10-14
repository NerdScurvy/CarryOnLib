using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CarryOn.Utility
{
    /// <summary>
    /// Wildcard and pattern-matching helpers for string keys and dictionary lookups.
    /// </summary>
    public static class WildcardMatcher
    {
        /// <summary>
        /// Converts a glob-style wildcard pattern (using <c>*</c> as a wildcard) into a
        /// regex pattern string with <c>^</c> and <c>$</c> anchors. The caller chooses
        /// <see cref="RegexOptions"/> (e.g. case-sensitive vs. case-insensitive).
        /// </summary>
        public static string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
        }

        /// <summary>
        /// Tries to get a value from a dictionary by exact key, then by trimmed case-insensitive key.
        /// </summary>
        public static bool TryGetValueTrimmedIgnoreCase<T>(IDictionary<string, T> map, string key, out T? value)
        {
            value = default;

            if (map == null || string.IsNullOrWhiteSpace(key)) return false;

            if (map.TryGetValue(key, out value))
            {
                return true;
            }

            var trimmedKey = key.Trim();
            foreach (var entry in map)
            {
                if (!string.Equals(entry.Key?.Trim(), trimmedKey, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                value = entry.Value;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to get a value by trailing-* wildcard key where longest matching prefix wins.
        /// </summary>
        public static bool TryGetTrailingWildcardValue<T>(IDictionary<string, T> map, string input, out T? value, Func<T, bool>? canUseValue = null)
        {
            value = default;

            if (map == null || string.IsNullOrWhiteSpace(input)) return false;

            var bestPrefixLength = -1;
            var found = false;

            foreach (var entry in map)
            {
                var entryKey = entry.Key?.Trim();
                if (string.IsNullOrWhiteSpace(entryKey) || !entryKey.EndsWith("*", StringComparison.Ordinal))
                {
                    continue;
                }

                var prefix = entryKey.Substring(0, entryKey.Length - 1);
                if (string.IsNullOrEmpty(prefix) || !input.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (prefix.Length <= bestPrefixLength)
                {
                    continue;
                }

                if (canUseValue != null && !canUseValue(entry.Value))
                {
                    continue;
                }

                bestPrefixLength = prefix.Length;
                value = entry.Value;
                found = true;
            }

            return found;
        }
    }
}
