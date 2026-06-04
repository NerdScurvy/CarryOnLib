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

    public interface ICarryManager
    {
        /// <summary>
        /// Gets the core API backing this carry manager instance.
        /// </summary>
        ICoreAPI Api { get; }

        /// <summary>
        /// Gets the carry event hub for this manager.
        /// </summary>
        CarryEvents CarryEvents { get; }


        /// <summary>
        /// Gets the active CarryOn configuration.
        /// </summary>
        /// <returns>The loaded configuration, or null when unavailable.</returns>
        CarryOnConfig GetConfig();

        /// <summary>
        /// Checks if the entity has permission to carry the block at the specified position.
        /// </summary>
        /// <param name="entity">The entity attempting to carry the block.</param>
        /// <param name="pos">The block position to check.</param>
        /// <returns>True if carry interaction is permitted; otherwise false.</returns>
        bool HasPermissionToCarry(Entity entity, BlockPos pos);

        /// <summary>
        /// Gets all carried blocks for the specified entity.
        /// </summary>
        /// <param name="entity">The entity whose carried blocks are requested.</param>
        /// <returns>An enumeration of carried blocks currently stored on the entity.</returns>
        IEnumerable<CarriedBlock> GetAllCarried(Entity entity);

        /// <summary>
        /// Gets the CarriedBlock carried by the entity in the specified carry slot.
        /// </summary>
        /// <param name="entity">The entity whose carried block is requested.</param>
        /// <param name="slot">The carry slot to inspect.</param>
        /// <returns>The carried block in the specified slot, or null when empty.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        CarriedBlock? GetCarried(Entity entity, CarrySlot slot);

        /// <summary>
        /// Sets the CarriedBlock for the entity in the specified carry slot.
        /// </summary>
        /// <param name="entity">The entity whose carried state is updated.</param>
        /// <param name="carriedBlock">The carried block to store.</param>
        void SetCarried(Entity entity, CarriedBlock carriedBlock);

        /// <summary>
        /// Sets the CarriedBlock for the entity in the specified carry slot.
        /// </summary>
        /// <param name="entity">The entity whose carried state is updated.</param>
        /// <param name="carriedBlock">The carried block to store.</param>
        /// <param name="markDirty">Whether to mark carried attributes dirty after the change.</param>
        void SetCarried(Entity entity, CarriedBlock carriedBlock, bool markDirty = true);

        /// <summary>
        /// Sets the CarriedBlock for the entity in the specified carry slot.
        /// </summary>
        /// <param name="entity">The entity whose carried state is updated.</param>
        /// <param name="slot">The destination carry slot.</param>
        /// <param name="itemStack">The block item stack to store in the slot.</param>
        /// <param name="blockEntityData">Serialized block entity data associated with the carried block.</param>
        /// <exception cref="ArgumentNullException"></exception>
        void SetCarried(Entity entity, CarrySlot slot, ItemStack itemStack, ITreeAttribute? blockEntityData);

        /// <summary>
        /// Sets the CarriedBlock for the entity in the specified carry slot.
        /// </summary>
        /// <param name="entity">The entity whose carried state is updated.</param>
        /// <param name="slot">The destination carry slot.</param>
        /// <param name="itemStack">The block item stack to store in the slot.</param>
        /// <param name="blockEntityData">Serialized block entity data associated with the carried block.</param>
        /// <param name="markDirty">Whether to mark carried attributes dirty after the change.</param>
        /// <exception cref="ArgumentNullException"></exception>
        void SetCarried(Entity entity, CarrySlot slot, ItemStack itemStack, ITreeAttribute? blockEntityData, bool markDirty = true);

        /// <summary>
        /// Removes the CarriedBlock from the entity in the specified carry slot.
        /// </summary>
        /// <param name="entity">The entity whose carried state is updated.</param>
        /// <param name="slot">The carry slot to clear.</param>
        /// <exception cref="ArgumentNullException"></exception>
        void RemoveCarried(Entity entity, CarrySlot slot);

        /// <summary>
        /// Removes the CarriedBlock from the entity in the specified carry slot.
        /// </summary>
        /// <param name="entity">The entity whose carried state is updated.</param>
        /// <param name="slot">The carry slot to clear.</param>
        /// <param name="markDirty">Whether to mark carried attributes dirty after the change.</param>
        /// <exception cref="ArgumentNullException"></exception>
        void RemoveCarried(Entity entity, CarrySlot slot, bool markDirty = true);

        /// <summary>
        ///   Attempts to swap the <see cref="CarriedBlock"/>s currently carried in the
        ///   entity's <paramref name="first"/> and <paramref name="second"/> slots.
        /// </summary>
        /// <param name="entity">The entity whose carried blocks are being swapped.</param>
        /// <param name="first">The first carry slot.</param>
        /// <param name="second">The second carry slot.</param>
        /// <exception cref="ArgumentNullException"> Thrown if entity is null. </exception>
        bool SwapCarried(Entity entity, CarrySlot first, CarrySlot second);

        /// <summary>
        /// Gets the CarriedBlock from the world at the specified position and slot.
        /// </summary>
        /// <param name="pos">The source world position to convert into a carried block.</param>
        /// <param name="slot">The carry slot metadata to assign to the returned block.</param>
        /// <param name="checkIsCarryable">Whether to validate slot carryability before conversion.</param>
        /// <returns>The converted carried block, or null when conversion fails.</returns>
        CarriedBlock GetCarriedFromWorld(BlockPos pos, CarrySlot slot, bool checkIsCarryable = false);

        /// <summary>
        /// Restores the block entity data for the carried block at the specified position.
        /// </summary>
        /// <param name="world">The world where block entity data should be restored.</param>
        /// <param name="carriedBlock">The carried block containing serialized block entity data.</param>
        /// <param name="pos">The target placement position.</param>
        /// <param name="dropped">Whether this restore is part of a drop flow.</param>
        void RestoreBlockEntityData(IWorldAccessor world, CarriedBlock carriedBlock, BlockPos pos, bool dropped = false);

        /// <summary>
        /// Tries to place the carriedBlock in the world, removing from entity if successful
        /// </summary>
        /// <param name="entity">The entity attempting to place the block.</param>
        /// <param name="carriedBlock">The carried block to place.</param>
        /// <param name="selection">The placement target selection.</param>
        /// <param name="dropped">If <c>true</c>, the block is set in world instead of placed, bypassing some checks.</param>
        /// <param name="playSound">Whether to play placement sound on success.</param>
        /// <returns>True if placement succeeds; otherwise false.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        bool TryPlaceDown(Entity entity, CarriedBlock carriedBlock, BlockSelection selection, bool dropped = false, bool playSound = true);

        /// <summary>
        /// Tries to place the carriedBlock in the world, removing from entity if successful
        /// </summary>
        /// <param name="entity">The entity attempting to place the block.</param>
        /// <param name="carriedBlock">The carried block to place.</param>
        /// <param name="selection">The placement target selection.</param>
        /// <param name="failureCode">Failure code output when placement fails.</param>
        /// <param name="dropped">If <c>true</c>, the block is set in world instead of placed, bypassing some checks.</param>
        /// <param name="playSound">Whether to play placement sound on success.</param>
        /// <returns>True if placement succeeds; otherwise false.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        bool TryPlaceDown(Entity entity, CarriedBlock carriedBlock, BlockSelection selection, ref string failureCode, bool dropped = false, bool playSound = true);

        /// <summary>
        /// Attempts to pick up a block from the specified position.
        /// </summary>
        /// <param name="entity">The entity attempting to pick up the block.</param>
        /// <param name="pos">The source block position.</param>
        /// <param name="carrySlot">The destination carry slot.</param>
        /// <param name="checkIsCarryable">Whether to validate carryability before pickup.</param>
        /// <param name="playSound">Whether to play pickup sound on success.</param>
        /// <returns>True if pickup succeeds; otherwise false.</returns>
        bool TryPickUp(Entity entity, BlockPos pos, CarrySlot carrySlot, bool checkIsCarryable = true, bool playSound = true);

        /// <summary>
        /// Attempts to pick up a block from the specified position.
        /// </summary>
        /// <param name="entity">The entity attempting to pick up the block.</param>
        /// <param name="pos">The source block position.</param>
        /// <param name="carrySlot">The destination carry slot.</param>
        /// <param name="failureCode">Failure code output when pickup fails.</param>
        /// <param name="checkIsCarryable">Whether to validate carryability before pickup.</param>
        /// <param name="playSound">Whether to play pickup sound on success.</param>
        /// <returns>True if pickup succeeds; otherwise false.</returns>
        bool TryPickUp(Entity entity, BlockPos pos, CarrySlot carrySlot, ref string failureCode, bool checkIsCarryable = true, bool playSound = true);

        /// <summary>
        /// Tries to place the carried block at the specified position.
        /// </summary>
        /// <param name="player">The player attempting to place from a carry slot.</param>
        /// <param name="carrySlot">The carry slot to place from.</param>
        /// <param name="selection">The initial target block selection.</param>
        /// <param name="placedAt">Position of where the block was placed. It may have replaced the selected block.</param>
        /// <returns>True if placement succeeds; otherwise false.</returns>
        bool TryPlaceDownAt(IPlayer player, CarrySlot carrySlot, BlockSelection selection, out BlockPos? placedAt);

        /// <summary>
        /// Tries to place the carried block at the specified position.
        /// </summary>
        /// <param name="player">The player attempting to place from a carry slot.</param>
        /// <param name="carrySlot">The carry slot to place from.</param>
        /// <param name="selection">The initial target block selection.</param>
        /// <param name="placedAt">Position of where the block was placed. It may have replaced the selected block.</param>
        /// <param name="failureCode">Failure code output when placement fails.</param>
        /// <returns>True if placement succeeds; otherwise false.</returns>
        bool TryPlaceDownAt(IPlayer player, CarrySlot carrySlot,
                                     BlockSelection selection, out BlockPos? placedAt, ref string failureCode);

        /// <summary>
        /// Tries to attach the carried block in player hands to an entity attachment slot.
        /// </summary>
        /// <param name="player">The acting server player.</param>
        /// <param name="targetEntityId">The target entity identifier.</param>
        /// <param name="slotIndex">The target attachable slot index.</param>
        /// <param name="playSound">Whether to play attach sound on success.</param>
        /// <returns>True if attach succeeds; otherwise false.</returns>
        bool TryAttach(IServerPlayer player, long targetEntityId, int slotIndex, bool playSound = true);

        /// <summary>
        /// Tries to attach the carried block in player hands to an entity attachment slot.
        /// </summary>
        /// <param name="player">The acting server player.</param>
        /// <param name="targetEntityId">The target entity identifier.</param>
        /// <param name="slotIndex">The target attachable slot index.</param>
        /// <param name="failureCode">Failure code output when attach fails.</param>
        /// <param name="playSound">Whether to play attach sound on success.</param>
        /// <returns>True if attach succeeds; otherwise false.</returns>
        bool TryAttach(IServerPlayer player, long targetEntityId, int slotIndex, ref string failureCode, bool playSound = true);

        /// <summary>
        /// Tries to detach a carryable block from an entity attachment slot to player hands.
        /// </summary>
        /// <param name="player">The acting server player.</param>
        /// <param name="targetEntityId">The target entity identifier.</param>
        /// <param name="slotIndex">The source attachable slot index.</param>
        /// <param name="playSound">Whether to play detach sound on success.</param>
        /// <returns>True if detach succeeds; otherwise false.</returns>
        bool TryDetach(IServerPlayer player, long targetEntityId, int slotIndex, bool playSound = true);

        /// <summary>
        /// Tries to detach a carryable block from an entity attachment slot to player hands.
        /// </summary>
        /// <param name="player">The acting server player.</param>
        /// <param name="targetEntityId">The target entity identifier.</param>
        /// <param name="slotIndex">The source attachable slot index.</param>
        /// <param name="failureCode">Failure code output when detach fails.</param>
        /// <param name="playSound">Whether to play detach sound on success.</param>
        /// <returns>True if detach succeeds; otherwise false.</returns>
        bool TryDetach(IServerPlayer player, long targetEntityId, int slotIndex, ref string failureCode, bool playSound = true);

        /// <summary>
        /// Sends a message to the player to lock the hotbar slots.
        /// </summary>
        /// <param name="player">The server player whose hotbar lock state should be updated.</param>
        void LockHotbarSlots(IServerPlayer player);

        /// <summary>
        /// Drops the carried blocks from the specified slots.
        /// </summary>
        /// <param name="entity">The entity dropping carried blocks.</param>
        /// <param name="slots">The carry slots to evaluate for dropping.</param>
        /// <param name="range">The placement search radius for drop attempts.</param>
        void DropCarried(Entity entity, IEnumerable<CarrySlot> slots, int range = 4);

        /// <summary>
        /// Drops the carried block as item(s) at the specified position.
        /// </summary>
        /// <param name="carriedBlock">The carried block being dropped.</param>
        /// <param name="centerBlock">The center position used for item spawning and sounds.</param>
        /// <param name="player">Optional server player context for drop rules.</param>
        /// <param name="entity">The entity source used for state updates and events.</param>
        void DropBlockAsItem(CarriedBlock carriedBlock, BlockPos centerBlock, IServerPlayer player, Entity entity);

        /// <summary>
        /// Initializes any carry events.
        /// </summary>
        /// <param name="api">Core API used for event discovery and initialization.</param>
        void InitEvents(ICoreAPI api);

        /// <summary>
        /// Registers a carried-render transform resolver.
        /// </summary>
        /// <param name="modId">Owning mod id for the resolver registration.</param>
        /// <param name="resolver">The resolver to register.</param>
        void RegisterTransformGroupResolver(string modId, ICarriedTransformGroupResolver resolver);

        /// <summary>
        /// Attempts to get a registered carried-render transform resolver by code.
        /// </summary>
        /// <param name="resolverCode">The resolver code to look up.</param>
        /// <param name="resolver">The resolver instance when found.</param>
        /// <returns>True if the resolver was found; otherwise false.</returns>
        bool TryGetTransformGroupResolver(string resolverCode, out ICarriedTransformGroupResolver? resolver);

        /// <summary>
        /// Attempts to get resolver registration metadata by resolver code.
        /// </summary>
        /// <param name="resolverCode">The resolver code to look up.</param>
        /// <param name="registration">The resolver registration when found.</param>
        /// <returns>True if the resolver registration was found; otherwise false.</returns>
        bool TryGetTransformGroupResolverRegistration(string resolverCode, out RegisteredTransformGroupResolver? registration);

        /// <summary>
        /// Unregisters a previously registered carried-render transform resolver.
        /// </summary>
        /// <param name="resolver">The resolver to unregister.</param>
        /// <returns>True if the resolver was removed; otherwise false.</returns>
        bool UnregisterTransformGroupResolver(ICarriedTransformGroupResolver resolver);

        /// <summary>
        /// Returns all currently registered carried-render transform resolvers.
        /// </summary>
        /// <returns>An ordered read-only list of registered resolvers.</returns>
        IReadOnlyList<RegisteredTransformGroupResolver> GetTransformGroupResolvers();

        /// <summary>
        /// Checks if the block is carryable.
        /// </summary>
        /// <param name="block">The block to evaluate.</param>
        /// <returns>True if the block has carryable behavior; otherwise false.</returns>
        bool IsCarryable(Block block);

        /// <summary>
        /// Checks if the block is carryable in the specified slot.
        /// </summary>
        /// <param name="block">The block to evaluate.</param>
        /// <param name="slot">The carry slot to validate.</param>
        /// <returns>True if the block can be carried in the specified slot; otherwise false.</returns>
        bool IsCarryable(Block block, CarrySlot slot);

        /// <summary>
        /// Checks if the entity can interact with a block while carrying in hands.
        /// </summary>
        /// <param name="block">The block to evaluate.</param>
        /// <returns>True if interaction while carrying is allowed; otherwise false.</returns>
        bool CanInteractWhileCarrying(Block block);

        /// <summary>
        /// Increments the carried-state revision and marks the carried root dirty if necessary.
        /// Client side only returns the current revision, while server side increments and returns the new revision.
        /// </summary>
        /// <param name="entity">The entity whose carried attributes are being touched.</param>
        /// <returns> The current carried-state revision. </returns>
        /// <exception cref="ArgumentNullException"> Thrown if entity is null. </exception>
        int TouchCarriedAttributes(Entity entity);

        /// <summary>
        /// Gets the current carried-state revision for the entity. This can be used to check if the carried state has changed since the last time it was checked.
        /// </summary>
        /// <param name="entity"> The entity whose carried attributes are being queried. </param>
        /// <returns> The current carried-state revision. </returns>
        /// <exception cref="ArgumentNullException"> Thrown if entity is null. </exception>
        int GetCarriedRevision(Entity entity);

    }
}