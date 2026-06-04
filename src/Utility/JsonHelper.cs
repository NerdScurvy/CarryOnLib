using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Newtonsoft.Json.Linq;

namespace CarryOn.Utility
{
    public static class JsonHelper
    {
        public static bool TryGetBool(JsonObject json, string key, out bool result)
        {
            if (!json.KeyExists(key))
            {
                result = false;
                return false;
            }
            result = json[key].AsBool();
            return true;
        }

        public static bool TryGetString(JsonObject json, string key, out string? result)
        {
            if (!json.KeyExists(key))
            {
                result = null;
                return false;
            }
            result = json[key].AsString();
            return result != null;
        }        
        public static bool TryGetInt(JsonObject json, string key, out int result)
        {
            if (!json.KeyExists(key))
            {
                result = 0;
                return false;
            }
            result = json[key].AsInt();
            return true;
        }

        public static bool TryGetFloat(JsonObject json, string key, out float result)
        {
            if (!json.KeyExists(key))
            {
                result = float.NaN;
                return false;
            }            
            result = json[key].AsFloat(float.NaN);
            return !float.IsNaN(result);
        }

        public static bool TryGetVec3f(JsonObject json, string key, out Vec3f? result)
        {
            if (!json.KeyExists(key))
            {
                result = null;
                return false;
            }
            // Try array form: [x, y, z]
            var floats = json[key].AsArray<float>();
            if (floats != null && floats.Length == 3)
            {
                result = new Vec3f(floats);
                return true;
            }
            // Try object form: { x: ..., y: ..., z: ... } with partials allowed (missing -> 0)
            float xVal = json[key]["x"].AsFloat(float.NaN);
            float yVal = json[key]["y"].AsFloat(float.NaN);
            float zVal = json[key]["z"].AsFloat(float.NaN);
            bool anyProvided = !(float.IsNaN(xVal) && float.IsNaN(yVal) && float.IsNaN(zVal));
            if (anyProvided)
            {
                if (float.IsNaN(xVal)) xVal = 0f;
                if (float.IsNaN(yVal)) yVal = 0f;
                if (float.IsNaN(zVal)) zVal = 0f;
                result = new Vec3f(xVal, yVal, zVal);
                return true;
            }
            result = null;
            return false;
        }

        public static bool TryGetVec4f(JsonObject json, string key, out Vec4f? result)
        {
            if (!json.KeyExists(key))
            {
                result = null;
                return false;
            }
            // Try array form: [x, y, z, w]
            var floats = json[key].AsArray<float>();
            if (floats != null && floats.Length == 4)
            {
                result = new Vec4f();
                result.Set(floats);
                return true;
            }
            // Try object form: { x: ..., y: ..., z: ..., w: ... } with partials allowed (missing -> 0)
            float xVal = json[key]["x"].AsFloat(float.NaN);
            float yVal = json[key]["y"].AsFloat(float.NaN);
            float zVal = json[key]["z"].AsFloat(float.NaN);
            float wVal = json[key]["w"].AsFloat(float.NaN);
            bool anyProvided = !(float.IsNaN(xVal) && float.IsNaN(yVal) && float.IsNaN(zVal) && float.IsNaN(wVal));
            if (anyProvided)
            {
                if (float.IsNaN(xVal)) xVal = 0f;
                if (float.IsNaN(yVal)) yVal = 0f;
                if (float.IsNaN(zVal)) zVal = 0f;
                if (float.IsNaN(wVal)) wVal = 0f;
                result = new Vec4f(xVal, yVal, zVal, wVal);
                return true;
            }
            result = null;
            return false;
        }
    

        public static bool TryGetVec3i(JsonObject json, string key, out Vec3i? result)
        {
            if (!json.KeyExists(key))
            {
                result = null;
                return false;
            }
            var ints = json[key].AsArray<int>();
            if (ints != null && ints.Length == 3)
            {
                result = new Vec3i(ints[0], ints[1], ints[2]);
                return true;
            }
            result = null;
            return false;
        }
        
        public static bool TryGetStringArray(JsonObject json, string key, out string?[]? result)
        {
            return TryGetArray(json, key, out result);
        }

        /// <summary>
        /// Tries to get an array value from a JsonObject key.
        /// </summary>
        public static bool TryGetArray<T>(JsonObject json, string key, out T?[]? result)
        {
            if (json == null || !json.KeyExists(key))
            {
                result = null;
                return false;
            }

            result = json[key].AsArray<T>();
            return result != null;
        }

        /// <summary>
        /// Tries to get a JObject value from a JsonObject key.
        /// </summary>
        public static bool TryGetObject(JsonObject json, string key, out JObject? result)
        {
            result = null;
            if (json == null || !json.KeyExists(key))
            {
                return false;
            }

            return TryAsObject(json[key]?.Token, out result);
        }

        /// <summary>
        /// Tries to cast a JToken to JObject.
        /// </summary>
        public static bool TryAsObject(JToken? token, out JObject? result)
        {
            result = token as JObject;
            return result != null;
        }

        /// <summary>
        /// Tries to get a JArray value from a JsonObject key.
        /// </summary>
        public static bool TryGetArray(JsonObject json, string key, out JArray? result)
        {
            result = null;
            if (json == null || !json.KeyExists(key))
            {
                return false;
            }

            result = json[key]?.Token as JArray;
            return result != null;
        }

        /// <summary>
        /// Returns true when token is null, JSON null, or undefined.
        /// </summary>
        public static bool IsNullOrUndefined(JToken token)
        {
            return token == null || token.Type == JTokenType.Null || token.Type == JTokenType.Undefined;
        }

        /// <summary>
        /// Tries to read a boolean from a dictionary of JTokens.
        /// </summary>
        public static bool TryGetTokenBool(IDictionary<string, JToken> dict, string key, bool defaultValue)
        {
            return dict != null && dict.TryGetValue(key, out var token) && token.Type == JTokenType.Boolean
                ? token.Value<bool>()
                : defaultValue;
        }

        /// <summary>
        /// Tries to read a float or int as float from a dictionary of JTokens.
        /// </summary>
        public static float TryGetTokenFloat(IDictionary<string, JToken> dict, string key, float defaultValue)
        {
            if (dict != null && dict.TryGetValue(key, out var token))
            {
                if (token.Type == JTokenType.Float) return token.Value<float>();
                if (token.Type == JTokenType.Integer) return token.Value<int>();
            }

            return defaultValue;
        }

        /// <summary>
        /// Tries to read a string array from a dictionary of JTokens.
        /// </summary>
        public static string[] TryGetTokenStringArray(IDictionary<string, JToken> dict, string key, string[] defaultValue)
        {
            return dict != null && dict.TryGetValue(key, out var token) && token.Type == JTokenType.Array
                ? token.ToObject<string[]>()!
                : defaultValue;
        }

        /// <summary>
        /// Tries to get a value from a dictionary by exact key, then by trimmed case-insensitive key.
        /// </summary>
        public static bool TryGetValueTrimmedIgnoreCase<T>(IDictionary<string, T> map, string key, out T? value)
        {
            value = default;

            if (map == null || string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

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
        public static bool TryGetTrailingWildcardValue<T>(IDictionary<string, T> map, string input, out T? value, System.Func<T, bool>? canUseValue = null)
        {
            value = default;

            if (map == null || string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

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

        /// <summary>
        /// Parses a JSON object into a case-insensitive dictionary of string keys to float values.
        /// Non-numeric values and empty keys are ignored.
        /// </summary>
        /// <param name="json">JSON object containing key/value pairs.</param>
        /// <returns>Case-insensitive map of parsed float values.</returns>
        public static IDictionary<string, float> ParseFloatMap(JsonObject json)
        {
            var map = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);

            if (json?.Exists != true || json.Token is not JObject obj)
            {
                return map;
            }

            foreach (var prop in obj.Properties())
            {
                if (string.IsNullOrWhiteSpace(prop.Name))
                {
                    continue;
                }

                if (prop.Value.Type is not (JTokenType.Integer or JTokenType.Float))
                {
                    continue;
                }

                map[prop.Name.Trim()] = prop.Value.Value<float>();
            }

            return map;
        }

        public static ModelTransform GetTransform(JsonObject json, ModelTransform? baseTransform)
        {
            var trans = baseTransform?.Clone() ?? new ModelTransform();
            if (TryGetVec3f(json, "translation", out var t))
            {
                trans.Translation = t;
            }
            else if (baseTransform != null)
            {
                trans.Translation = baseTransform.Translation;
            }
            if (TryGetVec3f(json, "rotation", out var r))
            {
                trans.Rotation = r;
            }
            else if (baseTransform != null)
            {
                trans.Rotation = baseTransform.Rotation;
            }
            if (TryGetVec3f(json, "origin", out var o))
            {
                trans.Origin = o;
            }
            else if (baseTransform != null)
            {
                trans.Origin = baseTransform.Origin;
            }
            // Try to get scale both as a Vec3f and single float - for compatibility reasons.
            var hasVecScale = TryGetVec3f(json, "scale", out var sv);
            var hasFloatScale = TryGetFloat(json, "scale", out var sf);
            if (hasVecScale)
            {
                trans.ScaleXYZ = sv;
            }
            if (hasFloatScale)
            {
                trans.ScaleXYZ = new Vec3f(sf, sf, sf);
            }
            else if (!hasVecScale && baseTransform != null)
            {
                trans.ScaleXYZ = baseTransform.ScaleXYZ;
            }

            // Apply per-axis overrides on top of any full vector values.
            if (TryGetFloat(json, "translationX", out var tx))
            {
                trans.Translation.X = tx;
            }
            if (TryGetFloat(json, "translationY", out var ty))
            {
                trans.Translation.Y = ty;
            }
            if (TryGetFloat(json, "translationZ", out var tz))
            {
                trans.Translation.Z = tz;
            }

            if (TryGetFloat(json, "rotationX", out var rx))
            {
                trans.Rotation.X = rx;
            }
            if (TryGetFloat(json, "rotationY", out var ry))
            {
                trans.Rotation.Y = ry;
            }
            if (TryGetFloat(json, "rotationZ", out var rz))
            {
                trans.Rotation.Z = rz;
            }

            if (TryGetFloat(json, "scaleX", out var sx))
            {
                trans.ScaleXYZ.X = sx;
            }
            if (TryGetFloat(json, "scaleY", out var sy))
            {
                trans.ScaleXYZ.Y = sy;
            }
            if (TryGetFloat(json, "scaleZ", out var sz))
            {
                trans.ScaleXYZ.Z = sz;
            }

            return trans;
        }
        
        public static bool HasAnyTransformValue(JsonObject json)
        {
            if (json == null || !json.Exists)
            {
                return false;
            }

            return json.KeyExists("translation")
                || json.KeyExists("rotation")
                || json.KeyExists("origin")
                || json.KeyExists("scale")
                || json.KeyExists("translationX")
                || json.KeyExists("translationY")
                || json.KeyExists("translationZ")
                || json.KeyExists("rotationX")
                || json.KeyExists("rotationY")
                || json.KeyExists("rotationZ")
                || json.KeyExists("scaleX")
                || json.KeyExists("scaleY")
                || json.KeyExists("scaleZ");
        }        
    }
}
