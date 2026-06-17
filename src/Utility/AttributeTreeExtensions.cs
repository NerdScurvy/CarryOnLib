using System;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace CarryOn.Utility
{
    public static class AttributeTreeExtensions
    {
        public static IAttribute? TryGet(this IAttribute? attr, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (attr is not ITreeAttribute tree) return null;
                attr = tree[key];
            }
            return attr;
        }

        public static T? TryGet<T>(this IAttribute attr, params string[] keys)
                where T : class, IAttribute
            => TryGet(attr, keys) as T;

        public static void Set(this IAttribute? attr, IAttribute? value, params string[] keys)
        {
            if (attr == null) throw new ArgumentNullException(nameof(attr));
            for (var i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                if (attr is not ITreeAttribute tree)
                {
                    if ((attr == null) && (value == null)) return;
                    var getter = $"attr{keys.Take(i).Select(k => $"[\"{k}\"]")}";
                    var type = attr?.GetType()?.ToString() ?? "null";
                    throw new ArgumentException($"{getter} is {type}, not TreeAttribute.", nameof(attr));
                }
                if (i == keys.Length - 1)
                {
                    if (value != null) tree[key] = value;
                    else tree.RemoveAttribute(key);
                }
                else
                {
                    attr = tree[key] ??= new TreeAttribute();
                }
            }
        }

        public static void Remove(this IAttribute attr, params string[] keys)
            => Set(attr, (IAttribute?)null, keys);

        public static void Set(this IAttribute attr, ItemStack value, params string[] keys)
            => Set(attr, (value != null) ? new ItemstackAttribute(value) : null, keys);

        public static IAttribute? LookupValue(this IAttribute? root, string dotNotation)
        {
            if (root == null || string.IsNullOrWhiteSpace(dotNotation)) return null;
            var keys = dotNotation.Split('.');
            IAttribute? current = root;
            foreach (var key in keys)
            {
                if (current is TreeAttribute tree)
                {
                    if (tree.HasAttribute(key))
                    {
                        current = tree[key];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            return current;
        }

        public static bool EvaluateDotNotationLogic(this ITreeAttribute config, ICoreAPI api, string logicString)
        {
            if (string.IsNullOrWhiteSpace(logicString)) return true;

            bool isAnd = logicString.Contains("&");
            bool isOr = logicString.Contains("|");

            string[] keys;
            if (isAnd && !isOr)
                keys = logicString.Split('&');
            else if (isOr && !isAnd)
                keys = logicString.Split('|');
            else if (isAnd && isOr)
                keys = logicString.Split('|');
            else
                keys = new[] { logicString };

            if (isAnd && isOr)
            {
                foreach (var orGroup in keys)
                {
                    var andKeys = orGroup.Split('&');
                    bool andResult = true;
                    foreach (var key in andKeys)
                    {
                        var attr = config.LookupValue(key.Trim());
                        if (attr is BoolAttribute boolAttr)
                            andResult &= boolAttr.value;
                        else
                        {
                            api.Logger.Warning($"CarryOn: EvaluateDotNotationLogic - Key '{key.Trim()}' not found or not boolean, assuming true.");
                            andResult &= true;
                        }
                    }
                    if (andResult) return true;
                }
                return false;
            }

            bool result = isOr ? false : true;
            foreach (var key in keys)
            {
                bool value;
                var attr = config.LookupValue(key.Trim());
                if (attr is BoolAttribute boolAttr)
                    value = boolAttr.value;
                else
                {
                    api.Logger.Warning($"CarryOn: EvaluateDotNotationLogic - Key '{key.Trim()}' not found or not boolean, assuming true.");
                    value = true;
                }

                if (isAnd) result &= value;
                else if (isOr) result |= value;
                else result = value;
            }
            return result;
        }
    }
}
