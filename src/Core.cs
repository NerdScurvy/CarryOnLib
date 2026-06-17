using CarryOn.API.Common.Interfaces;
using Vintagestory.API.Common;

namespace CarryOn.CarryOnLib
{
    public class Core : ModSystem
    {
        public ICarryManager? CarryManager { get; set; }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}