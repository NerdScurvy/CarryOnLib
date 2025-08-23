namespace CarryOn.API.Common
{
    public static class CarryCode
    {
        public static string ModId { get; } = "carryon";

        public static class CarryOnLib
        {
            public static string ModId { get; } = "carryonlib";
        }

        public static string CarryOnCode(string key) => $"{ModId}:{key}";

        public static class AttributeKey
        {
            public static string EntityLastSneakTap { get; } = CarryOnCode("LastSneakTapMs");

            public static class Watched
            {
                public static string EntityCarried { get; } = CarryOnCode("Carried");
                public static string EntityDoubleTapDismountEnabled { get; } = CarryOnCode("DoubleTapDismountEnabled");

            }
        }

        public static class FailureCode
        {
            public static string Continue { get; } = "__continue__";
            public static string Stop { get; } = "__stop__";
            public static string Default { get; } = "__default__";
            public static string Internal { get; } = "__failure__";
        }

        public static class HotKeyCode
        {
            public static string Pickup { get; } = "carryonpickupkey";
            public static string SwapBackModifier { get; } = "carryonswapbackmodifierkey";
            public static string Toggle { get; } = "carryontogglekey";
            public static string QuickDrop { get; } = "carryonquickdropkey";
            public static string ToggleDoubleTapDismount { get; } = "carryontoggledoubletapdismountkey";

        }

    }
}