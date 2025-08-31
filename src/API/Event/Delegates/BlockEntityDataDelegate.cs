using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace CarryOn.API.Event.Delegates
{
    public delegate void BlockEntityDataDelegate(BlockEntity blockEntity, ITreeAttribute blockEntityData, bool dropped = false);
}