using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace CarryOn.API.Common.Interfaces
{
    /// <summary>
    /// Contract for block behaviors that support transferring carryable items between
    /// the player's carried state and a block entity's inventory slots (e.g., shelving,
    /// display cases, barrel racks). Implement this interface to opt in to the transfer
    /// interaction system.
    ///
    /// The member shape mirrors the entity slot handler pattern
    /// (<see cref="ICarryableEntitySlotHandler"/>): a <c>Can*</c>/<c>Try*</c> split for
    /// pre-hold validation and authoritative mutation, a separate delay query
    /// (<see cref="GetInteractDelay"/>), and per-slot routing/validation methods. The
    /// mechanism terminology used to configure the handler is unchanged: a block opts in
    /// via the "transferCode" attribute of its <c>BlockBehaviorCarryable</c>, either with
    /// the reserved value "auto" (scans the block's behaviors for an implementation) or a
    /// code registered with the carry manager.
    ///
    /// Indexing contract: <see cref="GetSlotIndex"/> maps the raw
    /// <see cref="BlockSelection.SelectionBoxIndex"/> to the logical 0-based inventory
    /// slot index expected by every other member, returning -1 when the selection is not a
    /// transfer slot. The two index spaces differ and must not be mixed. For single-box
    /// racks the mapping is usually the identity. The server guards against forged logical
    /// indices with <see cref="IsValidSlot"/> before running the authoritative Try* call.
    ///
    /// Client/server split: <see cref="CanPut"/>/<see cref="CanTake"/> and the routing
    /// methods run on the client before the hold-to-interact progress circle starts, so
    /// invalid targets fail immediately instead of after the player has held the full
    /// interaction duration. <see cref="TryPut"/>/<see cref="TryTake"/> run on the server,
    /// are authoritative, and re-validate internally (server-side re-validation). The
    /// client never invokes the Try* methods.
    ///
    /// Error contract: a false return sets <paramref name="failureCode"/> to the full lang
    /// key of a message owned by the implementing mod (e.g. <c>rackemup:shattered</c>),
    /// which CarryOn resolves verbatim with <c>Lang.GetL(player.LanguageCode, failureCode)</c>,
    /// so the text is in the language of the player who will read it rather than the
    /// server's. A null or empty
    /// failureCode with a false return means "not this handler / continue to the next
    /// interaction". The reserved sentinel codes from
    /// <see cref="CarryConstants.FailureCodes"/> are not lang keys and keep their special
    /// meaning: <c>__ignore__</c> silently completes the interaction, <c>__default__</c>
    /// lets the vanilla interaction proceed, <c>__stop__</c> completes the interaction.
    /// <seealso cref="ICarryableEntitySlotHandler"/>
    /// </summary>
    public interface ICarryableBlockSlotHandler
    {
        /// <summary>
        /// Interaction hold duration in seconds for the progress circle when putting or
        /// taking. Invoked on the client at the start of the interaction and stored in the
        /// interaction's transfer delay; the handler may customise the duration per block
        /// and per slot.
        /// </summary>
        /// <param name="slotIndex">The logical slot index (as returned by
        /// <see cref="GetSlotIndex"/> and expected by <see cref="TryPut"/>/<see cref="TryTake"/>).</param>
        float GetInteractDelay(BlockEntity blockEntity, ICoreAPI api, int slotIndex);

        /// <summary>
        /// Maps a block selection (position + <see cref="BlockSelection.SelectionBoxIndex"/>)
        /// to a logical inventory slot index (0-based position in the block entity's
        /// inventory). Returns -1 for selections that are not transfer slots.
        /// </summary>
        int GetSlotIndex(BlockEntity blockEntity, BlockSelection selection);

        /// <summary>
        /// Whether the slot exists on the block entity and can be used for carry transfer.
        /// Cheap existence check used for routing on the client and as defense-in-depth
        /// against forged logical indices on the server before the Try* call.
        /// </summary>
        bool IsValidSlot(BlockEntity blockEntity, int slotIndex);

        /// <summary>
        /// True if the slot is empty and can accept a carried block. Routing/hint support
        /// only - it is not the source of the "occupied" error; mods own that text, so the
        /// validator still calls <see cref="CanPut"/> to obtain the mod's failure code.
        /// </summary>
        bool IsSlotEmpty(BlockEntity blockEntity, int slotIndex);

        /// <summary>
        /// True if the slot holds a carryable block that can be taken. Hint support:
        /// drives the take hint and slot-state feedback. It is NOT the routing decision -
        /// the empty/non-carryable-slot outcome is decided by <see cref="CanTake"/>'s
        /// failure code.
        /// </summary>
        bool IsSlotCarryable(BlockEntity blockEntity, int slotIndex);

        /// <summary>
        /// Non-mutating client-side pre-check: may the player start a put into this slot?
        /// Runs BEFORE the hold-to-interact progress circle starts, so invalid targets fail
        /// immediately. Returns false and sets <paramref name="failureCode"/> on failure.
        /// The server re-validates authoritatively in <see cref="TryPut"/>.
        /// </summary>
        bool CanPut(IPlayer player, BlockEntity blockEntity, int slotIndex, ItemStack itemStack,
            ICoreAPI api, out string failureCode);

        /// <summary>
        /// Validates and puts the player's carried block into the slot.
        /// Server-authoritative; mutates inventories. Re-runs the <see cref="CanPut"/>
        /// checks internally (server-side re-validation). Returns false and sets
        /// <paramref name="failureCode"/> on failure. For pre-hold client validation use
        /// <see cref="CanPut"/>.
        /// </summary>
        bool TryPut(IPlayer player, BlockEntity blockEntity, int slotIndex, ItemStack itemStack,
            ITreeAttribute? blockEntityData, ICoreAPI api, out string failureCode);

        /// <summary>
        /// Non-mutating client-side pre-check: may the player start a take from this slot?
        /// Runs BEFORE the hold-to-interact progress circle starts, so invalid targets fail
        /// immediately. Returns false and sets <paramref name="failureCode"/> on failure.
        /// The failure code also routes the empty/non-carryable-slot outcome: null/empty
        /// continues to the next CarryOn interaction (e.g. whole-block pickup),
        /// `FailureCodes.Default` lets the vanilla interaction proceed, and a mod lang key
        /// shows that error. The server re-validates authoritatively in <see cref="TryTake"/>.
        /// </summary>
        bool CanTake(IPlayer player, BlockEntity blockEntity, int slotIndex,
            ICoreAPI api, out string failureCode);

        /// <summary>
        /// Validates and takes the slot's carryable block into the player's hands.
        /// Server-authoritative; mutates inventories. Re-runs the <see cref="CanTake"/>
        /// checks internally (server-side re-validation). Returns false and sets
        /// <paramref name="failureCode"/> on failure. For pre-hold client validation use
        /// <see cref="CanTake"/>.
        /// </summary>
        bool TryTake(IPlayer player, BlockEntity blockEntity, int slotIndex, ICoreAPI api,
            out ItemStack itemStack, out ITreeAttribute? blockEntityData, out string failureCode);

        /// <summary>
        /// Whether carrying the targeted block itself (picking it up as a whole) is allowed
        /// for the player. Used to block carrying blocks that host the transfer system
        /// (e.g. shelving) so they are only accessed via slot transfer.
        /// </summary>
        bool IsBlockCarryAllowed(IPlayer player, BlockSelection selection);

        /// <summary>
        /// Invoked on the client immediately after the put/take network message is sent,
        /// before the server processes it. This is the optimistic counterpart of the
        /// server-side mutation: the slot's state flips (empty to filled on put, filled
        /// to empty on take) and client-side side effects that mirror the server's change
        /// are applied while the slot still holds its pre-transfer contents.
        /// Implementations may be no-ops.
        /// </summary>
        /// <param name="slotIndex">The logical slot index.</param>
        /// <param name="isPut">True for put, false for take.</param>
        void OnClientTransferToggled(IPlayer player, int slotIndex, bool isPut);
    }
}
