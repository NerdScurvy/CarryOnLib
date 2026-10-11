using System.Collections.Generic;
using CarryOn.API.Common.Models;
using CarryOn.API.Event;
using CarryOn.Utility;
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
        /// <summary>
        /// Gets the core API backing this carry manager instance.
        /// </summary>
        ICoreAPI Api { get; }

        /// <summary>
        /// Gets the carry event hub for subscribing to carry lifecycle events.
        /// </summary>
        CarryEvents CarryEvents { get; }

        /// <summary>
        /// Checks whether the entity has permission to carry the block at the specified position.
        /// </summary>
        bool HasPermissionAt(Entity entity, BlockPos pos, bool showErrorMessage = true);

        /// <summary>
        /// Gets all carried blocks currently stored on the entity.
        /// </summary>
        IEnumerable<CarriedBlock> GetAllCarried(Entity entity);

        /// <summary>
        /// Gets the carried block in the specified slot, or null if the slot is empty.
        /// </summary>
        CarriedBlock? GetCarried(Entity entity, CarrySlot slot);

        /// <summary>
        /// Gets the full protected read result of a carry slot, including the raw slot tree,
        /// so callers can distinguish a present-but-unusable carry (for example a block whose
        /// mod was removed, surfaced as <see cref="CarriedBlockReadResult.Failed(string)"/>
        /// with reason <c>missing-block</c>) from a genuinely empty slot. Never throws; see
        /// <see cref="CarriedBlockTreeSerializer.DeserializeSafe(ITreeAttribute, ICoreAPI?, CarrySlot)"/>.
        /// </summary>
        /// <param name="entity">The entity to inspect.</param>
        /// <param name="slot">The slot to read.</param>
        /// <param name="rawTree">The untouched slot attribute tree when the slot is present, otherwise null.</param>
        CarriedBlockReadResult GetCarriedReadResult(Entity entity, CarrySlot slot, out ITreeAttribute? rawTree);

        /// <summary>
        /// Checks whether a carry slot is occupied, without parsing or logging the carried tree.
        /// A slot holding a present-but-unreadable carry (for example a block whose mod was
        /// removed) still counts as occupied; use <see cref="GetCarried"/> /
        /// <see cref="GetCarriedReadResult"/> to distinguish readable carry content.
        /// </summary>
        bool IsCarrySlotOccupied(Entity entity, CarrySlot slot);

        /// <summary>
        /// Gets the carried block for a slot as it should be rendered. On the client, a slot
        /// holding a present-but-unreadable carried tree (for example a block whose mod was
        /// removed) is substituted with a render-only placeholder so the player can see something
        /// is carried; the underlying data is never modified. On the server this behaves exactly
        /// like <see cref="GetCarried"/>.
        /// </summary>
        CarriedBlock? GetRenderableCarried(Entity entity, CarrySlot slot);

        /// <summary>
        /// Sets the carried block in a slot determined by <paramref name="carriedBlock"/>.Slot.
        /// </summary>
        void SetCarried(Entity entity, CarriedBlock carriedBlock, CarrySlot? overrideSlot = null, bool markDirty = true);

        /// <summary>
        /// Removes the carried block from the specified slot, clearing it.
        /// </summary>
        void RemoveCarried(Entity entity, CarrySlot slot, bool markDirty = true);

        /// <summary>
        /// Swaps the carried blocks between two slots.
        /// </summary>
        bool SwapCarried(Entity entity, CarrySlot first, CarrySlot second);

        /// <summary>
        /// Adopts an untouched carried slot tree into the given slot without parsing it. Intended
        /// for present-but-unreadable carries (for example a block whose mod was removed), so the
        /// raw content can be recovered later once the source mod is re-enabled. Refuses when the
        /// destination slot is already occupied. Server-side only.
        /// </summary>
        /// <param name="entity">The entity receiving the raw carried tree.</param>
        /// <param name="rawTree">The untouched slot tree to adopt.</param>
        /// <param name="slot">The carry slot to place it in.</param>
        /// <returns>True when the raw tree was placed.</returns>
        bool SetCarriedRaw(Entity entity, ITreeAttribute? rawTree, CarrySlot slot);

        /// <summary>
        /// Converts the block at <paramref name="pos"/> into a <see cref="CarriedBlock"/> and removes it from the world.
        /// </summary>
        CarriedBlock? GetCarriedFromWorld(BlockPos pos, CarrySlot slot, bool checkIsCarryable = false);

        /// <summary>
        /// Converts the block at <paramref name="pos"/> into a <see cref="CarriedBlock"/> with entity context and failure reporting.
        /// </summary>
        CarriedBlock? GetCarriedFromWorld(Entity entity, BlockPos pos, CarrySlot slot, ref string failureCode, bool checkIsCarryable = false);

        /// <summary>
        /// Restores block entity data at the given position from a carried block's serialized state.
        /// </summary>
        void RestoreBlockEntityData(IWorldAccessor world, CarriedBlock carriedBlock, BlockPos pos, bool dropped = false);

        /// <summary>
        /// Tries to place a carried block into the world. Returns false and sets <paramref name="failureCode"/> on failure.
        /// </summary>
        bool TryPlaceDown(Entity entity, CarriedBlock carriedBlock, BlockSelection selection, ref string failureCode, bool dropped = false, bool playSound = true);

        /// <summary>
        /// Tries to pick up a block from the world into a carry slot. Returns false and sets <paramref name="failureCode"/> on failure.
        /// </summary>
        bool TryPickUp(Entity entity, BlockPos pos, CarrySlot carrySlot, ref string failureCode, bool checkIsCarryable = true, bool playSound = true, bool? captureAttachedSigns = null);

        /// <summary>
        /// Tries to pick a single bag item (backpack, hand basket, sack, quiver, ...) out of a ground
        /// storage slot (resolved from <paramref name="selection"/> against
        /// <c>BlockEntityGroundStorage.GetSlotAt</c>) and carry it in the hands slot. The ground storage
        /// block itself is never picked up. Server-authoritative; returns false and sets <paramref name="failureCode"/>
        /// on failure. The client must supply the exact hit position/face so both sides resolve the identical slot.
        /// </summary>
        bool TryPickUpGroundStorageBag(Entity entity, BlockSelection selection, ref string failureCode, int? maxInteractionDistance = null, bool playSound = true);

        /// <summary>
        /// Tries to stow the bag currently carried in the hands slot (picked up from ground storage) into the
        /// first empty slot of the entity's vanilla "backpack" inventory. Never touches the back carry slot.
        /// Returns false and sets <paramref name="failureCode"/> on failure (e.g. no empty backpack slot).
        /// </summary>
        bool TryStoreBagInBackpack(Entity entity, ref string failureCode, bool playSound = true);

        /// <summary>
        /// Tries to place the carried block at a target position. Returns false and sets <paramref name="failureCode"/> on failure.
        /// </summary>
        bool TryPlaceDownAt(IPlayer player, CarrySlot carrySlot, BlockSelection selection, out BlockPos? placedAt, ref string failureCode);

        /// <summary>
        /// Sends a client message to lock hotbar slots corresponding to active carry slots.
        /// </summary>
        /// <remarks>
        /// Server-only: this method operates on server state and sends to a single
        /// <see cref="IServerPlayer"/>. Calling it from the client side has no effect
        /// (there is no outgoing player channel on the client for this message).
        /// </remarks>
        void LockHotbarSlots(IServerPlayer player);

        /// <summary>
        /// Drops carried blocks from the specified slots, attempting world placement within range.
        /// </summary>
        void DropCarried(Entity entity, IEnumerable<CarrySlot> slots, int range = 4);

        /// <summary>
        /// Drops a single carried block, attempting world placement within range.
        /// </summary>
        void DropCarriedBlock(Entity entity, CarriedBlock carriedBlock, int range = 4);

        /// <summary>
        /// Drops a carried block as an entity or item(s) at the given position.
        /// </summary>
        void DropBlockAsEntityOrItem(CarriedBlock carriedBlock, BlockPos centerBlock, IServerPlayer player, Entity entity);

        /// <summary>
        /// Initializes carry events discovered by the event bootstrap service.
        /// </summary>
        void InitEvents(ICoreAPI api);

        /// <summary>
        /// Registers a carry event handler. The handler will be initialized during <see cref="InitEvents"/>.
        /// </summary>
        /// <typeparam name="T">The event handler type, which must implement <see cref="ICarryEventHandler"/>.</typeparam>
        void RegisterEventHandler<T>() where T : ICarryEventHandler, new();

        /// <summary>
        /// Registers a primary transform group resolver (determines the root/base transform group).
        /// </summary>
        void RegisterRootTransformGroupResolver(string modId, IRootTransformGroupResolver resolver);

        /// <summary>
        /// Attempts to get a registered primary transform group resolver by code.
        /// </summary>
        bool TryGetRootTransformGroupResolver(string resolverCode, out IRootTransformGroupResolver? resolver);

        /// <summary>
        /// Unregisters a previously registered primary transform group resolver.
        /// </summary>
        bool UnregisterRootTransformGroupResolver(IRootTransformGroupResolver resolver);

        /// <summary>
        /// Registers an attachment transform group resolver (determines child/attachment transforms).
        /// </summary>
        void RegisterAttachmentTransformGroupResolver(string modId, IAttachmentTransformGroupResolver resolver);

        /// <summary>
        /// Attempts to get a registered attachment transform group resolver by code.
        /// </summary>
        bool TryGetAttachmentTransformGroupResolver(string resolverCode, out IAttachmentTransformGroupResolver? resolver);

        /// <summary>
        /// Unregisters a previously registered attachment transform group resolver.
        /// </summary>
        bool UnregisterAttachmentTransformGroupResolver(IAttachmentTransformGroupResolver resolver);

        /// <summary>
        /// Registers an entity transform group resolver (overrides the wearing entity's
        /// base transform group during rendering, e.g. for cosmetic mods that hide the
        /// wearer's backpack).
        /// </summary>
        void RegisterEntityTransformGroupResolver(string modId, IEntityTransformGroupResolver resolver);

        /// <summary>
        /// Attempts to get a registered entity transform group resolver by code.
        /// </summary>
        bool TryGetEntityTransformGroupResolver(string resolverCode, out IEntityTransformGroupResolver? resolver);

        /// <summary>
        /// Unregisters a previously registered entity transform group resolver.
        /// </summary>
        bool UnregisterEntityTransformGroupResolver(IEntityTransformGroupResolver resolver);

        /// <summary>
        /// Enumerates all registered entity transform group resolvers in registration order.
        /// </summary>
        IEnumerable<IEntityTransformGroupResolver> EnumerateEntityTransformGroupResolvers();

        /// <summary>
        /// Resolves the wearing entity's base transform group override. Iterates registered
        /// entity resolvers in registration order; the first resolver to claim a non-empty
        /// group wins. Returns <paramref name="baseGroup"/> when no resolver overrides it.
        /// </summary>
        string ResolveEntityTransformGroupOverride(EntityAgent entity, CarrySlot carrySlot, string baseGroup);

        /// <summary>
        /// Checks whether the block has any carryable behavior.
        /// </summary>
        bool IsCarryable(Block block);

        /// <summary>
        /// Checks whether the block can be carried in the specified slot.
        /// </summary>
        bool IsCarryable(Block block, CarrySlot slot);

        /// <summary>
        /// Checks whether the entity can interact with a block while carrying in hands.
        /// </summary>
        bool CanInteractWhileCarrying(Block block);

        /// <summary>
        /// Increments the carried-state revision; server side increments, client side returns current.
        /// </summary>
        int TouchCarriedAttributes(Entity entity);

        /// <summary>
        /// Gets the current carried-state revision for change detection.
        /// </summary>
        int GetCarriedRevision(Entity entity);

        /// <summary>
        /// Registers a transfer behavior by its block behavior code.
        /// The behavior must already be registered with the VintageStory API
        /// via <c>api.RegisterBlockBehaviorClass(code, type)</c>.
        /// </summary>
        void RegisterTransferBehavior(string modId, string behaviorCode);

        /// <summary>
        /// Enumerates all registered transfer behavior codes.
        /// </summary>
        IEnumerable<string> EnumerateTransferBehaviors();

        /// <summary>
        /// Registers a standalone transfer handler for carryable block transfer processing.
        /// The handlerCode is referenced in BlockBehaviorCarryable JSON's "transferCode" attribute.
        /// Block behavior-based implementations of ICarryableBlockSlotHandler are auto-discovered
        /// and don't need registration - only standalone singletons need this.
        /// </summary>
        void RegisterTransferHandler(string handlerCode, ICarryableBlockSlotHandler handler);

        /// <summary>
        /// Attempts to get a registered transfer handler instance by code.
        /// </summary>
        bool TryGetTransferHandler(string handlerCode, out ICarryableBlockSlotHandler? handler);

        /// <summary>
        /// Checks whether a transfer handler code has been registered.
        /// </summary>
        bool IsTransferHandlerRegistered(string handlerCode);

        /// <summary>
        /// Enumerates all registered transfer handler codes.
        /// </summary>
        IEnumerable<string> EnumerateTransferHandlers();

        /// <summary>
        /// Registers a standalone pickup handler for carryable entity pickup.
        /// The handlerCode is referenced in EntityBehaviorCarryable JSON's "handlerCode" attribute.
        /// All handlers are standalone singletons registered by their handlerCode and
        /// resolved by it at runtime.
        /// </summary>
        void RegisterEntityPickupHandler(string handlerCode, ICarryableEntityPickupHandler handler);

        /// <summary>
        /// Attempts to get a registered pickup handler instance by code.
        /// </summary>
        bool TryGetEntityPickupHandler(string handlerCode, out ICarryableEntityPickupHandler? handler);

        /// <summary>
        /// Checks whether a pickup handler code has been registered.
        /// </summary>
        bool IsEntityPickupHandlerRegistered(string handlerCode);

        /// <summary>
        /// Enumerates all registered pickup handler codes.
        /// </summary>
        IEnumerable<string> EnumerateEntityPickupHandlers();

        /// <summary>
        /// Registers a spawner factory for entity spawning during the placement/drop pipeline.
        /// The factoryCode is referenced in <see cref="CarriedBlock.SpawnerFactoryCode"/> and
        /// looked up when a carried block is placed or dropped. Factories are registered as
        /// instances to support dependency injection.
        /// </summary>
        void RegisterEntitySpawnerFactory(string factoryCode, ICarryEntitySpawnerFactory factory);

        /// <summary>
        /// Attempts to get a registered spawner factory instance by code.
        /// </summary>
        bool TryGetEntitySpawnerFactory(string factoryCode, out ICarryEntitySpawnerFactory? factory);

        /// <summary>
        /// Checks whether a spawner factory code has been registered.
        /// </summary>
        bool IsEntitySpawnerFactoryRegistered(string factoryCode);

        /// <summary>
        /// Enumerates all registered spawner factory codes.
        /// </summary>
        IEnumerable<string> EnumerateEntitySpawnerFactories();

        /// <summary>
        /// Registers a standalone slot handler for per-slot attach/detach of carryable
        /// blocks on entities with attachable inventories.
        /// The handlerCode is referenced in EntityBehaviorCarryable JSON's
        /// "slotHandlerCode" attribute.
        /// "auto" is reserved for explicit auto-detection of a behavior/entity
        /// implementing <see cref="ICarryableEntitySlotHandler"/> and must not be used
        /// as a handler code.
        /// </summary>
        void RegisterSlotHandler(string handlerCode, ICarryableEntitySlotHandler handler);

        /// <summary>
        /// Attempts to get a registered slot handler instance by code.
        /// </summary>
        bool TryGetSlotHandler(string handlerCode, out ICarryableEntitySlotHandler? handler);

        /// <summary>
        /// Checks whether a slot handler code has been registered.
        /// </summary>
        bool IsSlotHandlerRegistered(string handlerCode);

        /// <summary>
        /// Enumerates all registered slot handler codes.
        /// </summary>
        IEnumerable<string> EnumerateSlotHandlers();
    }
}
