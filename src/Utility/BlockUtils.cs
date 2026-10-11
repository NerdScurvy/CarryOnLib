using System;
using CarryOn.API.Common.Models;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace CarryOn.Utility
{
    public static class BlockUtils
    {

        /// <summary>
        /// Creates a CarriedBlock from the specified position in the world.
        /// Does not remove the block from the world or assign to an entity.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="world"/> or <paramref name="pos"/> is null.</exception>
        public static CarriedBlock? CreateCarriedFromBlockPos(IWorldAccessor world, BlockPos pos, CarrySlot slot)
        {
            if (world == null) throw new ArgumentNullException(nameof(world));
            if (pos == null) throw new ArgumentNullException(nameof(pos));

            var block = world.BlockAccessor.GetBlock(pos);
            if (block.Id == 0) return null; // Can't pick up air.

            // Capture the original block code BEFORE OnPickBlock normalizes it
            var originalBlockCode = block.Code;

            var stack = block.OnPickBlock(world, pos) ?? new ItemStack(block);

            ITreeAttribute? blockEntityData = null;
            float? originalMeshAngle = null;
            var blockEntity = world.BlockAccessor.GetBlockEntity(pos);
            if (blockEntity != null)
            {
                blockEntityData = new TreeAttribute();
                blockEntity.ToTreeAttributes(blockEntityData);
                blockEntityData = blockEntityData.Clone();

                // Capture meshAngle BEFORE stripping: used for rotation delta on placement
                if (blockEntityData.HasAttribute("meshAngle"))
                    originalMeshAngle = blockEntityData.GetFloat("meshAngle");

                // We don't need to keep the position.
                blockEntityData.RemoveAttribute("posx");
                blockEntityData.RemoveAttribute("posy");
                blockEntityData.RemoveAttribute("posz");
                // And angle needs to be removed, or else it will
                // override the angle set from block placement.
                blockEntityData.RemoveAttribute("meshAngle");
            }

            return new CarriedBlock(slot, stack, blockEntityData, null, originalBlockCode, originalMeshAngle);
        }

        /// <summary>
        /// Returns the position that the specified block would
        /// be placed at for the specified block selection.
        /// </summary>
        public static BlockPos? GetPlacedPosition(IBlockAccessor blockAccessor, BlockSelection selection, Block block)
        {
            if (selection == null) return null;
            var position = selection.Position.Copy();
            var clickedBlock = blockAccessor.GetBlock(position);
            if (!clickedBlock.IsReplacableBy(block))
            {
                position.Offset(selection.Face);
                var replacedBlock = blockAccessor.GetBlock(position);
                if (!replacedBlock.IsReplacableBy(block)) return null;
            }
            return position;
        }

        /// <summary>
        /// Returns the world cell a carried ground-storable item (e.g. a bag picked up from
        /// ground storage) would occupy when placed at the given selection, mirroring
        /// <see cref="CollectibleBehaviorGroundStorable.Interact"/>: an existing ground-storage
        /// block at the selected cell (or directly above it) receives the item there; otherwise
        /// the item creates new storage in the cell above an upward-facing ground click whose
        /// block can attach the storage and whose target cell is replaceable. Returns null when
        /// no placement is available.
        /// </summary>
        public static BlockPos? GetGroundStoragePlacementPosition(IBlockAccessor? blockAccessor, BlockSelection? selection)
        {
            if (blockAccessor == null || selection?.Position == null) return null;

            if (blockAccessor.GetBlockEntity(selection.Position) is BlockEntityGroundStorage)
                return selection.Position.Copy();

            var above = selection.Position.UpCopy(1);
            if (blockAccessor.GetBlockEntity(above) is BlockEntityGroundStorage)
                return above;

            if (selection.Face != BlockFacing.UP) return null;

            var groundStorage = blockAccessor.GetBlock(new AssetLocation("groundstorage"));
            var clickedBlock = blockAccessor.GetBlock(selection.Position);
            if (groundStorage == null
                || !clickedBlock.CanAttachBlockAt(blockAccessor, groundStorage, selection.Position, BlockFacing.UP, null))
                return null;

            if (blockAccessor.GetBlock(above).Replaceable < 6000) return null;

            return above;
        }

        public static TreeAttribute? GetContainerSlots(CarriedBlock? carried)
        {
            if (carried?.BlockEntityData == null)
            {
                return null;
            }

            var inventory = carried.BlockEntityData.GetTreeAttribute("inventory");
            if (inventory == null)
            {
                return null;
            }

            return inventory.GetTreeAttribute("slots") as TreeAttribute;
        }
    }
}