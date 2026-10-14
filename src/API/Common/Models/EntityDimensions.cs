using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace CarryOn.API.Common.Models
{
    /// <summary>
    /// Horizontal and vertical extent of an entity's collision box.
    /// Width applies to both X and Z (entities are square in cross-section).
    /// </summary>
    public struct EntityDimensions
    {
        /// <summary> X/Z extent (hitboxSize.x from entity JSON). </summary>
        public float Width { get; }

        /// <summary> Y extent (hitboxSize.y from entity JSON). </summary>
        public float Height { get; }

        public EntityDimensions(float width, float height)
        {
            Width = width;
            Height = height;
        }

        /// <summary> 1x1 unit dimensions. </summary>
        public static EntityDimensions One => new(1f, 1f);

        /// <summary>
        /// Builds the position-relative collision box used by Vintage Story
        /// collision testing, matching the <c>Entity.CollisionBox</c> layout
        /// (origin at bottom-center of the entity).
        /// </summary>
        public Cuboidf ToCollisionBox()
        {
            var halfWidth = Width / 2f;
            return new Cuboidf(-halfWidth, 0f, -halfWidth, halfWidth, Height, halfWidth);
        }

        /// <summary>
        /// Reads dimensions from a resolved entity type's collision box size.
        /// Falls back to <see cref="One"/> when the type is null or reports non-positive size.
        /// </summary>
        public static EntityDimensions FromEntityType(EntityProperties? entityType)
        {
            if (entityType == null) return One;

            var width = entityType.CollisionBoxSize.X;
            var height = entityType.CollisionBoxSize.Y;
            return (width > 0f && height > 0f) ? new EntityDimensions(width, height) : One;
        }
    }
}
