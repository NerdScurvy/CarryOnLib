using System;
using System.Collections.Generic;
using CarryOn.API.Common.Models;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using static CarryOn.API.Common.Models.CarryConstants;

namespace CarryOn.Utility
{
    public static class CarriedBlockTreeSerializer
    {
        public static ITreeAttribute? Serialize(CarriedBlock carriedBlock)
        {
            var tree = new TreeAttribute();

            var stack = carriedBlock.ItemStack;

            tree.SetItemstack(AttributeKeys.CarriedBlockData.Stack, stack);

            if (carriedBlock.BlockEntityData != null)
            {
                tree[AttributeKeys.CarriedBlockData.Data] = carriedBlock.BlockEntityData;
            }

            var childrenTree = BuildAttachedBlocksTree(carriedBlock.AttachedBlocks);
            if (childrenTree != null)
                tree[AttributeKeys.CarriedBlockData.Children] = childrenTree;

            if (carriedBlock.OriginalBlockCode != null)
                tree.SetString(AttributeKeys.CarriedBlockData.OriginalBlockCode, carriedBlock.OriginalBlockCode.ToString());

            if (stack.Class == EnumItemClass.Item && stack.Collectible?.Code != null)
                tree.SetString(AttributeKeys.CarriedBlockData.OriginalItemCode, stack.Collectible.Code.ToString());

            if (carriedBlock.OriginalMeshAngle.HasValue)
                tree.SetFloat(AttributeKeys.CarriedBlockData.OriginalMeshAngle, carriedBlock.OriginalMeshAngle.Value);

            if (carriedBlock.SpawnerFactoryCode != null)
                tree.SetString(AttributeKeys.CarriedBlockData.SpawnerFactoryCode, carriedBlock.SpawnerFactoryCode);

            if (carriedBlock.SpawnAsEntityOnPlacement.HasValue)
                tree.SetBool(AttributeKeys.CarriedBlockData.SpawnAsEntityOnPlacement, carriedBlock.SpawnAsEntityOnPlacement.Value);

            return tree;
        }

        /// <summary>
        /// Backward-compatible read: previously this collapsed "no data", "unresolvable
        /// block" and "exception" into a single null. That distinction is now exposed by
        /// <see cref="DeserializeSafe"/>. Null is returned for both vacant slots and
        /// failed reads so existing callers keep their (non-destructive) semantics.
        /// </summary>
        public static CarriedBlock? Deserialize(ITreeAttribute tree, ICoreAPI? api, CarrySlot slot = CarrySlot.Hands)
            => DeserializeSafe(tree, api, slot).Carried;

        /// <summary>
        /// Backward-compatible read; see the <see cref="ICoreAPI"/> overload.
        /// </summary>
        public static CarriedBlock? Deserialize(ITreeAttribute tree, IWorldAccessor? world, CarrySlot slot = CarrySlot.Hands)
            => DeserializeSafe(tree, world, slot).Carried;

        /// <summary>
        /// Protected read of a carried block tree. Never throws; resolves the carried
        /// stack from its stored id (falling back to the stored block code when that id
        /// cannot be resolved), and reports <see cref="CarriedBlockReadStatus.Failed"/>
        /// for present-but-unusable data instead of silently treating it as empty.
        /// </summary>
        /// <param name="tree">The carried slot attribute tree.</param>
        /// <param name="api">API used to resolve the world for id and code lookup.</param>
        /// <param name="slot">The carry slot being read.</param>
        public static CarriedBlockReadResult DeserializeSafe(ITreeAttribute tree, ICoreAPI? api, CarrySlot slot = CarrySlot.Hands)
        {
            if (api == null) return CarriedBlockReadResult.Failed("no-api");
            return DeserializeSafe(tree, api.World, slot);
        }

        /// <summary>
        /// Protected read of a carried block tree; see the <see cref="ICoreAPI"/> overload.
        /// </summary>
        /// <param name="tree">The carried slot attribute tree.</param>
        /// <param name="world">The world used for id and code resolution.</param>
        /// <param name="slot">The carry slot being read.</param>
        public static CarriedBlockReadResult DeserializeSafe(ITreeAttribute tree, IWorldAccessor? world, CarrySlot slot = CarrySlot.Hands)
        {
            if (tree == null) return CarriedBlockReadResult.Empty("no-tree");
            if (world == null) return CarriedBlockReadResult.Failed("world-unavailable");

            try
            {
                var stack = tree.GetItemstack(AttributeKeys.CarriedBlockData.Stack);
                if (stack == null) return CarriedBlockReadResult.Empty("no-stack");

                // Item-class carries (e.g. a backpack / hand basket picked up from ground storage)
                // never produce a Block; their contents live in the stack's own attributes, so the
                // block entity data, attached children and block-specific fallbacks do not apply.
                if (stack.Class == EnumItemClass.Item)
                {
                    stack.ResolveBlockOrItem(world);
                    if (stack.Collectible == null)
                    {
                        var item = ResolveItemByStoredCode(world, tree);
                        if (item == null)
                            return CarriedBlockReadResult.Failed("unresolvable-item-id");

                        var oldAttributes = stack.Attributes;
                        stack = new ItemStack(item, stack.StackSize);
                        if (oldAttributes != null && oldAttributes.Count > 0)
                        {
                            // keep user/attach attributes carried on the stack
                            stack.Attributes = oldAttributes;
                        }
                    }

                    return CarriedBlockReadResult.Ok(new CarriedBlock(slot, stack, null));
                }

                var block = stack.Block;
                if (block == null)
                {
                    stack.ResolveBlockOrItem(world);
                    block = stack.Block;
                }

                if (block == null)
                {
                    // Direct id resolution failed: the stored id is unresolved against the
                    // current registry. The engine's ItemStack.FixMapping() would translate
                    // old→new ids, but its old→new maps come from the save's stored id table,
                    // which is not exposed to mods (ServerSystemBlockIdRemapper is instance-only,
                    // no public accessor on ICoreServerAPI, and WorldConfiguration holds no id
                    // table). The carry tree however stores the block code itself
                    // (OriginalBlockCode / legacy Data.blockCode), so re-resolving from it is
                    // exactly the outcome FixMapping would produce - without needing the
                    // unreachable maps.
                    block = ResolveByStoredCode(world, tree);

                    if (block == null)
                        return CarriedBlockReadResult.Failed("unresolvable-block-id");

                    var oldAttributes = stack.Attributes;
                    stack = new ItemStack(block, stack.StackSize);
                    if (oldAttributes != null && oldAttributes.Count > 0)
                    {
                        // keep user/attach attributes carried on the stack
                        stack.Attributes = oldAttributes;
                    }
                }

                // A block whose code is no longer registered resolves to VS's missing-block
                // placeholder (CollectibleObject.IsMissing): it keeps the stored code but has no
                // registration, shape or texture. It must never be placed as a missing-texture
                // block nor silently dropped - treat it exactly like an unresolvable block.
                if (block.IsMissing)
                    return CarriedBlockReadResult.Failed("missing-block");

                var blockEntityData = tree[AttributeKeys.CarriedBlockData.Data] as ITreeAttribute;
                var attachedBlocks = DeserializeAttachedBlocks(tree, world);

                var originalCodeStr = tree.GetString(AttributeKeys.CarriedBlockData.OriginalBlockCode, null);
                var originalCode = originalCodeStr != null ? new AssetLocation(originalCodeStr) : null;

                float? originalMeshAngle = null;
                if (tree.HasAttribute(AttributeKeys.CarriedBlockData.OriginalMeshAngle))
                    originalMeshAngle = tree.GetFloat(AttributeKeys.CarriedBlockData.OriginalMeshAngle);

                var spawnerFactoryCodeStr = tree.GetString(AttributeKeys.CarriedBlockData.SpawnerFactoryCode, null);

                bool? spawnAsEntityOnPlacement = null;
                if (tree.HasAttribute(AttributeKeys.CarriedBlockData.SpawnAsEntityOnPlacement))
                    spawnAsEntityOnPlacement = tree.GetBool(AttributeKeys.CarriedBlockData.SpawnAsEntityOnPlacement);

                var carriedBlock = new CarriedBlock(slot, stack, blockEntityData, attachedBlocks, originalCode, originalMeshAngle, spawnerFactoryCodeStr, spawnAsEntityOnPlacement);

                return CarriedBlockReadResult.Ok(carriedBlock);
            }
            catch (Exception ex)
            {
                return CarriedBlockReadResult.Failed("deserialize-exception", ex);
            }
        }

        /// <summary>
        /// Re-resolves the carried block from the code stored on the tree -
        /// <c>OriginalBlockCode</c> (2.0) or, for legacy 1.14 trees, <c>Data.blockCode</c>.
        /// Only used when the stored numeric id cannot be resolved at all; block ids are
        /// stable for a given save, so a resolvable id is trusted as-is and this is the
        /// recovery path for the unreachable-engine-map case.
        /// </summary>
        private static Block? ResolveByStoredCode(IWorldAccessor world, ITreeAttribute tree)
        {
            var code = GetStoredBlockCode(tree);
            if (code == null) return null;

            return world.GetBlock(code);
        }

        private static CollectibleObject? ResolveItemByStoredCode(IWorldAccessor world, ITreeAttribute tree)
        {
            var code = tree.GetString(AttributeKeys.CarriedBlockData.OriginalItemCode, null);
            if (code == null) return null;

            return world.GetItem(new AssetLocation(code));
        }

        private static AssetLocation? GetStoredBlockCode(ITreeAttribute tree)
        {
            var originalCodeStr = tree.GetString(AttributeKeys.CarriedBlockData.OriginalBlockCode, null);
            if (originalCodeStr != null)
                return new AssetLocation(originalCodeStr);

            var data = tree[AttributeKeys.CarriedBlockData.Data] as ITreeAttribute;
            var blockCodeStr = data?.GetString("blockCode", null);
            if (blockCodeStr != null)
                return new AssetLocation(blockCodeStr);

            return null;
        }

        public static List<AttachedCarriedBlock>? DeserializeAttachedBlocks(ITreeAttribute slotAttribute, ICoreAPI? api)
            => DeserializeAttachedBlocks(slotAttribute, api?.World);

        public static List<AttachedCarriedBlock>? DeserializeAttachedBlocks(ITreeAttribute slotAttribute, IWorldAccessor? world)
        {
            if (slotAttribute[AttributeKeys.CarriedBlockData.Children] is not TreeAttribute childrenTree || childrenTree.Count == 0)
                return null;

            var attached = new List<AttachedCarriedBlock>();
            foreach (var key in childrenTree.Keys)
            {
                try
                {
                    if (childrenTree[key] is not ITreeAttribute childAttr) continue;

                    var childStack = childAttr.GetItemstack(AttributeKeys.CarriedBlockData.Stack);
                    if (childStack?.Class != EnumItemClass.Block) continue;
                    if (childStack.Block == null)
                    {
                        if (world == null) continue;
                        childStack.ResolveBlockOrItem(world);
                        if (childStack.Block == null)
                        {
                            world.Logger.Debug($"carryon: skipping attached block child '{key}' - block id could not be resolved.");
                            continue;
                        }
                    }

                    // Missing-block placeholder (removed/disabled mod): skip the child rather than
                    // attaching an unregistered, textureless block to a placed carried tree.
                    if (childStack.Block.IsMissing)
                    {
                        world?.Logger.Debug($"carryon: skipping attached block child '{key}' - block '{childStack.Block.Code}' is not registered (missing-block placeholder); use a mod block that is still loaded.");
                        continue;
                    }

                    var childData = childAttr[AttributeKeys.CarriedBlockData.Data] as ITreeAttribute;

                    var offsetX = childAttr.GetInt(AttributeKeys.CarriedBlockData.OffsetX, 0);
                    var offsetY = childAttr.GetInt(AttributeKeys.CarriedBlockData.OffsetY, 0);
                    var offsetZ = childAttr.GetInt(AttributeKeys.CarriedBlockData.OffsetZ, 0);
                    var relativeOffset = new BlockPos(offsetX, offsetY, offsetZ);

                    var faceCode = childAttr.GetString(AttributeKeys.CarriedBlockData.OriginalFace, null);
                    var originalFace = faceCode != null ? BlockFacing.FromCode(faceCode) : null;

                    var originalCodeStr = childAttr.GetString(AttributeKeys.CarriedBlockData.OriginalBlockCode, null);
                    var originalCode = originalCodeStr != null ? new AssetLocation(originalCodeStr) : null;

                    float? originalMeshAngle = null;
                    if (childAttr.HasAttribute(AttributeKeys.CarriedBlockData.OriginalMeshAngle))
                        originalMeshAngle = childAttr.GetFloat(AttributeKeys.CarriedBlockData.OriginalMeshAngle);

                    var carriedBlock = new CarriedBlock(CarrySlot.Attached, childStack, childData, null, originalCode, originalMeshAngle);
                    attached.Add(new AttachedCarriedBlock(relativeOffset, carriedBlock, originalFace));
                }
                catch (Exception ex)
                {
                    world?.Logger.Debug($"carryon: error reading attached block child '{key}': {ex.Message}");
                }
            }

            return attached.Count > 0 ? attached : null;
        }

        public static TreeAttribute? BuildAttachedBlocksTree(IReadOnlyList<AttachedCarriedBlock>? attachedBlocks)
        {
            if (attachedBlocks == null || attachedBlocks.Count == 0) return null;

            var childrenTree = new TreeAttribute();
            for (int i = 0; i < attachedBlocks.Count; i++)
            {
                var child = attachedBlocks[i];
                var childAttr = new TreeAttribute();

                childAttr.SetItemstack(AttributeKeys.CarriedBlockData.Stack, child.ItemStack);

                if (child.BlockEntityData != null)
                {
                    childAttr[AttributeKeys.CarriedBlockData.Data] = child.BlockEntityData;
                }

                childAttr.SetInt(AttributeKeys.CarriedBlockData.OffsetX, child.RelativeOffset.X);
                childAttr.SetInt(AttributeKeys.CarriedBlockData.OffsetY, child.RelativeOffset.Y);
                childAttr.SetInt(AttributeKeys.CarriedBlockData.OffsetZ, child.RelativeOffset.Z);

                if (child.OriginalLocalFace != null)
                    childAttr.SetString(AttributeKeys.CarriedBlockData.OriginalFace, child.OriginalLocalFace.Code);

                if (child.OriginalBlockCode != null)
                    childAttr.SetString(AttributeKeys.CarriedBlockData.OriginalBlockCode, child.OriginalBlockCode.ToString());

                if (child.OriginalMeshAngle.HasValue)
                    childAttr.SetFloat(AttributeKeys.CarriedBlockData.OriginalMeshAngle, child.OriginalMeshAngle.Value);

                childrenTree[$"child_{i}"] = childAttr;
            }

            return childrenTree;
        }
    }
}
