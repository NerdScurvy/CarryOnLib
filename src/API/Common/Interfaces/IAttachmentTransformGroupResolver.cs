using CarryOn.API.Common.Models;
using Vintagestory.API.Common;

namespace CarryOn.API.Common.Interfaces
{
    public interface IAttachmentTransformGroupResolver
    {
        string ResolverCode { get; }

        bool TryResolve(ICoreAPI api, CarriedBlock carried, string baseGroup, out AttachmentResolveResult? result);

        string? GetCacheSignature(ICoreAPI api, CarriedBlock carried, string baseGroup) => null;
    }
}
