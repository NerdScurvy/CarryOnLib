using CarryOn.API.Common.Interfaces;
using CarryOn.API.Common.Models;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using static CarryOn.API.Common.Models.CarryConstants;

namespace CarryOn.Utility
{
    /// <summary>
    /// Convenience extension methods for <see cref="ICarryManager"/> that suppress failure details.
    /// Use the <see cref="ICarryManager"/> methods directly when error reporting is needed.
    /// </summary>
    public static class CarryManagerExtensions
    {
        /// <summary>
        /// Convenience overload that suppresses failure details.
        /// Use <see cref="ICarryManager.TryPickUp"/> directly when error reporting is needed.
        /// </summary>
        public static bool TryPickUp(this ICarryManager mgr, Entity entity, BlockPos pos, CarrySlot slot, bool checkIsCarryable = true, bool playSound = true)
        {
            var failureCode = FailureCodes.Ignore;
            return mgr.TryPickUp(entity, pos, slot, ref failureCode, checkIsCarryable, playSound);
        }

        /// <summary>
        /// Convenience overload that suppresses failure details.
        /// Use <see cref="ICarryManager.TryPlaceDown"/> directly when error reporting is needed.
        /// </summary>
        public static bool TryPlaceDown(this ICarryManager mgr, Entity entity, CarriedBlock carriedBlock, BlockSelection selection, bool dropped = false, bool playSound = true)
        {
            var failureCode = FailureCodes.Ignore;
            return mgr.TryPlaceDown(entity, carriedBlock, selection, ref failureCode, dropped, playSound);
        }

        /// <summary>
        /// Convenience overload that suppresses failure details.
        /// Use <see cref="ICarryManager.TryPlaceDownAt"/> directly when error reporting is needed.
        /// </summary>
        public static bool TryPlaceDownAt(this ICarryManager mgr, IPlayer player, CarrySlot slot, BlockSelection selection, out BlockPos? placedAt)
        {
            var failureCode = FailureCodes.Ignore;
            return mgr.TryPlaceDownAt(player, slot, selection, out placedAt, ref failureCode);
        }

        /// <summary>
        /// Convenience overload that suppresses failure details.
        /// Use <see cref="ICarryManager.TryAttach"/> directly when error reporting is needed.
        /// </summary>
        public static bool TryAttach(this ICarryManager mgr, IServerPlayer player, long targetEntityId, int slotIndex, bool playSound = true)
        {
            var failureCode = FailureCodes.Ignore;
            return mgr.TryAttach(player, targetEntityId, slotIndex, ref failureCode, playSound);
        }

        /// <summary>
        /// Convenience overload that suppresses failure details.
        /// Use <see cref="ICarryManager.TryDetach"/> directly when error reporting is needed.
        /// </summary>
        public static bool TryDetach(this ICarryManager mgr, IServerPlayer player, long targetEntityId, int slotIndex, bool playSound = true)
        {
            var failureCode = FailureCodes.Ignore;
            return mgr.TryDetach(player, targetEntityId, slotIndex, ref failureCode, playSound);
        }
    }
}
