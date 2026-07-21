using Vintagestory.API.Client;
using Vintagestory.API.Util;
using static CarryOn.API.Common.Models.CarryConstants;

namespace CarryOn.Utility
{
    /// <summary>
    /// Client-side helpers for checking carry key states via <see cref="IInputAPI"/>.
    /// These extensions depend on client-only APIs and are not usable on the server.
    /// </summary>
    public static class CarryInputExtensions
    {
        public static bool IsCarryKeyPressed(this IInputAPI input, bool checkMouse = false)
        {
            if (checkMouse && !input.InWorldMouseButton.Right) return false;

            var hotKey = input.HotKeys.Get(HotKeyCodes.Pickup);
            if (hotKey?.CurrentMapping == null) return false;

            return input.KeyboardKeyState[hotKey.CurrentMapping.KeyCode];
        }

        public static bool IsCarrySwapBackKeyPressed(this IInputAPI input)
        {
            var hotKey = input.HotKeys.Get(HotKeyCodes.SwapBackModifier);
            if (hotKey?.CurrentMapping == null) return false;

            return input.KeyboardKeyState[hotKey.CurrentMapping.KeyCode];
        }
    }
}
