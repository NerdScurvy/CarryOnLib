
using System;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace CarryOn.API.Common.Models
{
  public class CarriedBlock
  {

    public CarrySlot Slot { get; }

    public ItemStack ItemStack { get; }
    public Block Block => ItemStack.Block;

    public ITreeAttribute BlockEntityData { get; }

    public CarriedBlock(CarrySlot slot, ItemStack stack, ITreeAttribute blockEntityData)
    {
      Slot = slot;
      ItemStack = stack ?? throw new ArgumentNullException(nameof(stack));
      BlockEntityData = blockEntityData;
    }
  }
}