using Vintagestory.API.Client;
using Vintagestory.API.Util;
using static CarryOn.API.Common.Models.CarryConstant;

namespace CarryOn.Utility
{
    public static class CarryInputExtensions
    {
        public static bool IsCarryKeyPressed(this IInputAPI input, bool checkMouse = false)
        {
            if (checkMouse && !input.InWorldMouseButton.Right) return false;

            var hotKey = input.HotKeys.Get(HotKeyCode.Pickup);
            if (hotKey?.CurrentMapping == null) return false;

            return input.KeyboardKeyState[hotKey.CurrentMapping.KeyCode];
        }

        public static bool IsCarrySwapBackKeyPressed(this IInputAPI input)
        {
            var hotKey = input.HotKeys.Get(HotKeyCode.SwapBackModifier);
            if (hotKey?.CurrentMapping == null) return false;

            return input.KeyboardKeyState[hotKey.CurrentMapping.KeyCode];
        }
    }
}
