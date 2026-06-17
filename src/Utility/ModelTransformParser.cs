using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace CarryOn.Utility
{
    public static class ModelTransformParser
    {
        public static ModelTransform GetTransform(JsonObject json, ModelTransform? baseTransform)
        {
            var trans = baseTransform?.Clone() ?? new ModelTransform();
            if (JsonHelper.TryGetVec3f(json, "translation", out var t))
            {
                trans.Translation = t;
            }
            else if (baseTransform != null)
            {
                trans.Translation = baseTransform.Translation;
            }
            if (JsonHelper.TryGetVec3f(json, "rotation", out var r))
            {
                trans.Rotation = r;
            }
            else if (baseTransform != null)
            {
                trans.Rotation = baseTransform.Rotation;
            }
            if (JsonHelper.TryGetVec3f(json, "origin", out var o))
            {
                trans.Origin = o;
            }
            else if (baseTransform != null)
            {
                trans.Origin = baseTransform.Origin;
            }
            var hasVecScale = JsonHelper.TryGetVec3f(json, "scale", out var sv);
            var hasFloatScale = JsonHelper.TryGetFloat(json, "scale", out var sf);
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

            if (JsonHelper.TryGetFloat(json, "translationX", out var tx))
            {
                trans.Translation.X = tx;
            }
            if (JsonHelper.TryGetFloat(json, "translationY", out var ty))
            {
                trans.Translation.Y = ty;
            }
            if (JsonHelper.TryGetFloat(json, "translationZ", out var tz))
            {
                trans.Translation.Z = tz;
            }

            if (JsonHelper.TryGetFloat(json, "rotationX", out var rx))
            {
                trans.Rotation.X = rx;
            }
            if (JsonHelper.TryGetFloat(json, "rotationY", out var ry))
            {
                trans.Rotation.Y = ry;
            }
            if (JsonHelper.TryGetFloat(json, "rotationZ", out var rz))
            {
                trans.Rotation.Z = rz;
            }

            if (JsonHelper.TryGetFloat(json, "scaleX", out var sx))
            {
                trans.ScaleXYZ.X = sx;
            }
            if (JsonHelper.TryGetFloat(json, "scaleY", out var sy))
            {
                trans.ScaleXYZ.Y = sy;
            }
            if (JsonHelper.TryGetFloat(json, "scaleZ", out var sz))
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
