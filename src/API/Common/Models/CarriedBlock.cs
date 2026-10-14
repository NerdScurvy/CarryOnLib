using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace CarryOn.API.Common.Models
{
    public class CarriedBlock
    {

        public CarrySlot Slot { get; }

        public ItemStack ItemStack { get; }
        public Block Block => ItemStack.Block;

        public ITreeAttribute? BlockEntityData { get; }

        public IReadOnlyList<AttachedCarriedBlock>? AttachedBlocks { get; }

        public bool HasAttachedBlocks => AttachedBlocks is { Count: > 0 };

        /// <summary>
        /// The original world block code at pickup time, before <see cref="Block.OnPickBlock"/>
        /// normalization. Preserves the facing variant (e.g. "chest-east") that gets lost when
        /// <c>OnPickBlock</c> returns a generic/north-normalized variant.
        /// </summary>
        public AssetLocation? OriginalBlockCode { get; }

        /// <summary>
        /// The original meshAngle from the block entity at pickup time, before it gets stripped
        /// during OnPickBlock processing. Used to detect rotation for blocks like chests whose
        /// facing is determined by meshAngle rather than block code variants.
        /// </summary>
        public float? OriginalMeshAngle { get; }

        /// <summary>
        /// The factory code of the <see cref="CarryOn.API.Common.Interfaces.ICarryEntitySpawnerFactory"/>
        /// that should handle spawning the entity when this carried block is placed or dropped.
        /// Populated during pickup (entity to carried block) by the pickup handler's
        /// <see cref="CarryOn.API.Common.Interfaces.ICarryableEntityPickupHandler.SerializeToCarriedBlock"/>
        /// method. Null when the carried block was picked up from the world (not from an entity),
        /// meaning it will be placed as a normal block.
        /// </summary>
        public string? SpawnerFactoryCode { get; }

        /// <summary>
        /// Signal decoupling "this block has a factory code" from "this block should re-spawn
        /// as an entity when placed by hand (<c>dropped == false</c>)". Set by pickup handlers
        /// for genuinely custom entity blocks (racks, boats, ...) that must round-trip through
        /// their entity factory on placement. Null for normal blocks picked up from the world;
        /// the placement pipeline resolves null against the CarryOn config default. Persisted
        /// with the carried block so it survives the drop → <c>EntityCarriedBlock</c> → pickup
        /// round trip.
        /// </summary>
        public bool? SpawnAsEntityOnPlacement { get; }

        public CarriedBlock(CarrySlot slot, ItemStack stack, ITreeAttribute? blockEntityData)
          : this(slot, stack, blockEntityData, null, null, null, null)
        {
        }

        public CarriedBlock(CarrySlot slot, ItemStack stack, ITreeAttribute? blockEntityData, IEnumerable<AttachedCarriedBlock>? attachedBlocks)
          : this(slot, stack, blockEntityData, attachedBlocks, null, null, null)
        {
        }

        public CarriedBlock(CarrySlot slot, ItemStack stack, ITreeAttribute? blockEntityData, IEnumerable<AttachedCarriedBlock>? attachedBlocks, AssetLocation? originalBlockCode)
          : this(slot, stack, blockEntityData, attachedBlocks, originalBlockCode, null, null)
        {
        }

        public CarriedBlock(CarrySlot slot, ItemStack stack, ITreeAttribute? blockEntityData, IEnumerable<AttachedCarriedBlock>? attachedBlocks, AssetLocation? originalBlockCode, float? originalMeshAngle)
          : this(slot, stack, blockEntityData, attachedBlocks, originalBlockCode, originalMeshAngle, null)
        {
        }

        public CarriedBlock(CarrySlot slot, ItemStack stack, ITreeAttribute? blockEntityData, IEnumerable<AttachedCarriedBlock>? attachedBlocks, AssetLocation? originalBlockCode, float? originalMeshAngle, string? spawnerFactoryCode)
          : this(slot, stack, blockEntityData, attachedBlocks, originalBlockCode, originalMeshAngle, spawnerFactoryCode, null)
        {
        }

        public CarriedBlock(CarrySlot slot, ItemStack stack, ITreeAttribute? blockEntityData, IEnumerable<AttachedCarriedBlock>? attachedBlocks, AssetLocation? originalBlockCode, float? originalMeshAngle, string? spawnerFactoryCode, bool? spawnAsEntityOnPlacement)
        {
            Slot = slot;
            ItemStack = stack ?? throw new ArgumentNullException(nameof(stack));
            BlockEntityData = blockEntityData;
            AttachedBlocks = attachedBlocks?.ToList().AsReadOnly();
            OriginalBlockCode = originalBlockCode;
            OriginalMeshAngle = originalMeshAngle;
            SpawnerFactoryCode = spawnerFactoryCode;
            SpawnAsEntityOnPlacement = spawnAsEntityOnPlacement;
        }
    }
}
