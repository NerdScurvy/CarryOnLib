using System.Collections.Generic;
using CarryOn.API.Common.Models;
using Vintagestory.API.Common;

namespace CarryOn.API.Common.Interfaces
{
    public interface IRootTransformGroupResolver
    {
        string ResolverCode { get; }

        bool TryResolve(ICoreAPI api, CarriedBlock carried, string baseGroup, out IList<string>? candidates);

        string? GetCacheSignature(ICoreAPI api, CarriedBlock carried, string baseGroup) => null;
    }
}
