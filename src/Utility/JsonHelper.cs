using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

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

        public static bool TryGetString(JsonObject json, string key, out string result)
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

        public static bool TryGetVec3f(JsonObject json, string key, out Vec3f result)
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

        public static bool TryGetVec3i(JsonObject json, string key, out Vec3i result)
        {
            if (!json.KeyExists(key))
            {
                result = null;
                return false;
            }
            var ints = json[key].AsArray<int>();
            var success = (ints?.Length == 3);
            result = success ? new Vec3i(ints[0], ints[1], ints[2]) : null;
            return success;
        }
        
        public static bool TryGetStringArray(JsonObject json, string key, out string[] result)
        {
            if (!json.KeyExists(key))
            {
                result = null;
                return false;
            }
            var strings = json[key].AsArray<string>();
            var success = true;
            result = strings;
            return success;
        }        

        public static ModelTransform GetTransform(JsonObject json, ModelTransform baseTransform)
        {
            var trans = baseTransform?.Clone() ?? new ModelTransform();
            if (TryGetVec3f(json, "translation", out var t)) trans.Translation = t;
            if (TryGetVec3f(json, "rotation", out var r)) trans.Rotation = r;
            if (TryGetVec3f(json, "origin", out var o)) trans.Origin = o;
            // Try to get scale both as a Vec3f and single float - for compatibility reasons.
            if (TryGetVec3f(json, "scale", out var sv)) trans.ScaleXYZ = sv;
            if (TryGetFloat(json, "scale", out var sf)) trans.ScaleXYZ = new Vec3f(sf, sf, sf);
            return trans;
        }
    }
}
