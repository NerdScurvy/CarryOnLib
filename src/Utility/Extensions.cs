using System;
using System.Collections.Generic;
using System.Linq;
using CarryOn.API.Common;
using CarryOn.API.Event;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace CarryOn.Utility
{
    public static class Extensions
    {

        private static ICarryManager clientCarryManager = null;
        private static ICarryManager serverCarryManager = null;

        /// <summary>
        /// Clears the cached carry managers.
        /// </summary>
        public static void ClearCachedCarryManager()
        {
            clientCarryManager = null;
            serverCarryManager = null;
        }

        /// <summary>
        /// Returns the <see cref="ICarryManager"/> for the specified API.
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        public static ICarryManager GetCarryManager(ICoreAPI api)
        {
            if (api.Side == EnumAppSide.Server)
            {
                serverCarryManager ??= api.ModLoader.GetModSystem<CarryOnLib.Core>()?.CarryManager;
                return serverCarryManager;
            }
            clientCarryManager ??= api.ModLoader.GetModSystem<CarryOnLib.Core>()?.CarryManager;
            return clientCarryManager;
        }

        /* ------------------------------ */
        /* Entity extensions              */
        /* ------------------------------ */

        /// <summary>
        /// Returns whether the specified entity has permission to carry the block at the given position.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool HasPermissionToCarry(this Entity entity, BlockPos pos)
            => GetCarryManager(entity.Api)?.HasPermissionToCarry(entity, pos) ?? false;

        /// <summary>
        /// Returns the <see cref="CarriedBlock"/> this entity is carrying in the specified slot, or null of none.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        public static CarriedBlock GetCarried(this Entity entity, CarrySlot slot)
            => GetCarryManager(entity.Api)?.GetCarried(entity, slot);

        /// <summary>
        /// Returns all the <see cref="CarriedBlock"/>s this entity is carrying.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static IEnumerable<CarriedBlock> GetCarried(this Entity entity)
            => GetCarryManager(entity.Api)?.GetAllCarried(entity);

        /// <summary>
        /// Attempts to make this entity drop all of its carried blocks around its current position in the specified area.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="range"></param>
        public static void DropAllCarried(this Entity entity, int range = 4)
            => GetCarryManager(entity.Api)?.DropCarried(entity, Enum.GetValues(typeof(CarrySlot)).Cast<CarrySlot>(), range);

        /// <summary>
        /// Attempts to swap the <see cref="CarriedBlock"/>s currently carried in the
        /// entity's <paramref name="first"/> and <paramref name="second"/> slots.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool SwapCarried(this Entity entity, CarrySlot first, CarrySlot second)
            => GetCarryManager(entity.Api).SwapCarried(entity, first, second);

        /* ------------------------------ */
        /* CarriedBlock Extensions        */
        /* ------------------------------ */

        /// <summary>
        /// Sets the carried block for the entity in the specified carry slot.
        /// </summary>
        /// <param name="carriedBlock"></param>
        /// <param name="entity"></param>
        /// <param name="slot"></param>
        public static void Set(this CarriedBlock carriedBlock, Entity entity, CarrySlot slot)
            => GetCarryManager(entity.Api).SetCarried(entity, slot, carriedBlock.ItemStack, carriedBlock.BlockEntityData);

        /* ------------------------------ */

        /// <summary>
        /// Registers a block or entity behavior class with the API.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="api"></param>
        public static void Register<T>(this ICoreAPI api)
        {
            var name = (string)typeof(T).GetProperty("Name").GetValue(null);
            if (typeof(BlockBehavior).IsAssignableFrom(typeof(T)))
                api.RegisterBlockBehaviorClass(name, typeof(T));
            else if (typeof(EntityBehavior).IsAssignableFrom(typeof(T)))
                api.RegisterEntityBehaviorClass(name, typeof(T));
            else throw new ArgumentException("T is not a block or entity behavior", nameof(T));
        }


        /// <summary>
        /// Checks if the block has a behavior of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="block"></param>
        /// <returns></returns>
        public static bool HasBehavior<T>(this Block block)
            where T : BlockBehavior
                => block.HasBehavior(typeof(T));

        /// <summary>
        /// Gets the behavior of the specified type, or the default value if it doesn't exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="block"></param>
        /// <param name="default"></param>
        /// <returns></returns>
        public static T GetBehaviorOrDefault<T>(this Block block, T @default)
            where T : BlockBehavior
                => (T)block.GetBehavior<T>() ?? @default;


        /// <summary>
        /// Tries to get an attribute from the specified keys.
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static IAttribute TryGet(this IAttribute attr, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (attr is not ITreeAttribute tree) return null;
                attr = tree[key];
            }
            return attr;
        }

        /// <summary>
        /// Tries to get an attribute of the specified type from the specified keys.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attr"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static T TryGet<T>(this IAttribute attr, params string[] keys)
                where T : class, IAttribute
            => TryGet(attr, keys) as T;

        /// <summary>
        /// Sets an attribute at the specified keys, creating tree nodes as necessary.
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="value"></param>
        /// <param name="keys"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void Set(this IAttribute attr, IAttribute value, params string[] keys)
        {
            if (attr == null) throw new ArgumentNullException(nameof(attr));
            for (var i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                if (attr is not ITreeAttribute tree)
                {
                    if ((attr == null) && (value == null)) return; // If removing value, return on missing tree nodes.
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

        /// <summary>
        /// Removes an attribute at the specified keys.
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="keys"></param>
        public static void Remove(this IAttribute attr, params string[] keys)
            => Set(attr, (IAttribute)null, keys);

        /// <summary>
        /// Sets an attribute at the specified keys, creating tree nodes as necessary.
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="value"></param>
        /// <param name="keys"></param>
        public static void Set(this IAttribute attr, ItemStack value, params string[] keys)
            => Set(attr, (value != null) ? new ItemstackAttribute(value) : null, keys);

        /// <summary>
        /// Tries to get a string value from the dictionary of JTokens.
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool TryGetBool(this Dictionary<string, JToken> dict, string key, bool defaultValue)
            => dict.TryGetValue(key, out var token) && token.Type == JTokenType.Boolean ? token.Value<bool>() : defaultValue;

        /// <summary>
        /// Tries to get a float value from the dictionary of JTokens.
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float TryGetFloat(this Dictionary<string, JToken> dict, string key, float defaultValue)
            => dict.TryGetValue(key, out var token) && token.Type == JTokenType.Float ? token.Value<float>() :
               dict.TryGetValue(key, out token) && token.Type == JTokenType.Integer ? token.Value<int>() : defaultValue;

        /// <summary>
        /// Tries to get an array of strings from the dictionary of JTokens.
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string[] TryGetStringArray(this Dictionary<string, JToken> dict, string key, string[] defaultValue)
            => dict.TryGetValue(key, out var token) && token.Type == JTokenType.Array ? token.ToObject<string[]>() : defaultValue;


        /// <summary>
        /// Helper to lookup a value in an IAttribute using dot notation (e.g. "carryon.Carryables.Anvil").
        /// Returns the final IAttribute found, or null if not found.
        /// </summary>
        public static IAttribute LookupValue(this IAttribute root, string dotNotation)
        {
            if (root == null || string.IsNullOrWhiteSpace(dotNotation)) return null;
            var keys = dotNotation.Split('.');
            IAttribute current = root;
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

        /// <summary>
        /// Evaluates a logic string of dot-notation config keys separated by & (AND) or | (OR).
        /// Uses LookupConfigValue to get each value, assumes true if not found or not boolean.
        /// Example: "carryon.Carryables.Clutter&carryon.Carryables.BookCase" (AND)
        ///          "carryon.Carryables.Clutter|carryon.Carryables.BookCase" (OR)
        /// </summary>
        public static bool EvaluateDotNotationLogic(this ITreeAttribute config, ICoreAPI api, string logicString)
        {
            if (string.IsNullOrWhiteSpace(logicString)) return true;

            // Determine logic type
            bool isAnd = logicString.Contains("&");
            bool isOr = logicString.Contains("|");

            // Split by delimiter
            string[] keys;
            if (isAnd && !isOr)
                keys = logicString.Split('&');
            else if (isOr && !isAnd)
                keys = logicString.Split('|');
            else if (isAnd && isOr)
                // Mixed logic, treat & as higher precedence than |
                keys = logicString.Split('|');
            else
                keys = new[] { logicString };

            // If mixed logic, evaluate each OR group as AND
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

            // Pure AND or OR
            bool result = isOr ? false : true;
            foreach (var key in keys)
            {
                bool value;
                var attr = config.LookupValue(key.Trim());
                if (attr is BoolAttribute boolAttr) // Assume true if not found or not boolean
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
