using CarryOn.API.Event;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;

namespace CarryOn.API.Common
{

    public interface ICarryManager
    {
        ICoreAPI Api { get; }

        CarryEvents CarryEvents { get; }

        CarriedBlock GetCarriedBlock(Entity entity, CarrySlot slot);

        void SetCarriedBlock(Entity entity, CarriedBlock carriedBlock);

        void SetCarriedBlock(Entity entity, CarrySlot slot, ItemStack itemStack, ITreeAttribute blockEntityData);

        void RemoveCarriedBlock(Entity entity, CarrySlot slot);

        bool TryPlaceDown(Entity entity, CarriedBlock carriedBlock, BlockSelection selection, bool dropped = false, bool playSound = true);

    }
}