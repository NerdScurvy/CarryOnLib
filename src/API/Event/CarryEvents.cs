using System;
using System.Collections.Generic;
using System.Linq;
using CarryOn.API.Common.Models;
using CarryOn.API.Event.Data;
using CarryOn.API.Event.Delegates;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace CarryOn.API.Event
{
    /// <summary>
    /// Event bus for CarryOn lifecycle and permission events.
    /// Thread safety: this class is not thread-safe for subscribe/unsubscribe during trigger.
    /// All trigger methods iterate synchronously at the time of the trigger call.
    /// Subscribe/unsubscribe is safe between triggers but not during a concurrent trigger.
    /// This matches VintageStory's predominantly single-threaded game loop.
    /// </summary>
    public class CarryEvents
    {
        public event BeforePickUpBlockDelegate? BeforePickUpBlock;

        public event BeforeRemoveBlockDelegate? BeforeRemoveBlockFromWorld;

        public event BlockEntityDataDelegate? BeforeRestoreBlockEntityData;

        private readonly SortedDictionary<int, List<CheckPermissionAtDelegate>> permissionHandlers = new();

        public event EventHandler<BlockDroppedEventArgs>? BlockDropped;

        public event EventHandler<BlockRemovedEventArgs>? BlockRemoved;

        /// <summary>
        /// Optional callback invoked when a subscribed event handler throws an exception.
        /// If null, exceptions from handlers are silently swallowed.
        /// Set this during mod initialization to receive handler errors (e.g. for logging).
        /// </summary>
        public Action<Exception>? OnEventHandlerError { get; set; }

        /// <summary>
        /// Registers a permission-check handler at the given priority. Handlers run in ascending
        /// priority order (lower int = runs first). The first handler to return a non-null
        /// <c>hasPermission</c> decides the outcome; later handlers are not consulted.
        ///
        /// Returning a non-null value is an authoritative decision (allow or deny). Returning null
        /// means "no opinion" and defers to the next handler by priority.
        ///
        /// Priorities are not globally reserved; modders may choose any <c>int</c>. CarryOn's own
        /// internal handler (<c>DroppedBlockTracker</c>) registers at priority 0 by convention.
        /// A mod wanting to override internal drops handling would use a negative priority; one
        /// wanting to run after would use a positive priority.
        /// </summary>
        /// <param name="priority">Execution priority; lower runs first.</param>
        /// <param name="handler">The permission-check handler.</param>
        public void RegisterCheckPermissionAt(int priority, CheckPermissionAtDelegate handler)
        {
            ArgumentNullException.ThrowIfNull(handler);
            if (!permissionHandlers.TryGetValue(priority, out var list))
                permissionHandlers[priority] = list = new List<CheckPermissionAtDelegate>();
            list.Add(handler);
        }

        /// <summary>
        /// Removes a previously registered permission-check handler. Returns true if found and removed.
        /// </summary>
        /// <param name="handler">The permission-check handler previously registered.</param>
        /// <returns>True if the handler was found and removed; false if it was not registered.</returns>
        public bool UnregisterCheckPermissionAt(CheckPermissionAtDelegate handler)
            => permissionHandlers.Values.Any(list => list.Remove(handler));

        public bool? TriggerCheckPermissionAt(EntityPlayer playerEntity, BlockPos pos, bool isReinforced)
        {
            return OnCheckPermissionAt(playerEntity, pos, isReinforced);
        }

        public bool TriggerBeforePickUpBlock(Entity entity, BlockPos pos, CarrySlot slot, CarriedBlock carriedBlock, ref string failureCode)
        {
            return OnBeforePickUpBlock(entity, pos, slot, carriedBlock, ref failureCode);
        }

        public void TriggerBeforeRemoveBlockFromWorld(CarriedBlock carriedBlock, BlockPos pos)
        {
            OnBeforeRemoveBlockFromWorld(carriedBlock, pos);
        }

        public void TriggerBeforeRestoreBlockEntityData(BlockEntity blockEntity, ITreeAttribute blockEntityData, bool dropped)
        {
            OnBeforeRestoreBlockEntityData(blockEntity, blockEntityData, dropped);
        }

        public void TriggerBlockDropped(BlockPos position, Entity entity, CarriedBlock carriedBlock, bool destroyed = false, bool hadContents = false, bool blockPlaced = true)
        {
            var args = new BlockDroppedEventArgs
            {
                Entity = entity,
                Position = position,
                CarriedBlock = carriedBlock,
                Destroyed = destroyed,
                HadContents = hadContents,
                BlockPlaced = blockPlaced
            };

            OnBlockDropped(args);
        }

        public void TriggerBlockRemoved(BlockPos position)
        {
            var args = new BlockRemovedEventArgs
            {
                Position = position
            };

            OnBlockRemoved(args);
        }

        protected virtual bool? OnCheckPermissionAt(EntityPlayer playerEntity, BlockPos pos, bool isReinforced)
        {
            foreach (var list in permissionHandlers.Values)
            {
                foreach (var del in list)
                {
                    try
                    {
                        del(playerEntity, pos, isReinforced, out bool? hasPermission);
                        if (hasPermission != null) return hasPermission.Value;
                    }
                    catch (Exception ex)
                    {
                        OnEventHandlerError?.Invoke(ex);
                    }
                }
            }

            return null;
        }

        protected virtual bool OnBeforePickUpBlock(Entity entity, BlockPos pos, CarrySlot slot, CarriedBlock carriedBlock, ref string failureCode)
        {
            var delegates = BeforePickUpBlock?.GetInvocationList();
            if (delegates == null) return true;

            foreach (var del in delegates.Cast<BeforePickUpBlockDelegate>())
            {
                try
                {
                    del(entity, pos, slot, carriedBlock, out bool? canPickUp, out string delegateFailureCode);

                    if (delegateFailureCode != null) failureCode = delegateFailureCode;
                    if (canPickUp != null) return canPickUp.Value;
                }
                catch (Exception ex)
                {
                    OnEventHandlerError?.Invoke(ex);
                }
            }

            return true;
        }

        protected virtual void OnBeforeRemoveBlockFromWorld(CarriedBlock carriedBlock, BlockPos pos)
        {
            var delegates = BeforeRemoveBlockFromWorld?.GetInvocationList();
            if (delegates == null) return;

            foreach (var del in delegates.Cast<BeforeRemoveBlockDelegate>())
            {
                try
                {
                    del(carriedBlock, pos);
                }
                catch (Exception ex)
                {
                    OnEventHandlerError?.Invoke(ex);
                }
            }
        }

        protected virtual void OnBeforeRestoreBlockEntityData(BlockEntity blockEntity, ITreeAttribute blockEntityData, bool dropped)
        {
            var delegates = BeforeRestoreBlockEntityData?.GetInvocationList();
            if (delegates == null) return;

            foreach (var del in delegates.Cast<BlockEntityDataDelegate>())
            {
                try
                {
                    del(blockEntity, blockEntityData, dropped);
                }
                catch (Exception ex)
                {
                    OnEventHandlerError?.Invoke(ex);
                }
            }
        }

        protected virtual void OnBlockDropped(BlockDroppedEventArgs e)
        {
            var delegates = BlockDropped?.GetInvocationList();
            if (delegates == null) return;

            foreach (var del in delegates.Cast<EventHandler<BlockDroppedEventArgs>>())
            {
                try
                {
                    del(this, e);
                }
                catch (Exception ex)
                {
                    OnEventHandlerError?.Invoke(ex);
                }
            }
        }

        protected virtual void OnBlockRemoved(BlockRemovedEventArgs e)
        {
            var delegates = BlockRemoved?.GetInvocationList();
            if (delegates == null) return;

            foreach (var del in delegates.Cast<EventHandler<BlockRemovedEventArgs>>())
            {
                try
                {
                    del(this, e);
                }
                catch (Exception ex)
                {
                    OnEventHandlerError?.Invoke(ex);
                }
            }
        }
    }
}
