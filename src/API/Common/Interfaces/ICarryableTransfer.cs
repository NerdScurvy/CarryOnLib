using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace CarryOn.API.Common.Interfaces
{
    /// <summary>
    /// Contract for block behaviors that support transferring carryable items between
    /// the player's carried state and a block entity's inventory (e.g., shelving, display cases).
    /// Implement this interface to opt in to the transfer interaction system.
    /// </summary>
    public interface ICarryableTransfer
    {
        bool IsTransferEnabled(ICoreAPI api);

        bool CanPutCarryable(IPlayer player, BlockEntity blockEntity, int index, ItemStack itemStack, ITreeAttribute? blockEntityData, out float? transferDelay, out string failureCode, out string onScreenErrorMessage);

        bool CanTakeCarryable(IPlayer player, BlockEntity blockEntity, int index, out float? transferDelay, out string failureCode, out string onScreenErrorMessage);

        bool TryPutCarryable(IPlayer player, BlockEntity blockEntity, int index, ItemStack itemstack, ITreeAttribute? blockEntityData, out string failureCode, out string onScreenErrorMessage);

        bool TryTakeCarryable(IPlayer player, BlockEntity blockEntity, int index, out ItemStack itemstack, out ITreeAttribute? blockEntityData, out string failureCode, out string onScreenErrorMessage);

        bool IsBlockCarryAllowed(IPlayer player, BlockSelection selection);

    }
}