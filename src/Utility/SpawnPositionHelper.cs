using System;
using CarryOn.API.Common.Models;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace CarryOn.Utility
{
    /// <summary>
    /// Face-aware and collision-aware spawn position resolution for entities
    /// spawned from carried blocks. Entity origins are assumed to be at
    /// bottom-center (feet). Pass 1 produces a position that touches the
    /// targeted surface exactly without colliding; pass 2 depenetrates -
    /// whenever the entity's collision box intersects terrain (or an entity),
    /// it is pushed out along the smallest escape axis of the intersecting
    /// box, repeating until free space is found. All checks use the game's
    /// own <see cref="CollisionTester"/> so placement agrees with physics.
    /// </summary>
    public static class SpawnPositionHelper
    {
        /// <summary>
        /// Default upper bound for depenetration iterations. Each iteration
        /// fully escapes one intersecting box; corners can need a handful of
        /// rounds. Callers can override via <see cref="GetAdjustedSpawnPosition"/>.
        /// </summary>
        public const int DefaultDepenetrationSteps = 8;

        /// <summary>
        /// Hit coordinates within this distance of a face plane are treated
        /// as exactly on the plane - ray picking carries float jitter that
        /// would otherwise leave hair-thin overlaps.
        /// </summary>
        private const double PlaneSnap = 1e-3;

        /// <summary>
        /// Pass 1: adjusts the raw hit position based on which block face was
        /// targeted and the entity's dimensions. The result touches the
        /// targeted surface exactly (feet on top faces, head against ceilings,
        /// back against walls) without colliding with it.
        /// Returns <see cref="EntitySpawnContext.Position"/> unchanged for drops
        /// (no face) or when a placement constraint rejects the targeted face.
        /// </summary>
        public static Vec3d GetSurfacePosition(
            EntitySpawnContext context,
            IBlockAccessor blockAccessor,
            EntityDimensions dimensions,
            PlacementConstraint constraints = PlacementConstraint.None)
        {
            var face = context.Face;
            if (face == null) return context.Position;
            if (!IsFaceAllowed(face, constraints)) return context.Position;

            return ApplyFacePosition(context, blockAccessor, face, dimensions);
        }

        /// <summary>
        /// Pass 2: applies <see cref="GetSurfacePosition"/>, then resolves
        /// collisions by depenetration. While the entity's collision box
        /// intersects terrain (or optionally an existing entity), the entity
        /// moves away from the intersecting box along its smallest escape
        /// axis; the check repeats until free space is found. Because
        /// escapes are exactly as large as the penetration, the entity ends
        /// up flush against surrounding blocks instead of being nudged a
        /// fixed step away.
        /// <para>
        /// When <paramref name="resolutionMode"/> is
        /// <see cref="SpawnResolutionMode.BestEffort"/> (default), the last
        /// position is returned even if still partially intersecting after
        /// <paramref name="maxDepenetrationSteps"/> rounds. When it is
        /// <see cref="SpawnResolutionMode.RequireClearSpace"/>, null is
        /// returned so the caller can abort spawning and report insufficient
        /// space.
        /// </para>
        /// Drops (null face) are returned unchanged.
        /// </summary>
        /// <returns>
        /// The resolved spawn position, or null when
        /// <see cref="SpawnResolutionMode.RequireClearSpace"/> is set and the
        /// entity could not be fully cleared.
        /// </returns>
        public static Vec3d? GetAdjustedSpawnPosition(
            EntitySpawnContext context,
            IBlockAccessor blockAccessor,
            EntityDimensions dimensions,
            PlacementConstraint constraints = PlacementConstraint.None,
            bool avoidEntities = true,
            int maxDepenetrationSteps = DefaultDepenetrationSteps,
            SpawnResolutionMode resolutionMode = SpawnResolutionMode.BestEffort)
        {
            var pos = GetSurfacePosition(context, blockAccessor, dimensions, constraints);

            var face = context.Face;
            if (face == null) return pos;

            var world = context.Api.World;
            var testBox = dimensions.ToCollisionBox();

            for (int i = 0; i < maxDepenetrationSteps; i++)
            {
                Cuboidd? blocker = world.CollisionTester.GetCollidingCollisionBox(blockAccessor, testBox, pos, false);

                if (blocker == null && avoidEntities)
                {
                    var entity = FirstIntersectingEntity(context, pos, dimensions);
                    if (entity != null) blocker = EntityWorldBox(entity);
                }

                if (blocker == null) break;

                var escaped = Depenetrate(pos, dimensions, blocker);
                if (escaped.X == pos.X && escaped.Y == pos.Y && escaped.Z == pos.Z) break;

                pos.X = escaped.X;
                pos.Y = escaped.Y;
                pos.Z = escaped.Z;
            }

            if (resolutionMode == SpawnResolutionMode.RequireClearSpace)
            {
                if (!BlockCollisionUtils.HasClearance(world, blockAccessor, testBox, pos))
                    return null;

                if (avoidEntities && FirstIntersectingEntity(context, pos, dimensions) != null)
                    return null;
            }

            return pos;
        }

        /// <summary>
        /// Computes the bottom-center position for the given target face,
        /// sized to the entity so it rests against the selected surface
        /// without colliding: top face seats the feet on the surface, bottom
        /// face puts the head at the ceiling plane, side faces put the back
        /// half a width away from the wall plane and center vertically on
        /// the aim point (never below the targeted cell's base).
        /// Face-plane coordinates come from integer block positions - ray
        /// hit coordinates carry float jitter near planes.
        /// </summary>
        private static Vec3d ApplyFacePosition(
            EntitySpawnContext context,
            IBlockAccessor blockAccessor,
            BlockFacing face,
            EntityDimensions dimensions)
        {
            var position = context.Position;
            var blockPos = context.BlockPosition;

            if (face == BlockFacing.UP)
            {
                var surfaceTop = BlockCollisionUtils.GetSurfaceTop(blockAccessor.GetBlock(blockPos));
                return new Vec3d(position.X, blockPos.Y + surfaceTop, position.Z);
            }

            if (face == BlockFacing.DOWN)
            {
                // Head must touch the ceiling underside. Partial undersides
                // (trapdoors etc.) keep their aim height; near-zero hits are
                // snapped to the exact plane to cancel ray jitter.
                var hitY = (context.HitPosition?.Y ?? 0.5);
                if (hitY <= PlaneSnap) hitY = 0;
                return new Vec3d(position.X, blockPos.Y + hitY - dimensions.Height, position.Z);
            }

            // Side faces: outward coordinate derived from the integer block
            // position and face direction so the entity's back sits exactly
            // on the face plane. The origin is at the feet, so shift down
            // half the height to center the entity vertically on the aim
            // point - but never let it sink below the targeted cell's base
            // plane (aiming low seats the entity on the ground).
            var normal = face.Normalf;
            var halfWidth = dimensions.Width / 2f;
            var centerX = position.X;
            var centerZ = position.Z;
            if (normal.X > 0) centerX = blockPos.X + 1 + halfWidth;
            else if (normal.X < 0) centerX = blockPos.X - halfWidth;
            if (normal.Z > 0) centerZ = blockPos.Z + 1 + halfWidth;
            else if (normal.Z < 0) centerZ = blockPos.Z - halfWidth;

            var centerY = Math.Max(position.Y - dimensions.Height / 2f, blockPos.Y);
            return new Vec3d(centerX, centerY, centerZ);
        }

        /// <summary>
        /// Whether the targeted face satisfies the placement constraints.
        /// Top faces always pass; drops were rejected by the caller before this.
        /// </summary>
        private static bool IsFaceAllowed(BlockFacing face, PlacementConstraint constraints)
        {
            if (face == BlockFacing.UP) return true;
            if (constraints == PlacementConstraint.None) return true;

            if (face == BlockFacing.DOWN)
            {
                return (constraints & (PlacementConstraint.NoCeilingPlacement | PlacementConstraint.MustBeOnGround)) == 0;
            }

            return (constraints & (PlacementConstraint.NoWallPlacement | PlacementConstraint.MustBeOnGround)) == 0;
        }

        /// <summary>
        /// Pushes the entity out of the given blocker box along the axis with
        /// the smallest penetration depth (minimum translation vector), so
        /// the resulting gap is zero - the boxes merely touch again.
        /// Returns the position unchanged when no positive escape exists.
        /// </summary>
        private static Vec3d Depenetrate(Vec3d pos, EntityDimensions dimensions, Cuboidd blocker)
        {
            double halfWidth = dimensions.Width / 2f;

            double minX = pos.X - halfWidth;
            double maxX = pos.X + halfWidth;
            double minY = pos.Y;
            double maxY = pos.Y + dimensions.Height;
            double minZ = pos.Z - halfWidth;
            double maxZ = pos.Z + halfWidth;

            double best = double.MaxValue;
            double dx = 0, dy = 0, dz = 0;

            Consider(+1, 0, 0, blocker.X2 - minX, ref best, ref dx, ref dy, ref dz);
            Consider(-1, 0, 0, maxX - blocker.X1, ref best, ref dx, ref dy, ref dz);
            Consider(0, +1, 0, blocker.Y2 - minY, ref best, ref dx, ref dy, ref dz);
            Consider(0, -1, 0, maxY - blocker.Y1, ref best, ref dx, ref dy, ref dz);
            Consider(0, 0, +1, blocker.Z2 - minZ, ref best, ref dx, ref dy, ref dz);
            Consider(0, 0, -1, maxZ - blocker.Z1, ref best, ref dx, ref dy, ref dz);

            return new Vec3d(pos.X + dx, pos.Y + dy, pos.Z + dz);
        }

        private static void Consider(
            double ax, double ay, double az, double depth,
            ref double best, ref double dx, ref double dy, ref double dz)
        {
            if (depth <= 0 || depth >= best) return;
            best = depth;
            dx = ax * depth;
            dy = ay * depth;
            dz = az * depth;
        }

        /// <summary>
        /// First alive entity intersecting the spawn footprint, via
        /// <see cref="IWorldAccessor.GetIntersectingEntities"/> - the same
        /// helper the game uses to decide whether a block can be placed.
        /// The collision boxes are relative to <paramref name="pos"/>'s cell;
        /// a non-null match predicate is required (null crashes the engine).
        /// </summary>
        private static Entity? FirstIntersectingEntity(EntitySpawnContext context, Vec3d pos, EntityDimensions dimensions)
        {
            var world = context.Api.World;
            var basePos = new BlockPos(
                (int)Math.Floor(pos.X),
                (int)Math.Floor(pos.Y),
                (int)Math.Floor(pos.Z));

            float relX = (float)(pos.X - basePos.X);
            float relY = (float)(pos.Y - basePos.Y);
            float relZ = (float)(pos.Z - basePos.Z);
            var halfWidth = dimensions.Width / 2f;

            var localBox = new Cuboidf(
                relX - halfWidth, relY, relZ - halfWidth,
                relX + halfWidth, relY + dimensions.Height, relZ + halfWidth);

            var found = world.GetIntersectingEntities(basePos, [localBox], e => e != null && e.Alive);
            return (found != null && found.Length > 0) ? found[0] : null;
        }

        /// <summary>
        /// The entity's collision box in world coordinates (its
        /// <see cref="Entity.CollisionBox"/> is relative to its position).
        /// Null when the entity has no usable box.
        /// </summary>
        private static Cuboidd? EntityWorldBox(Entity entity)
        {
            var box = entity.CollisionBox;
            if (box == null) return null;
            var p = entity.Pos;
            if (p == null) return null;
            return new Cuboidd(p.X + box.X1, p.Y + box.Y1, p.Z + box.Z1, p.X + box.X2, p.Y + box.Y2, p.Z + box.Z2);
        }
    }
}
