using CarryOn.API.Common.Models;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace CarryOn.API.Event.Delegates
{
    // null = no opinion/continue, true = allow, false = deny
    public delegate void BeforePickUpBlockDelegate(
        Entity entity,
        BlockPos pos,
        CarrySlot slot,
        CarriedBlock carriedBlock,
        out bool? canPickUp,
        out string failureCode
    );
}