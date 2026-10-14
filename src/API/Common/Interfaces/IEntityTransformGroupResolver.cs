using CarryOn.API.Common.Models;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace CarryOn.API.Common.Interfaces
{
    /// <summary>
    /// Provides a per-entity override for the base transform group of a block carried
    /// on a wearing entity during rendering (for example, downgrading the back-slot
    /// group to the no-backpack group when a cosmetic mod hides the wearer's backpack).
    /// Register implementations via <see cref="ICarryManager.RegisterEntityTransformGroupResolver"/>.
    /// </summary>
    public interface IEntityTransformGroupResolver
    {
        /// <summary>Unique identifier for this resolver, used for registration and lookup.</summary>
        string ResolverCode { get; }

        /// <summary>
        /// Attempts to override the wearing entity's base transform group. Returns true with a
        /// non-empty <paramref name="group"/> if the group was overridden; false (or an empty
        /// group) to keep <paramref name="baseGroup"/>.
        /// </summary>
        /// <param name="api">The core API instance.</param>
        /// <param name="entity">The wearing entity being rendered.</param>
        /// <param name="carrySlot">The carry slot being rendered.</param>
        /// <param name="baseGroup">The base transform group determined by the normal carry logic.</param>
        /// <param name="group">The override group, or null to keep the base group.</param>
        bool TryResolve(ICoreAPI api, EntityAgent entity, CarrySlot carrySlot, string baseGroup, out string? group);
    }
}
