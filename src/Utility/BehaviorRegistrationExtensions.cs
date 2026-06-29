using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace CarryOn.Utility
{
    public static class BehaviorRegistrationExtensions
    {
        public static void Register<T>(this ICoreAPI api)
        {
            var name = typeof(T).GetProperty("Name")?.GetValue(null) as string ?? throw new ArgumentException($"Type {typeof(T).Name} does not have a static 'Name' property.");
            if (typeof(BlockBehavior).IsAssignableFrom(typeof(T)))
                api.RegisterBlockBehaviorClass(name, typeof(T));
            else if (typeof(EntityBehavior).IsAssignableFrom(typeof(T)))
                api.RegisterEntityBehaviorClass(name, typeof(T));
            else throw new ArgumentException("T is not a block or entity behavior", nameof(T));
        }

        public static bool HasBehavior<T>(this Block block)
            where T : BlockBehavior
                => block.HasBehavior(typeof(T));

        public static T GetBehaviorOrDefault<T>(this Block block, T @default)
            where T : BlockBehavior
                => block.GetBehavior<T>() ?? @default;
    }
}
