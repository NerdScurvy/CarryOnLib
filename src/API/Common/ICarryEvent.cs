using Vintagestory.API.Common; // Add this if IModSystem is defined in Vintagestory.API.Common

namespace CarryOn.API.Common
{
    public interface ICarryEvent
    {
      // TODO: Change to send API instead of ModSystem when API implements way to get CarryEvents
      void Init(ModSystem modSystem);
    }
}