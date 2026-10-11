using CarryOn.API.Common.Models;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace CarryOn.API.Common.Interfaces
{
    /// <summary>
    /// Contract for handlers that manage the pickup direction of carryable entities:
    /// converting an in-world entity into a <see cref="CarriedBlock"/> held by the player.
    ///
    /// <para>
    /// This interface is designed to be implemented by an <see cref="EntityBehavior"/>.
    /// When the entity's <c>EntityBehaviorCarryable</c> has <c>handlerCode</c> set to
    /// <c>"auto"</c>, the behavior system scans the entity's behaviors for an
    /// <see cref="ICarryableEntityPickupHandler"/> and uses it directly - no singleton
    /// registration required.
    /// </para>
    ///
    /// <para>
    /// Alternatively, a named <c>handlerCode</c> triggers a registry lookup via
    /// <see cref="ICarryManager.RegisterEntityPickupHandler(string, ICarryableEntityPickupHandler)"/>
    /// for a singleton pickup handler.
    /// </para>
    ///
    /// The placement/drop direction (CarriedBlock → Entity) is handled separately by
    /// <see cref="ICarryEntitySpawnerFactory"/>.
    /// <seealso cref="ICarryEntitySpawnerFactory"/>
    /// </summary>
    public interface ICarryableEntityPickupHandler
    {
        /// <summary>
        /// Interaction delay in seconds for the progress circle when picking up this entity.
        /// Invoked on the client; the handler may customise the duration based on the
        /// target entity or api.
        /// </summary>
        float GetInteractDelay(Entity entity, ICoreAPI api);

        /// <summary>
        /// Determines whether the specified player can pick up this entity.
        /// Called on both client (for interaction help) and server (for validation).
        /// </summary>
        bool CanPickup(
            EnumGameMode gameMode,
            string? playerUid,
            Entity entity,
            ICoreAPI api);

        /// <summary>
        /// Converts the entity's in-world data into a <see cref="CarriedBlock"/> for
        /// placement in the player's carry slot. Called on the server during pickup.
        /// Returns null if conversion fails.
        ///
        /// The returned <see cref="CarriedBlock"/> should have <see cref="CarriedBlock.SpawnerFactoryCode"/>
        /// set to the code of the <see cref="ICarryEntitySpawnerFactory"/> that should handle
        /// placement. This ensures the placement pipeline can look up the correct factory to
        /// recreate the entity when the carried block is placed or dropped.
        ///
        /// Custom entity blocks (racks, boats, ...) that must re-spawn as an entity when
        /// placed by hand should also set <see cref="CarriedBlock.SpawnAsEntityOnPlacement"/>
        /// to <c>true</c>; this signal decouples "has a factory code" from "re-spawn on
        /// placement" so ordinary blocks never round-trip through the entity factory by
        /// accident.
        /// </summary>
        CarriedBlock? SerializeToCarriedBlock(
            Entity entity,
            CarrySlot slot,
            ICoreAPI api);

        /// <summary>
        /// Called after the entity is successfully picked up. Allows the handler
        /// to perform cleanup (e.g., play sound, fire events) and despawn the entity.
        /// The handler is responsible for removing or despawning the entity from the world
        /// if that is the desired behavior.
        ///
        /// <para>
        /// <paramref name="carriedBlock"/> may be null when a current-unreadable carried
        /// entity (for example a block whose mod was removed) was adopted verbatim into a
        /// carry slot without parsing; handlers must tolerate this.
        /// </para>
        /// </summary>
        void OnPickedUp(
            Entity entity,
            IPlayer player,
            CarriedBlock? carriedBlock,
            ICoreAPI api);
    }
}
