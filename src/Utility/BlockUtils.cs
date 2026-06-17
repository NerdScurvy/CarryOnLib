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
        /// <param name="pos"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
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
        /// Gets the origin block of a multiblock structure if the specified block is part of one.
        /// </summary>
        /// <param name="blockAccessor"></param>
        /// <param name="position"></param>
        /// <param name="block"></param>
        /// <returns>will return the origin block if it exists, otherwise the original block</returns>
        public static Block GetOriginBlockIfMultiBlock(IBlockAccessor blockAccessor, BlockPos position, Block block)
        {
            if (blockAccessor == null || position == null) return block;

            if (block is BlockMultiblock multiblock)
            {
                var multiPosition = position.Copy();
                multiPosition.Add(multiblock.OffsetInv);
                return blockAccessor.GetBlock(multiPosition);
            }
            return block;
        }

        /// <summary>
        /// Get the block position for the main block within for a multiblock structure
        /// </summary>
        public static BlockPos? GetMultiblockOrigin(BlockPos position, BlockMultiblock multiblock)
        {
            if (position == null) return null;

            if (multiblock != null)
            {
                var multiPosition = position.Copy();
                multiPosition.Add(multiblock.OffsetInv);
                return multiPosition;
            }
            return position;
        }

        /// <summary>
        /// Create a new block selection pointing to the main block within a multiblock structure
        /// </summary>
        public static BlockSelection? GetMultiblockOriginSelection(IBlockAccessor blockAccessor, BlockSelection? blockSelection)
        {
            if (blockSelection?.Block is BlockMultiblock multiblock)
            {
                var position = GetMultiblockOrigin(blockSelection.Position, multiblock);
                if (position == null) return null;
                var block = blockAccessor.GetBlock(position);
                var selection = blockSelection.Clone();
                selection.Position = position;
                selection.Block = block;

                return selection;
            }
            return blockSelection;
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