using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace CarryOn.Utility
{
    public class CarryHelper
    {
        /// <summary> 
        /// Returns the position that the specified block would
        /// be placed at for the specified block selection.
        /// </summary>
        public static BlockPos GetPlacedPosition(IWorldAccessor world, BlockSelection selection, Block block)
        {
            if (selection == null) return null;
            var position = selection.Position.Copy();
            var clickedBlock = world.BlockAccessor.GetBlock(position);
            if (!clickedBlock.IsReplacableBy(block))
            {
                position.Offset(selection.Face);
                var replacedBlock = world.BlockAccessor.GetBlock(position);
                if (!replacedBlock.IsReplacableBy(block)) return null;
            }
            return position;
        }
      
    }
}