using System;

namespace CarryOn.API.Common.Models
{
    [Flags]
    public enum CarryHintType
    {
        None = 0,
        BasePickup = 1,
        ForcePickup = 2,
        TransferPut = 4,
        TransferTake = 8,
    }
}