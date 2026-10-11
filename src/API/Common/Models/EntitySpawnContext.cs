using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace CarryOn.API.Common.Models
{
    /// <summary>
    /// Provides full context for spawning an entity from a carried block.
    /// Populated by the carry placement/drop pipeline and passed to
    /// <see cref="Interfaces.ICarryEntitySpawnerFactory.SpawnFromCarriedBlock"/>.
    /// </summary>
    public class EntitySpawnContext
    {
        /// <summary>
        /// The carried block being spawned as an entity.
        /// </summary>
        public required CarriedBlock CarriedBlock { get; init; }

        /// <summary>
        /// The UID of the player who placed/dropped this block, or "unknown".
        /// </summary>
        public required string OwnerUid { get; init; }

        /// <summary>
        /// The precise world position for spawning. Sub-block floating-point precision.
        /// For placement: derived from block position + hit position offset.
        /// For drop: derived from the player entity's world position.
        /// </summary>
        public required Vec3d Position { get; init; }

        /// <summary>
        /// The block position of the targeted cell (integer coords).
        /// The <see cref="Position"/> is within or adjacent to this cell.
        /// </summary>
        public required BlockPos BlockPosition { get; init; }

        /// <summary>
        /// The exact hit point within the targeted block, in 0-1 normalized coordinates.
        /// Null when the spawn originates from a drop (no block target).
        /// </summary>
        public Vec3d? HitPosition { get; init; }

        /// <summary>
        /// The face of the block that was hit. Null when from a drop.
        /// </summary>
        public BlockFacing? Face { get; init; }

        /// <summary>
        /// Whether this spawn is the result of dropping (disconnect, death, shift-rightclick drop)
        /// versus deliberate placement (right-click with carry in hand).
        /// </summary>
        public required bool Dropped { get; init; }

        /// <summary>
        /// The API instance. Both client and server set this.
        /// The pipeline checks for <see cref="Vintagestory.API.Server.ICoreServerAPI"/>
        /// before calling <see cref="Interfaces.ICarryEntitySpawnerFactory.SpawnFromCarriedBlock"/>.
        /// </summary>
        public required ICoreAPI Api { get; init; }
    }
}
