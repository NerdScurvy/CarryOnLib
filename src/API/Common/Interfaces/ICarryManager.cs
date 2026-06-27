using System.Collections.Generic;
using CarryOn.API.Common.Models;
using CarryOn.API.Event;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace CarryOn.API.Common.Interfaces
{

    /// <summary>
    /// Central manager for the CarryOn carry system. Provides state queries, mutations,
    /// world I/O, and transform group resolver registration.
    /// </summary>
    public interface ICarryManager
    {
        /// <summary>Gets the core API backing this carry manager instance.</summary>
        ICoreAPI Api { get; }

        /// <summary>Gets the carry event hub for subscribing to carry lifecycle events.</summary>
        CarryEvents CarryEvents { get; }

        /// <summary>Gets the active CarryOn configuration, or null if not yet loaded.</summary>
        CarryOnConfig? Config { get; }

        /// <summary>Checks whether the entity has permission to carry the block at the specified position.</summary>
        bool HasPermissionToCarry(Entity entity, BlockPos pos);

        /// <summary>Gets all carried blocks currently stored on the entity.</summary>
        IEnumerable<CarriedBlock> GetAllCarried(Entity entity);

        /// <summary>Gets the carried block in the specified slot, or null if the slot is empty.</summary>
        CarriedBlock? GetCarried(Entity entity, CarrySlot slot);

        /// <summary>Sets the carried block in a slot determined by <paramref name="carriedBlock"/>.Slot.</summary>
        void SetCarried(Entity entity, CarriedBlock carriedBlock, CarrySlot? overrideSlot = null, bool markDirty = true);

        /// <summary>Removes the carried block from the specified slot, clearing it.</summary>
        void RemoveCarried(Entity entity, CarrySlot slot, bool markDirty = true);

        /// <summary>Swaps the carried blocks between two slots.</summary>
        bool SwapCarried(Entity entity, CarrySlot first, CarrySlot second);

        /// <summary>Converts the block at <paramref name="pos"/> into a <see cref="CarriedBlock"/> and removes it from the world.</summary>
        CarriedBlock? GetCarriedFromWorld(BlockPos pos, CarrySlot slot, bool checkIsCarryable = false);

        /// <summary>Converts the block at <paramref name="pos"/> into a <see cref="CarriedBlock"/> with entity context and failure reporting.</summary>
        CarriedBlock? GetCarriedFromWorld(Entity entity, BlockPos pos, CarrySlot slot, ref string failureCode, bool checkIsCarryable = false);

        /// <summary>Restores block entity data at the given position from a carried block's serialized state.</summary>
        void RestoreBlockEntityData(IWorldAccessor world, CarriedBlock carriedBlock, BlockPos pos, bool dropped = false);

        /// <summary>Tries to place a carried block into the world. Returns false and sets <paramref name="failureCode"/> on failure.</summary>
        bool TryPlaceDown(Entity entity, CarriedBlock carriedBlock, BlockSelection selection, ref string failureCode, bool dropped = false, bool playSound = true);

        /// <summary>Tries to pick up a block from the world into a carry slot. Returns false and sets <paramref name="failureCode"/> on failure.</summary>
        bool TryPickUp(Entity entity, BlockPos pos, CarrySlot carrySlot, ref string failureCode, bool checkIsCarryable = true, bool playSound = true, bool? captureAttachedSigns = null);

        /// <summary>Tries to place the carried block at a target position. Returns false and sets <paramref name="failureCode"/> on failure.</summary>
        bool TryPlaceDownAt(IPlayer player, CarrySlot carrySlot, BlockSelection selection, out BlockPos? placedAt, ref string failureCode);

        /// <summary>Tries to attach the carried block in hands to an entity attachment slot. Returns false and sets <paramref name="failureCode"/> on failure.</summary>
        bool TryAttach(IServerPlayer player, long targetEntityId, int slotIndex, ref string failureCode, bool playSound = true);

        /// <summary>Tries to detach a carryable block from an entity attachment slot. Returns false and sets <paramref name="failureCode"/> on failure.</summary>
        bool TryDetach(IServerPlayer player, long targetEntityId, int slotIndex, ref string failureCode, bool playSound = true);

        /// <summary>Sends a client message to lock hotbar slots corresponding to active carry slots.</summary>
        void LockHotbarSlots(IServerPlayer player);

        /// <summary>Drops carried blocks from the specified slots, attempting world placement within range.</summary>
        void DropCarried(Entity entity, IEnumerable<CarrySlot> slots, int range = 4);

        /// <summary>Drops a single carried block, attempting world placement within range.</summary>
        void DropCarriedBlock(Entity entity, CarriedBlock carriedBlock, int range = 4);

        /// <summary>Drops a carried block as an entity or item(s) at the given position.</summary>
        void DropBlockAsEntityOrItem(CarriedBlock carriedBlock, BlockPos centerBlock, IServerPlayer player, Entity entity);

        /// <summary>Initializes carry events discovered by the event bootstrap service.</summary>
        void InitEvents(ICoreAPI api);

        /// <summary>Registers a primary transform group resolver (determines the root/base transform group).</summary>
        void RegisterRootTransformGroupResolver(string modId, IRootTransformGroupResolver resolver);

        /// <summary>Attempts to get a registered primary transform group resolver by code.</summary>
        bool TryGetRootTransformGroupResolver(string resolverCode, out IRootTransformGroupResolver? resolver);

        /// <summary>Unregisters a previously registered primary transform group resolver.</summary>
        bool UnregisterRootTransformGroupResolver(IRootTransformGroupResolver resolver);

        /// <summary>Registers an attachment transform group resolver (determines child/attachment transforms).</summary>
        void RegisterAttachmentTransformGroupResolver(string modId, IAttachmentTransformGroupResolver resolver);

        /// <summary>Attempts to get a registered attachment transform group resolver by code.</summary>
        bool TryGetAttachmentTransformGroupResolver(string resolverCode, out IAttachmentTransformGroupResolver? resolver);

        /// <summary>Unregisters a previously registered attachment transform group resolver.</summary>
        bool UnregisterAttachmentTransformGroupResolver(IAttachmentTransformGroupResolver resolver);

        /// <summary>Checks whether the block has any carryable behavior.</summary>
        bool IsCarryable(Block block);

        /// <summary>Checks whether the block can be carried in the specified slot.</summary>
        bool IsCarryable(Block block, CarrySlot slot);

        /// <summary>Checks whether the entity can interact with a block while carrying in hands.</summary>
        bool CanInteractWhileCarrying(Block block);

        /// <summary>Increments the carried-state revision; server side increments, client side returns current.</summary>
        int TouchCarriedAttributes(Entity entity);

        /// <summary>Gets the current carried-state revision for change detection.</summary>
        int GetCarriedRevision(Entity entity);

    }
}
