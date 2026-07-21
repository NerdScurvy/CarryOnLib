namespace CarryOn.API.Common.Models
{
    public static class CarryConstants
    {
        public const string ModId = "carryon";

        public static string GetCarryCode(string key) => $"{ModId}:{key}";

        public static class FailureCodes
        {
            public const string Stop = "__stop__";
            public const string Default = "__default__";
            public const string Continue = "__continue__";
            public const string Ignore = "__ignore__";
        }

        public static class HotKeyCodes
        {
            public const string Pickup = "carryonpickupkey";
            public const string SwapBackModifier = "carryonswapbackmodifierkey";
            public const string Toggle = "carryontogglekey";
            public const string QuickDrop = "carryonquickdropkey";
            public const string QuickDropAll = "carryonquickdropallkey";
            public const string ToggleDoubleTapDismount = "carryontoggledoubletapdismountkey";
        }

        public static class AttributeKeys
        {
            public static class CarriedBlockData
            {
                public const string Stack = "Stack";
                public const string Data = "Data";
                public const string Children = "Children";
                public const string OffsetX = "OffsetX";
                public const string OffsetY = "OffsetY";
                public const string OffsetZ = "OffsetZ";
                public const string OriginalFace = "OriginalFace";
                public const string OriginalBlockCode = "OriginalBlockCode";
                public const string OriginalMeshAngle = "OriginalMeshAngle";
            }
        }
    }
}
