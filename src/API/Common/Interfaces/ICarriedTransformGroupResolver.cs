using CarryOn.API.Common.Models;
using Vintagestory.API.Common;

namespace CarryOn.API.Common.Interfaces
{
    public interface ICarriedTransformGroupResolver
    {
        string ResolverCode { get; }

        bool TryResolve(ICoreAPI api, CarriedBlock carried, string baseGroup, out CarriedGroupResolution? resolution);

        /// <summary>
        /// Returns an additional cache signature fragment for this resolver.
        /// Return null when the resolved transform output does not depend on any
        /// carried block state beyond the existing base cache inputs.
        /// </summary>
        /// <param name="api">The core API instance.</param>
        /// <param name="carried">The carried block.</param>
        /// <param name="baseGroup">The base transform group.</param>
        /// <param name="resolution">The resolved group resolution.</param>
        string? GetCacheSignature(ICoreAPI api, CarriedBlock carried, string baseGroup, CarriedGroupResolution? resolution)
        {
            return null;
        }        
    }
}
