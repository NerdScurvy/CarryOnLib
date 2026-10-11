using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace CarryOn.Utility
{
    /// <summary>
    /// Shared block collision / space-check utilities used by both spawn
    /// placement and gravity drop resolution: surface height lookup,
    /// entity-space terrain clearance and movement passability.
    /// Placement-specific validation (door/trapdoor/multiblock awareness)
    /// is deliberately out of scope here.
    /// </summary>
    public static class BlockCollisionUtils
    {
        /// <summary>
        /// Returns the top surface Y (block-local 0-1 range) for the given
        /// block's collision shape. Falls back to selection boxes, then to 1.0
        /// (full block height) if the block has no boxes.
        /// </summary>
        public static float GetSurfaceTop(Block block)
        {
            var boxes = block.CollisionBoxes;
            if (boxes == null || boxes.Length == 0)
                boxes = block.SelectionBoxes;
            if (boxes == null || boxes.Length == 0)
                return 1.0f;

            float maxY = 0f;
            foreach (var box in boxes)
            {
                if (box.Y2 > maxY)
                    maxY = box.Y2;
            }
            return maxY;
        }

        /// <summary>
        /// Whether an entity whose position-relative collision box is
        /// <paramref name="entityBoxRel"/> placed at <paramref name="pos"/>
        /// fits without intersecting terrain. Uses the engine's
        /// <see cref="CollisionTester"/> with touch checking disabled, so
        /// resting exactly on a supporting surface is not a collision -
        /// no exclusion of the support block is needed. Callers construct
        /// the box however they need (e.g. bottom-center origin via
        /// <c>new Cuboidf(-half, 0, -half, half, height, half)</c>).
        /// </summary>
        public static bool HasClearance(
            IWorldAccessor world,
            IBlockAccessor blockAccessor,
            Cuboidf entityBoxRel,
            Vec3d pos)
        {
            return world.CollisionTester.GetCollidingCollisionBox(blockAccessor, entityBoxRel, pos, false) == null;
        }

        /// <summary>
        /// Whether a block is passable for entity movement - entities can
        /// fall through it (air, replaceable, gas). For gravity-landing
        /// logic; not for placement validation, which needs door/trapdoor
        /// and multiblock awareness.
        /// </summary>
        public static bool IsPassable(Block block)
        {
            if (block.Id == 0) return true;
            if (block.Replaceable >= 6000) return true;
            if (block.MatterState == EnumMatterState.Gas) return true;

            return false;
        }
    }
}
