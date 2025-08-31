using Vintagestory.API.Common;

namespace CarryOn.API.Event.Delegates
{
    public delegate void ActionDelegate(EnumEntityAction action, ref EnumHandling handled);
}