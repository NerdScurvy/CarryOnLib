using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace CarryOn.API.Common
{
  public class CarriedBlock
  {

    public CarrySlot Slot { get; }

    public ItemStack ItemStack { get; }
    public Block Block => ItemStack.Block;

    public ITreeAttribute BlockEntityData { get; }
  }
}