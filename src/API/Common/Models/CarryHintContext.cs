using Vintagestory.API.Common;

namespace CarryOn.API.Common.Models
{
    public class CarryHintContext
    {
        public IPlayer? Player { get; set; }

        public BlockSelection? Selection { get; set; }

        public BlockEntity? BlockEntity { get; set; }

        public int SelectionBoxIndex { get; set; } = -1;

        public bool IsTargetCarryable { get; set; }

        public bool IsForcePickupEnabled { get; set; }

        public bool CanDoCarryAction { get; set; }

        public bool IsCarryingInHands { get; set; }

        public bool? IsTargetSlotEmpty { get; set; }

        public bool? CanTransferPut { get; set; }

        public bool? CanTransferTake { get; set; }
    }
}