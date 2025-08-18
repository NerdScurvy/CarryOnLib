using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace CarryOn.API.Common
{
    public interface ICarryableTransfer
    {
        bool CanPutCarryable(IPlayer player, BlockEntity blockEntity, int index, ItemStack itemStack, TreeAttribute blockEntityData, out string failureCode, out string onScreenErrorMessage);

        bool CanTakeCarryable(IPlayer player, BlockEntity blockEntity, int index, out string failureCode, out string onScreenErrorMessage);

        bool TryPutCarryable(IPlayer player, BlockEntity blockEntity, int index, ItemStack itemstack, TreeAttribute blockEntityData, out string failureCode, out string onScreenErrorMessage);

        bool TryTakeCarryable(IPlayer player, BlockEntity blockEntity, int index, out ItemStack itemstack, out TreeAttribute blockEntityData, out string failureCode, out string onScreenErrorMessage);

        bool BlockCarryAllowed(IPlayer player, BlockSelection selection);
    }
}