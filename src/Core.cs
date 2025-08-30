using CarryOn.API.Common;
using CarryOn.API.Common.Interfaces;
using CarryOn.Utility;
using Vintagestory.API.Common;

namespace CarryOn.CarryOnLib
{
    public class Core : ModSystem
    {
        public ICarryManager CarryManager { get; set; }

        public override void Dispose()
        {
            Extensions.ClearCachedCarryManager();
            base.Dispose();
        }
    }
}