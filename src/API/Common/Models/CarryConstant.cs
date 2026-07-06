namespace CarryOn.API.Common.Models
{
    public static class CarryConstant
    {
        public static string ModId { get; } = "carryon";

        public static string CarryOnCode(string key) => $"{ModId}:{key}";

        public static class FailureCode
        {
            public static string Stop { get; } = "__stop__";
            public static string Default { get; } = "__default__";
            public static string Continue { get; } = "__continue__";
            public static string Ignore { get; } = "__ignore__";
        }

        public static class HotKeyCode
        {
            public static string Pickup { get; } = "carryonpickupkey";
            public static string SwapBackModifier { get; } = "carryonswapbackmodifierkey";
            public static string Toggle { get; } = "carryontogglekey";
            public static string QuickDrop { get; } = "carryonquickdropkey";
            public static string QuickDropAll { get; } = "carryonquickdropallkey";
            public static string ToggleDoubleTapDismount { get; } = "carryontoggledoubletapdismountkey";            
        }

        public static class AttributeKey
        {
            public static class CarriedBlock
            {
                public static string Stack { get; } = "Stack";
                public static string Data { get; } = "Data";
                public static string Children { get; } = "Children";
                public static string OffsetX { get; } = "OffsetX";
                public static string OffsetY { get; } = "OffsetY";
                public static string OffsetZ { get; } = "OffsetZ";
                public static string OriginalFace { get; } = "OriginalFace";
                public static string OriginalBlockCode { get; } = "OriginalBlockCode";
                public static string OriginalMeshAngle { get; } = "OriginalMeshAngle";
            }
        }
    }
}
