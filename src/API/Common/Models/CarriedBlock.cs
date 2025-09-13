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

    // Optional client-side label data (synced subset when full BlockEntityData not available)
    public string LabelText { get; }
    public int? LabelColor { get; }
    public float? LabelFontSize { get; }
    public CarriedBlock(CarrySlot slot, ItemStack stack, ITreeAttribute blockEntityData,
                        string labelText = null, int? labelColor = null, float? labelFontSize = null)
    {
      Slot = slot;
      ItemStack = stack ?? throw new ArgumentNullException(nameof(stack));
      BlockEntityData = blockEntityData;
      LabelText = labelText;
      LabelColor = labelColor;
      LabelFontSize = labelFontSize;
    }
  }
}