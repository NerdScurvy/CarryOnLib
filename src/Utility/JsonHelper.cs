using System;
using System.Collections.Generic;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Newtonsoft.Json.Linq;

namespace CarryOn.Utility
{
    public static class JsonHelper
    {
        /// <summary>
        /// Tries to read a boolean value from a JsonObject key. Returns false when the key
        /// is absent or the value cannot be read as a bool.
        /// </summary>
        /// <param name="json">The JSON object to read from.</param>
        /// <param name="key">The key to look up.</param>
        /// <param name="result">Set to the parsed value on success.</param>
        /// <returns>True if the key exists and the value was read; false otherwise.</returns>
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

        /// <summary>
        /// Tries to read a string value from a JsonObject key.
        /// </summary>
        /// <param name="json">The JSON object to read from.</param>
        /// <param name="key">The key to look up.</param>
        /// <param name="result">Set to the parsed value on success, or null on failure.</param>
        /// <returns>True if the key exists and resolved to a non-null string; false otherwise.</returns>
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
        /// <summary>
        /// Tries to read an integer value from a JsonObject key. Returns false when the key
        /// is absent.
        /// </summary>
        /// <param name="json">The JSON object to read from.</param>
        /// <param name="key">The key to look up.</param>
        /// <param name="result">Set to the parsed value on success.</param>
        /// <returns>True if the key exists and the value was read; false otherwise.</returns>
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

        /// <summary>
        /// Tries to read a float value from a JsonObject key.
        /// </summary>
        /// <param name="json">The JSON object to read from.</param>
        /// <param name="key">The key to look up.</param>
        /// <param name="result">Set to the parsed value on success.</param>
        /// <returns>
        /// True if the key exists and resolved to a valid (non-NaN) float; false otherwise.
        /// Note: <c>result</c> is <see cref="float.NaN"/> on both "key not found" and
        /// "value is NaN", so callers cannot distinguish those two cases - they only
        /// receive a true/false for existence-and-validity.
        /// </returns>
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

        /// <summary>
        /// Tries to read a 3-component vector from a JsonObject key. Supports array form
        /// <c>[x, y, z]</c> and object form <c>{ x, y, z }</c>; missing object components
        /// default to 0.
        /// </summary>
        /// <param name="json">The JSON object to read from.</param>
        /// <param name="key">The key to look up.</param>
        /// <param name="result">Set to the parsed vector on success, or null on failure.</param>
        /// <returns>True if a valid vector was parsed; false otherwise.</returns>
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

        /// <summary>
        /// Tries to read a 4-component vector from a JsonObject key. Supports array form
        /// <c>[x, y, z, w]</c> and object form <c>{ x, y, z, w }</c>; missing object components
        /// default to 0.
        /// </summary>
        /// <param name="json">The JSON object to read from.</param>
        /// <param name="key">The key to look up.</param>
        /// <param name="result">Set to the parsed vector on success, or null on failure.</param>
        /// <returns>True if a valid vector was parsed; false otherwise.</returns>
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
                result = new Vec4f(floats[0], floats[1], floats[2], floats[3]);
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
    

        /// <summary>
        /// Tries to read a 3-component integer vector from a JsonObject key. Supports array
        /// form <c>[x, y, z]</c> and object form <c>{ x, y, z }</c>; missing object
        /// components default to 0.
        /// </summary>
        /// <param name="json">The JSON object to read from.</param>
        /// <param name="key">The key to look up.</param>
        /// <param name="result">Set to the parsed vector on success, or null on failure.</param>
        /// <returns>True if a valid vector was parsed; false otherwise.</returns>
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
            // Try object form: { x: ..., y: ..., z: ... } with partials allowed (missing -> 0)
            var xObj = json[key]["x"];
            var yObj = json[key]["y"];
            var zObj = json[key]["z"];
            bool anyProvided = xObj.Exists || yObj.Exists || zObj.Exists;
            if (anyProvided)
            {
                int x = xObj.Exists ? xObj.AsInt(0) : 0;
                int y = yObj.Exists ? yObj.AsInt(0) : 0;
                int z = zObj.Exists ? zObj.AsInt(0) : 0;
                result = new Vec3i(x, y, z);
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
                ? token.ToObject<string[]>() ?? defaultValue
                : defaultValue;
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

    }
}
