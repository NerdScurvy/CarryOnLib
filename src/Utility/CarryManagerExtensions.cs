using CarryOn.API.Common.Interfaces;
using CarryOn.API.Common.Models;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace CarryOn.Utility
{
    /// <summary>
    /// Convenience overloads for <see cref="ICarryManager"/> methods that require a
    /// <c>ref string failureCode</c> parameter. These extensions pass
    /// <see cref="CarryCode.FailureCode.Ignore"/> automatically.
    /// </summary>
    public static class CarryManagerExtensions
    {
        /// <summary>Tries to pick up a block without tracking failure details.</summary>
        public static bool TryPickUp(this ICarryManager mgr, Entity entity, BlockPos pos, CarrySlot slot, bool checkIsCarryable = true, bool playSound = true)
        {
            var failureCode = CarryCode.FailureCode.Ignore;
            return mgr.TryPickUp(entity, pos, slot, ref failureCode, checkIsCarryable, playSound);
        }

        /// <summary>Tries to place a carried block without tracking failure details.</summary>
        public static bool TryPlaceDown(this ICarryManager mgr, Entity entity, CarriedBlock carriedBlock, BlockSelection selection, bool dropped = false, bool playSound = true)
        {
            var failureCode = CarryCode.FailureCode.Ignore;
            return mgr.TryPlaceDown(entity, carriedBlock, selection, ref failureCode, dropped, playSound);
        }

        /// <summary>Tries to place a carried block at a target position without tracking failure details.</summary>
        public static bool TryPlaceDownAt(this ICarryManager mgr, IPlayer player, CarrySlot slot, BlockSelection selection, out BlockPos? placedAt)
        {
            var failureCode = CarryCode.FailureCode.Ignore;
            return mgr.TryPlaceDownAt(player, slot, selection, out placedAt, ref failureCode);
        }

        /// <summary>Tries to attach to an entity slot without tracking failure details.</summary>
        public static bool TryAttach(this ICarryManager mgr, IServerPlayer player, long targetEntityId, int slotIndex, bool playSound = true)
        {
            var failureCode = CarryCode.FailureCode.Ignore;
            return mgr.TryAttach(player, targetEntityId, slotIndex, ref failureCode, playSound);
        }

        /// <summary>Tries to detach from an entity slot without tracking failure details.</summary>
        public static bool TryDetach(this ICarryManager mgr, IServerPlayer player, long targetEntityId, int slotIndex, bool playSound = true)
        {
            var failureCode = CarryCode.FailureCode.Ignore;
            return mgr.TryDetach(player, targetEntityId, slotIndex, ref failureCode, playSound);
        }
    }
}