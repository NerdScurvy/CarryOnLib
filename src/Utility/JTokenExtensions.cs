using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace CarryOn.Utility
{
    public static class JTokenExtensions
    {
        public static bool TryGetBool(this Dictionary<string, JToken> dict, string key, bool defaultValue)
            => JsonHelper.TryGetTokenBool(dict, key, defaultValue);

        public static float TryGetFloat(this Dictionary<string, JToken> dict, string key, float defaultValue)
            => JsonHelper.TryGetTokenFloat(dict, key, defaultValue);

        public static string[] TryGetStringArray(this Dictionary<string, JToken> dict, string key, string[] defaultValue)
            => JsonHelper.TryGetTokenStringArray(dict, key, defaultValue);
    }
}
