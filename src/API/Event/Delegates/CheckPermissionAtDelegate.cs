using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace CarryOn.API.Event.Delegates
{
    public delegate void CheckPermissionAtDelegate(EntityPlayer playerEntity, BlockPos pos, bool isReinforced, out bool? hasPermission);
}