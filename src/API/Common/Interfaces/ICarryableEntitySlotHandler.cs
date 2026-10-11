using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace CarryOn.API.Common.Interfaces
{
    /// <summary>
    /// Contract for handlers that manage per-slot attach/detach of carryable blocks
    /// on entities with attachable inventories (boats, packs, etc.).
    ///
    /// Unlike <see cref="ICarryableEntityPickupHandler"/> (which converts a whole entity to
    /// a single <see cref="CarriedBlock"/> and despawns it), slot handlers operate on
    /// individual slots of an entity that stays in the world. The player attaches a
    /// carried block into a slot or detaches a slot's carryable block back into their
    /// hands.
    ///
    /// A slot handler only runs while its target entity is alive, so it does not need
    /// to be a singleton. It may be implemented by an <see cref="EntityBehavior"/> or by
    /// the entity class itself; the CarryOn mod's <c>EntityBehaviorCarryable</c>
    /// auto-detects such implementations when the entity has no "slotHandlerCode"
    /// configured, or when it is explicitly set to the reserved value "auto". Shared
    /// handlers may instead be registered with the carry manager as singletons and
    /// referenced by "slotHandlerCode" in the entity's JSON. "auto" is reserved and is
    /// never treated as a registered handler code.
    ///
    /// A slot handler may coexist with an <see cref="ICarryableEntityPickupHandler"/> on the
    /// same entity. While the player is looking at a storage-slot selection box, the
    /// slot handler takes precedence (per-slot attach/detach); while looking at any
    /// other part of the entity, the pickup handler governs whole-entity pickup.
    ///
    /// Indexing contract: methods named with a <c>selectionBoxIndex</c> parameter take
    /// the raw <see cref="EntitySelection.SelectionBoxIndex"/> (0 = the entity body,
    /// storage boxes start at 1). All other methods take the <b>logical slot index</b>
    /// returned by <see cref="GetSlotIndex"/> (0-based position in the entity's slot
    /// inventory). The two index spaces differ and must not be mixed.
    ///
    /// See <see cref="ICarryableEntityPickupHandler"/> for the complementary contract that
    /// converts a whole entity to a single <see cref="CarriedBlock"/> and back.
    /// <seealso cref="ICarryableEntityPickupHandler"/>
    /// </summary>
    public interface ICarryableEntitySlotHandler
    {
        /// <summary>
        /// Interaction hold duration in seconds for the progress circle when
        /// attaching or detaching. Invoked on the client; the handler may customise
        /// the duration per entity and per slot.
        /// </summary>
        /// <param name="slotIndex">The logical slot index (as returned by
        /// <see cref="GetSlotIndex"/> and expected by <see cref="TryAttach"/>/<see cref="TryDetach"/>).</param>
        float GetInteractDelay(Entity entity, ICoreAPI api, int slotIndex);

        /// <summary>
        /// Maps a selection box index to a logical slot index (0-based position in the
        /// entity's slot inventory). The selection box index is the raw
        /// <see cref="EntitySelection.SelectionBoxIndex"/>: 0 is the entity body and
        /// storage boxes start at 1. Returns -1 for selection boxes that are not
        /// storage slots (e.g. the entity body or seats).
        /// </summary>
        int GetSlotIndex(Entity entity, int selectionBoxIndex);

        /// <summary>
        /// Whether the slot exists on the entity and can be used for carry attach/detach.
        /// </summary>
        bool IsValidSlot(Entity entity, int slotIndex);

        /// <summary>
        /// True if the slot is empty and can accept a carried block.
        /// </summary>
        bool IsSlotEmpty(Entity entity, int slotIndex);

        /// <summary>
        /// True if the slot contains a carryable block that can be detached.
        /// </summary>
        bool IsSlotCarryable(Entity entity, int slotIndex);

        /// <summary>
        /// Non-mutating client-side pre-check: may the player start an attach into this slot?
        /// Runs BEFORE the hold-to-interact progress circle starts, so invalid targets fail
        /// immediately instead of after the player has held the full interaction duration.
        /// Returns false and sets <paramref name="failureCode"/> on failure (Ignored when the
        /// failure should be silent). The server re-validates authoritatively in
        /// <see cref="TryAttach"/>.
        ///
        /// Failure-code resolution: a code containing a domain separator (<c>:</c>) is a
        /// full lang key owned by the implementing mod and resolves verbatim (e.g.
        /// "rackemup:mold-rack-transfer-incompatible"); a code without one is a bare code
        /// that resolves as "carryon:&lt;code&gt;" (the standard CarryOn-scoped messages).
        /// CarryOn handles display of the error message automatically from the
        /// <paramref name="failureCode"/>, so implementations only need to set the code.
        /// The code may also be a flow-control value from <see cref="FailureCodes"/>:
        /// use <see cref="FailureCodes.Ignore"/> to suppress the error message entirely
        /// (e.g. when the failure should be silent).
        /// </summary>
        bool CanAttach(Entity entity, int slotIndex, IPlayer player, ICoreAPI api, out string failureCode);

        /// <summary>
        /// Validates and attaches the player's carried block in hands to the slot.
        /// Server-authoritative; mutates inventory. Returns false and sets
        /// <paramref name="failureCode"/> on failure. For pre-hold client validation use
        /// <see cref="CanAttach"/>. The code may also be a flow-control value from
        /// <see cref="FailureCodes"/> (e.g. <see cref="FailureCodes.Ignore"/> to suppress
        /// the error message).
        /// </summary>
        bool TryAttach(Entity entity, int slotIndex, IPlayer player, ICoreAPI api, out string failureCode);

        /// <summary>
        /// Non-mutating client-side pre-check: may the player start a detach from this slot?
        /// Runs BEFORE the hold-to-interact progress circle starts, so invalid targets fail
        /// immediately. Returns false and sets <paramref name="failureCode"/> on failure
        /// (Ignored when the failure should be silent). The server re-validates
        /// authoritatively in <see cref="TryDetach"/>.
        ///
        /// Failure-code resolution: a code containing a domain separator (<c>:</c>) is a
        /// full lang key owned by the implementing mod and resolves verbatim (e.g.
        /// "rackemup:mold-rack-transfer-incompatible"); a code without one is a bare code
        /// that resolves as "carryon:&lt;code&gt;" (the standard CarryOn-scoped messages).
        /// CarryOn handles display of the error message automatically from the
        /// <paramref name="failureCode"/>, so implementations only need to set the code.
        /// The code may also be a flow-control value from <see cref="FailureCodes"/>:
        /// use <see cref="FailureCodes.Ignore"/> to suppress the error message entirely
        /// (e.g. when the failure should be silent).
        /// </summary>
        bool CanDetach(Entity entity, int slotIndex, IPlayer player, ICoreAPI api, out string failureCode);

        /// <summary>
        /// Validates and detaches the slot's carryable block into the player's hands.
        /// Server-authoritative; mutates inventory. Returns false and sets
        /// <paramref name="failureCode"/> on failure. For pre-hold client validation use
        /// <see cref="CanDetach"/>. The code may also be a flow-control value from
        /// <see cref="FailureCodes"/> (e.g. <see cref="FailureCodes.Ignore"/> to suppress
        /// the error message).
        /// </summary>
        bool TryDetach(Entity entity, int slotIndex, IPlayer player, ICoreAPI api, out string failureCode);

        /// <summary>
        /// Gets the position for the interaction-help overlay for the given selection
        /// box, or null to fall back to entity-level positioning. The selection box
        /// index is the raw <see cref="EntitySelection.SelectionBoxIndex"/> (0 = the
        /// entity body, storage boxes start at 1).
        /// </summary>
        Vec3d? GetHelpPosition(Entity entity, int selectionBoxIndex);

        /// <summary>
        /// Invoked on the client immediately after the attach/detach network message is
        /// sent, before the server processes it. This is the optimistic counterpart of the
        /// server-side mutation: the slot's state flips (empty to filled on attach, filled
        /// to empty on detach) and client-side side effects that mirror the server's change
        /// are applied (e.g. clearing cached container contents keyed by the affected slot)
        /// while the slot still holds the pre-attach/detach contents.
        /// Implementations may be no-ops.
        /// </summary>
        /// <param name="isAttached">True for attach, false for detach.</param>
        void OnClientAttachmentToggled(Entity entity, int slotIndex, IPlayer player, bool isAttached);
    }
}
