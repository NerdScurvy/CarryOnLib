using CarryOn.API.Common.Interfaces;
using Vintagestory.API.Common;

namespace CarryOn.CarryOnLib
{
    public class CarryOnLibSystem : ModSystem
    {
        public ICarryManager? CarryManager { get; set; }
    }
}