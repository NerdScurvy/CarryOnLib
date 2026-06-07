using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace CarryOn.API.Common.Models
{
  public class AttachedCarriedBlock
  {
    public BlockPos RelativeOffset { get; }
    public CarriedBlock CarriedBlock { get; }
    public BlockFacing? OriginalLocalFace { get; }

    // Convenience accessors
    public ItemStack ItemStack => CarriedBlock.ItemStack;
    public Block Block => CarriedBlock.Block;
    public ITreeAttribute? BlockEntityData => CarriedBlock.BlockEntityData;
    public AssetLocation? OriginalBlockCode => CarriedBlock.OriginalBlockCode;
    public float? OriginalMeshAngle => CarriedBlock.OriginalMeshAngle;

    public AttachedCarriedBlock(BlockPos relativeOffset, CarriedBlock carriedBlock, BlockFacing? originalLocalFace)
    {
      RelativeOffset = relativeOffset ?? new BlockPos(0, 0, 0);
      CarriedBlock = carriedBlock ?? throw new System.ArgumentNullException(nameof(carriedBlock));
      OriginalLocalFace = originalLocalFace;
    }
  }
}
