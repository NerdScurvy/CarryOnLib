using CarryOn.API.Common;
using Vintagestory.API.Common;

namespace CarryOn.CarryOnLib
{
    public class Core : ModSystem
    {
        public ICarryManager CarryManager { get; set; }

    }
}