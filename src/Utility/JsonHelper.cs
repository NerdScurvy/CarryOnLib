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

        public static bool TryGetVec4f(JsonObject json, string key, out Vec4f result)
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
            var success = strings != null;
            result = strings;
            return success;
        }        

        public static ModelTransform GetTransform(JsonObject json, ModelTransform baseTransform)
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
    }
}
