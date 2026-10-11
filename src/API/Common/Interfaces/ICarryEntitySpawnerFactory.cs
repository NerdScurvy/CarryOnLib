using CarryOn.API.Common.Models;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace CarryOn.API.Common.Interfaces
{
    /// <summary>
    /// Contract for factories that spawn entities from carried blocks during the
    /// placement/drop pipeline. Each factory handles a specific entity type and
    /// is registered as an instance on <see cref="ICarryManager"/>.
    ///
    /// <para>
    /// When a <see cref="CarriedBlock"/> with a <see cref="CarriedBlock.SpawnerFactoryCode"/>
    /// is placed or dropped, the carry system looks up the registered
    /// <see cref="ICarryEntitySpawnerFactory"/> for that code and delegates entity creation to it.
    /// </para>
    ///
    /// <para>
    /// Factories are registered as instances (not types) to support dependency injection.
    /// External mods can inject their own dependencies (config, API references, services)
    /// through the factory's constructor.
    /// </para>
    ///
    /// The pickup direction (Entity → CarriedBlock) is handled separately by
    /// <see cref="ICarryableEntityPickupHandler"/>.
    /// <seealso cref="ICarryableEntityPickupHandler"/>
    /// </summary>
    public interface ICarryEntitySpawnerFactory
    {
        /// <summary>
        /// Determines whether this factory should spawn an entity for the given
        /// carried block instead of placing it as a block in the world.
        /// Called during both player placement and drop logic on both client and server.
        /// Return false to fall through to normal block placement or item drops.
        /// </summary>
        bool ShouldSpawnAsEntity(
            CarriedBlock carriedBlock,
            bool dropped,
            ICoreAPI api);

        /// <summary>
        /// Spawns the entity in the world from a <see cref="CarriedBlock"/>.
        /// Called on the server during drop or placement when
        /// <see cref="ShouldSpawnAsEntity"/> returned true.
        /// Returns the spawned entity, or null on failure (falls back to item drops).
        ///
        /// <para>
        /// The <see cref="EntitySpawnContext"/> provides the spawn position (with sub-block
        /// floating-point precision), the block target info, whether this is a drop vs
        /// placement, and the carried block data. The factory decides how to interpret
        /// this information - e.g. randomizing yaw on drop, aligning to hit face, etc.
        /// </para>
        /// </summary>
        Entity? SpawnFromCarriedBlock(EntitySpawnContext context);

        /// <summary>
        /// Called after the entity is successfully spawned from a carried block.
        /// Allows the factory to perform post-spawn setup.
        ///
        /// <b>Sound contract:</b> the factory is responsible for playing the placement sound for
        /// the entity-spawn path. The carry pipeline is silent when spawning the entity, so a
        /// factory that omits sound logic here places/drops the entity without any audio. The
        /// built-in <c>CarriedBlockEntityFactory</c> plays the block's place sound here
        /// (<c>carriedBlock.Block?.Sounds?.Place.Location</c>, falling back to the default place
        /// sound, via <c>api.World.PlaySoundAt</c>); custom factories should do the same.
        /// </summary>
        void OnSpawned(
            Entity entity,
            CarriedBlock carriedBlock,
            ICoreAPI api);
    }
}
