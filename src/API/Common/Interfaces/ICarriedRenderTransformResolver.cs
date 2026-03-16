using CarryOn.API.Common.Models;
using Vintagestory.API.Common;

namespace CarryOn.API.Common.Interfaces
{
    public interface ICarriedRenderTransformResolver
    {
        int Priority { get; }

        bool TryResolve(ICoreAPI api, CarriedBlock carried, string baseGroup, out CarriedRenderResolution resolution);
    }
}
