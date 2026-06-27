using System;
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
    public class CarryEvents
    {
        public event BeforePickUpBlockDelegate? BeforePickUpBlock;

        public event BeforeRemoveBlockDelegate? BeforeRemoveBlockFromWorld;

        public event BlockEntityDataDelegate? BeforeRestoreBlockEntityData;

        public event CheckPermissionAtDelegate? CheckPermissionAt;

        public event EventHandler<BlockDroppedEventArgs>? BlockDropped;

        public event EventHandler<BlockRemovedEventArgs>? BlockRemoved;

        public Action<Exception>? OnEventHandlerError { get; set; }

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
            var delegates = CheckPermissionAt?.GetInvocationList();
            if (delegates == null) return null;

            foreach (var del in delegates.Cast<CheckPermissionAtDelegate>())
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
