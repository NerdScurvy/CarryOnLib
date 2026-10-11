using System;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace CarryOn.Utility
{
    /// <summary>
    /// Optional gravity drop-position resolver for entities spawned from
    /// dropped carried blocks. Scans downward from the candidate position
    /// to find the first surface (solid block top, walkable partial block,
    /// or liquid surface) with clearance for the given footprint. Modders
    /// who want full manual control simply never call this helper.
    /// </summary>
    public static class GravitySpawnHelper
    {
        /// <summary>
        /// Default liquid submerge offset: entity feet at blockY + 0.5
        /// (entity from blockY+0.5 to blockY+1.0 = top at liquid surface).
        /// </summary>
        public const float DefaultLiquidSubmergeOffset = 0.5f;

        /// <summary>
        /// Resolves a drop position with gravity: scans down the candidate's
        /// column for the first surface (solid block top, walkable partial
        /// block, or liquid surface) with clearance for the given footprint.
        /// Returns <paramref name="fallbackPos"/> ?? <paramref name="candidatePos"/>
        /// when no floor is found within <paramref name="maxDepth"/> cells.
        /// Purely optional - callers that want raw control ignore this helper.
        /// </summary>
        /// <param name="world">The world accessor (for collision testing).</param>
        /// <param name="blockAccessor">The block accessor for terrain queries.</param>
        /// <param name="candidatePos">The starting position (typically player feet).</param>
        /// <param name="testBoxRel">Position-relative collision box (bottom-center
        /// origin, e.g. <see cref="EntityDimensions.ToCollisionBox"/>).</param>
        /// <param name="fallbackPos">Position returned when no floor is found.
        /// Defaults to <paramref name="candidatePos"/>.</param>
        /// <param name="maxDepth">Maximum number of cells to scan downward.
        /// Defaults to <see cref="int.MaxValue"/> (scan to world bottom).</param>
        /// <param name="liquidSubmergeOffset">Y offset within a liquid cell for
        /// the entity's feet when floating. Defaults to
        /// <see cref="DefaultLiquidSubmergeOffset"/> (0.5 - feet at mid-cell,
        /// top of entity at liquid surface).</param>
        public static Vec3d ResolveDropPosition(
            IWorldAccessor world,
            IBlockAccessor blockAccessor,
            Vec3d candidatePos,
            Cuboidf testBoxRel,
            Vec3d? fallbackPos = null,
            int maxDepth = int.MaxValue,
            float liquidSubmergeOffset = DefaultLiquidSubmergeOffset)
        {
            var startPos = candidatePos.AsBlockPos;

            // If starting in a liquid, spawn at the candidate position directly
            if (blockAccessor.GetBlock(startPos).IsLiquid())
                return candidatePos;

            var x = startPos.X;
            var z = startPos.Z;
            var minY = Math.Max(0, startPos.Y - maxDepth);

            for (int y = startPos.Y; y >= minY; y--)
            {
                var pos = new BlockPos(x, y, z);
                var block = blockAccessor.GetBlock(pos);

                if (block.IsLiquid())
                {
                    var targetPos = new Vec3d(x + 0.5, y + liquidSubmergeOffset, z + 0.5);
                    if (BlockCollisionUtils.HasClearance(world, blockAccessor, testBoxRel, targetPos))
                        return targetPos;

                    continue;
                }

                var surfaceTop = BlockCollisionUtils.GetSurfaceTop(block);
                var hasCollisionBoxes = block.CollisionBoxes is { Length: > 0 };

                // If the block has no collision boxes but the player's feet
                // are inside it, the player's offset within the block gives
                // the true surface height (handles partial blocks like paths).
                if (!hasCollisionBoxes && candidatePos.Y > y && candidatePos.Y < y + surfaceTop)
                    surfaceTop = Math.Max(0f, Math.Min((float)(candidatePos.Y - y), surfaceTop));

                if (!BlockCollisionUtils.IsPassable(block))
                {
                    var targetPos = new Vec3d(x + 0.5, y + surfaceTop, z + 0.5);
                    if (BlockCollisionUtils.HasClearance(world, blockAccessor, testBoxRel, targetPos))
                        return targetPos;
                }

                // Passable block with a surface the entity can stand on
                // (e.g. wooden paths - passable but walkable)
                if (surfaceTop > 0f && block.Id != 0 && candidatePos.Y >= y + surfaceTop)
                {
                    var targetPos = new Vec3d(x + 0.5, y + surfaceTop, z + 0.5);
                    if (BlockCollisionUtils.HasClearance(world, blockAccessor, testBoxRel, targetPos))
                        return targetPos;
                }
            }

            return fallbackPos ?? candidatePos;
        }
    }
}
