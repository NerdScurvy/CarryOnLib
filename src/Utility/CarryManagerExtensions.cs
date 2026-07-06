using CarryOn.API.Common.Interfaces;
using CarryOn.API.Common.Models;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using static CarryOn.API.Common.Models.CarryConstant;

namespace CarryOn.Utility
{
    public static class CarryManagerExtensions
    {
        public static bool TryPickUp(this ICarryManager mgr, Entity entity, BlockPos pos, CarrySlot slot, bool checkIsCarryable = true, bool playSound = true)
        {
            var failureCode = FailureCode.Ignore;
            return mgr.TryPickUp(entity, pos, slot, ref failureCode, checkIsCarryable, playSound);
        }

        public static bool TryPlaceDown(this ICarryManager mgr, Entity entity, CarriedBlock carriedBlock, BlockSelection selection, bool dropped = false, bool playSound = true)
        {
            var failureCode = FailureCode.Ignore;
            return mgr.TryPlaceDown(entity, carriedBlock, selection, ref failureCode, dropped, playSound);
        }

        public static bool TryPlaceDownAt(this ICarryManager mgr, IPlayer player, CarrySlot slot, BlockSelection selection, out BlockPos? placedAt)
        {
            var failureCode = FailureCode.Ignore;
            return mgr.TryPlaceDownAt(player, slot, selection, out placedAt, ref failureCode);
        }

        public static bool TryAttach(this ICarryManager mgr, IServerPlayer player, long targetEntityId, int slotIndex, bool playSound = true)
        {
            var failureCode = FailureCode.Ignore;
            return mgr.TryAttach(player, targetEntityId, slotIndex, ref failureCode, playSound);
        }

        public static bool TryDetach(this ICarryManager mgr, IServerPlayer player, long targetEntityId, int slotIndex, bool playSound = true)
        {
            var failureCode = FailureCode.Ignore;
            return mgr.TryDetach(player, targetEntityId, slotIndex, ref failureCode, playSound);
        }
    }
}
