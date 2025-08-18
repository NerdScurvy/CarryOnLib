using Vintagestory.API.Common; // Add this if IModSystem is defined in Vintagestory.API.Common

namespace CarryOn.API.Common
{
    public interface ICarryEvent
    {
      void Init(ICoreAPI api);
    }
}