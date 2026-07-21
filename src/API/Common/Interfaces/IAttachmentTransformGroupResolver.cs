using CarryOn.API.Common.Models;
using Vintagestory.API.Common;

namespace CarryOn.API.Common.Interfaces
{
    /// <summary>
    /// Provides dynamic transform group candidates for attached/cluster child blocks during rendering.
    /// Register implementations via <see cref="ICarryManager.RegisterAttachmentTransformGroupResolver"/>.
    /// </summary>
    public interface IAttachmentTransformGroupResolver
    {
        /// <summary>Unique identifier for this resolver, used for registration and lookup.</summary>
        string ResolverCode { get; }

        /// <summary>
        /// Attempts to resolve attachment transform group candidates for the given carried block.
        /// Returns true if custom candidates were resolved; false to fall back to the default.
        /// </summary>
        /// <param name="api">The core API instance.</param>
        /// <param name="carried">The primary carried block whose attachments are being resolved.</param>
        /// <param name="baseGroup">The default transform group determined by the carry system.</param>
        /// <param name="result">The resolved attachment result, or null to use the default.</param>
        bool TryResolve(ICoreAPI api, CarriedBlock carried, string baseGroup, out AttachmentResolveResult? result);

        /// <summary>
        /// Returns an optional cache signature string. When non-null and equal to a previous call,
        /// the resolver's output can be cached. Return null to disable caching.
        /// </summary>
        /// <param name="api">The core API instance.</param>
        /// <param name="carried">The primary carried block whose attachments are being resolved.</param>
        /// <param name="baseGroup">The default transform group determined by the carry system.</param>
        string? GetCacheSignature(ICoreAPI api, CarriedBlock carried, string baseGroup) => null;
    }
}
