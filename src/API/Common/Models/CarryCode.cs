using Vintagestory.API.Client;

namespace CarryOn.API.Common.Models
{
    public static class CarryCode
    {
        public static string ModId { get; } = "carryon";

        public const string ConfigFile = "CarryOnConfig.json";

        public static class CarryOnLib
        {
            public static string ModId { get; } = "carryonlib";
        }

        public static string CarryOnCode(string key) => $"{ModId}:{key}";

        public static class Default
        {
            public static float PlaceSpeed { get; } = 0.75f;
            public static float SwapSpeed { get; } = 1.5f;
            public static float PickUpSpeed { get; } = 0.8f;
            public static float TransferSpeed { get; } = 0.8f;
            public static float InteractSpeed { get; } = 0.8f;
            public static int MaxInteractionDistance { get; } = 6;
            public static int HotbarSize { get; } = 10;
            public static int DoubleTapThresholdMs { get; } = 500;
            public static GlKeys PickupKeybind { get; } = GlKeys.ShiftLeft;
            public static GlKeys SwapBackModifierKeybind { get; } = GlKeys.ControlLeft;
            public static GlKeys FunctionKeybind { get; } = GlKeys.K;

        }

        public static class AttributeKey
        {
            public static string EntityLastSneakTap { get; } = CarryOnCode("LastSneakTapMs");

            public static string CarriedRevision { get; } = "Rev";

            public static class Watched
            {
                public static string EntityCarried { get; } = CarryOnCode("Carried");

                public static string EntityDoubleTapDismountEnabled { get; } = CarryOnCode("DoubleTapDismountEnabled");

            }
        }

        public static class ConfigKey
        {
            //ConfigVersion
            public static string ConfigVersionKey { get; } = "ConfigVersion";

            public static string CarryablesKey = "Carryables";
            public static string CarryablesOnBackKey = "CarryablesOnBack";
            public static string InteractablesKey = "Interactables";
            public static string CarryOptionsKey = "CarryOptions";
            public static string DebuggingOptionsKey = "DebuggingOptions";
            public static string CarryableFiltersKey = "CarryableFilters";

            public static class CarryableFilters
            {
                public static string AutoMapSimilarKey { get; } = "AutoMapSimilar";
                public static string AutoMatchIgnoreModsKey { get; } = "AutoMatchIgnoreMods";
                public static string AllowedShapeOnlyMatchesKey { get; } = "AllowedShapeOnlyMatches";
                public static string RemoveBaseCarryableBehaviourKey { get; } = "RemoveBaseCarryableBehaviour";
                public static string RemoveCarryableBehaviourKey { get; } = "RemoveCarryableBehaviour";
            }

            public static class DebuggingOptions
            {
                public static string LoggingEnabledKey { get; } = "LoggingEnabled";
                public static string DisableHarmonyPatchKey { get; } = "DisableHarmonyPatch";
                public static string EnablePackAdjustmentToolKey { get; } = "EnablePackAdjustmentTool";
            }

            public static class CarryOptions
            {
                public static string BackSlotEnabledKey { get; } = "BackSlotEnabled";
                public static string AllowChestTrunksOnBackKey { get; } = "AllowChestTrunksOnBack";
                public static string AllowHighCapacityStorageOnBackKey { get; } = "AllowHighCapacityStorageOnBack";
                public static string AllowCratesOnBackKey { get; } = "AllowCratesOnBack";
                public static string AllowSprintWhileCarryingKey { get; } = "AllowSprintWhileCarrying";
                public static string IgnoreCarrySpeedPenaltyKey { get; } = "IgnoreCarrySpeedPenalty";
                public static string RemoveInteractDelayWhileCarryingKey { get; } = "RemoveInteractDelayWhileCarrying";
                public static string InteractSpeedMultiplierKey { get; } = "InteractSpeedMultiplier";
                public static string MaxInteractionDistanceKey { get; } = "MaxInteractionDistance";
                public static string PreventSwapFromBackOnTargetKey { get; } = "PreventSwapFromBackOnTarget";
                public static string WalkSpeedOverridesKey { get; } = "WalkSpeedOverrides";
                public static string WalkSpeedOverridesByBlockCodeKey { get; } = "ByBlockCode";
                public static string WalkSpeedOverridesByBlockClassKey { get; } = "ByBlockClass";
                public static string WalkSpeedOverridesSlotDefaultsKey { get; } = "SlotDefaults";

                public static string TooHotToCarryKey { get; } = "TooHotToCarry";
                public static string TooHotToCarryTemperatureKey { get; } = "TooHotToCarryTemperature";

                public static string CarryAttachedWallSignsKey { get; } = "CarryAttachedWallSigns";

                public static string BackpackSelectionModeKey { get; } = "BackpackSelectionMode";

            }

            public static class DropOnDamage
            {
                public static string EnabledKey { get; } = "Enabled";
                public static string DamageThresholdKey { get; } = "DamageThreshold";
            }

            public static class Interactables
            {
                public static string DoorKey { get; } = "Door";
                public static string BarrelKey { get; } = "Barrel";
                public static string StorageKey { get; } = "Storage";
            }

            public static class Carryables
            {
                public static string AnvilKey { get; } = "Anvil";
                public static string BarrelKey { get; } = "Barrel";
                public static string BookshelfKey { get; } = "Bookshelf";
                public static string BunchOCandlesKey { get; } = "BunchOCandles";
                public static string CabinetKey { get; } = "Cabinet";
                public static string ChandelierKey { get; } = "Chandelier";
                public static string ChestTrunkKey { get; } = "ChestTrunk";
                public static string ChestKey { get; } = "Chest";
                public static string ClutterKey { get; } = "Clutter";
                public static string CrateKey { get; } = "Crate";
                public static string DisplayCaseKey { get; } = "DisplayCase";
                public static string FlowerpotKey { get; } = "Flowerpot";
                public static string ForgeKey { get; } = "Forge";
                public static string HenboxKey { get; } = "Henbox";
                public static string LogWithResinKey { get; } = "LogWithResin";
                public static string LootVesselKey { get; } = "LootVessel";
                public static string MoldRackKey { get; } = "MoldRack";
                public static string MoldKey { get; } = "Mold";
                public static string OvenKey { get; } = "Oven";
                public static string PlanterKey { get; } = "Planter";
                public static string QuernKey { get; } = "Quern";
                public static string ReedChestKey { get; } = "ReedChest";
                public static string ResonatorKey { get; } = "Resonator";
                public static string ShelfKey { get; } = "Shelf";
                public static string SignKey { get; } = "Sign";
                public static string StorageVesselKey { get; } = "StorageVessel";
                public static string ToolRackKey { get; } = "ToolRack";
                public static string TorchHolderKey { get; } = "TorchHolder";
            }
        }

        public static class FailureCode
        {
            public static string Continue { get; } = "__continue__";
            public static string Stop { get; } = "__stop__";
            public static string Default { get; } = "__default__";
            public static string Internal { get; } = "__failure__";
            public static string Ignore { get; } = "__ignore__";

            // Kept as the historical wire value for compatibility.
            public static string RequiresOwnership { get; } = "requiresownership";
            public static string EntityNotFound { get; } = "entity-not-found";
            public static string EntityOutOfReach { get; } = "entity-out-of-reach";
            public static string SlotNotEmpty { get; } = "slot-not-empty";
            public static string SlotEmpty { get; } = "slot-empty";

            // Carry placement failure codes
            public static string AlreadyCarrying { get; } = "already-carrying";
            public static string NoPermission { get; } = "no-permission";
            public static string NotCarryable { get; } = "not-carryable";
            public static string NotCarrying { get; } = "not-carrying";

            // Carry attachment failure codes
            public static string SlotNotFound { get; } = "slot-not-found";
            public static string SlotDataMissing { get; } = "slot-data-missing";
            public static string SlotIncompatibleBlock { get; } = "slot-incompatible-block";
            public static string SlotPreventAttaching { get; } = "slot-prevent-attaching";
            public static string BlockHasAttachedBlocks { get; } = "block-has-attached-blocks";
            public static string AttachUnavailable { get; } = "attach-unavailable";
            public static string AttachFailed { get; } = "attach-failed";
            public static string DetachUnavailable { get; } = "detach-unavailable";
            public static string SlotNotCarryable { get; } = "slot-not-carryable";
            public static string SlotInventoryOpen { get; } = "slot-inventory-open";

            // Event-driven failure codes
            public static string TooHot { get; } = "too-hot";

            // Carry handler failure codes
            public static string InvalidData { get; } = "invalid-data";
            public static string CannotInteract { get; } = "cannot-interact";

            // Interaction logic failure codes
            public static string AttachableNotFound { get; } = "attachable-not-found";
            public static string CannotSwapBack { get; } = "cannot-swap-back";
            public static string NothingCarried { get; } = "nothing-carried";
            public static string PlaceDownNoPermission { get; } = "place-down-no-permission";

            // Default fallback codes
            public static string PickUpFailed { get; } = "pick-up-failed";
            public static string PlaceDownFailed { get; } = "place-down-failed";

            // Cluster carry failure codes
            public static string AttachedBlockNoClearance { get; } = "attached-block-no-clearance";
            public static string UnsupportedAttachment { get; } = "unsupported-attachment";
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

        public static class SoundPath
        {
            public static string DefaultPlace { get; } = "sounds/player/build";
            public static string DefaultBreak { get; } = "game:sounds/block/planks";
            public static string Throw { get; } = "sounds/player/throw";
        }

        public static string DefaultTransformGroup { get; } = "default";
        public static string FrontCarryAttachmentPoint { get; } = "carryon:FrontCarry";
        public static string WorldConfigPrefix { get; } = "carryon:";

    }
}