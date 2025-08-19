using Vintagestory.API.Common;

namespace CarryOn.API.Common
{

    public interface ICarryManager
    {
        CarriedBlock GetCarriedBlock(IPlayer player,CarrySlot slot);
    }
}