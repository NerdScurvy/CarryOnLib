namespace CarryOn.API.Common.Models
{
    /// <summary>
    /// Chooses how the collision box used for spawn placement tests relates
    /// to the spawned entity's own physics box.
    /// </summary>
    public enum SpawnCollisionBoxMode
    {
        /// <summary>
        /// The box is used only by placement tests; the entity keeps the
        /// collision box defined by its entity type JSON.
        /// </summary>
        PlacementTestOnly = 0,

        /// <summary>
        /// The box is also assigned to <c>Entity.CollisionBox</c> before
        /// spawning, so in-game physics use exactly the footprint that was
        /// tested during placement.
        /// </summary>
        AlwaysEnabled = 1,
    }
}
