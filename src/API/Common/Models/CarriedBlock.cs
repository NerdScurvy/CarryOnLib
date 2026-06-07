using System;
using System.Collections.Generic;
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

    public CarriedBlock(CarrySlot slot, ItemStack stack, ITreeAttribute? blockEntityData)
      : this(slot, stack, blockEntityData, null, null, null)
    {
    }

    public CarriedBlock(CarrySlot slot, ItemStack stack, ITreeAttribute? blockEntityData, List<AttachedCarriedBlock>? attachedBlocks)
      : this(slot, stack, blockEntityData, attachedBlocks, null, null)
    {
    }

    public CarriedBlock(CarrySlot slot, ItemStack stack, ITreeAttribute? blockEntityData, List<AttachedCarriedBlock>? attachedBlocks, AssetLocation? originalBlockCode)
      : this(slot, stack, blockEntityData, attachedBlocks, originalBlockCode, null)
    {
    }

    public CarriedBlock(CarrySlot slot, ItemStack stack, ITreeAttribute? blockEntityData, List<AttachedCarriedBlock>? attachedBlocks, AssetLocation? originalBlockCode, float? originalMeshAngle)
    {
      Slot = slot;
      ItemStack = stack ?? throw new ArgumentNullException(nameof(stack));
      BlockEntityData = blockEntityData;
      AttachedBlocks = attachedBlocks;
      OriginalBlockCode = originalBlockCode;
      OriginalMeshAngle = originalMeshAngle;
    }

  }
}