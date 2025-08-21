using CarryOn.API.Event;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace CarryOn.API.Common
{

    public interface ICarryManager
    {
        ICoreAPI Api { get; }

        CarryEvents CarryEvents { get; }

        CarriedBlock GetCarried(Entity entity, CarrySlot slot);

        void SetCarried(Entity entity, CarriedBlock carriedBlock);

        void SetCarried(Entity entity, CarrySlot slot, ItemStack itemStack, ITreeAttribute blockEntityData);

        void RemoveCarried(Entity entity, CarrySlot slot);

        bool TryPlaceDown(Entity entity, CarriedBlock carriedBlock, BlockSelection selection, bool dropped = false, bool playSound = true);

        bool TryPickUp(Entity entity, BlockPos pos, CarrySlot slot, bool checkIsCarryable = true, bool playSound = true);

    }
}