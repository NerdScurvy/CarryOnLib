using System;
using System.Collections.Generic;
using CarryOn.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Datastructures;
using static CarryOn.API.Common.Models.CarryCode;

namespace CarryOn.API.Common.Models
{
    public class CarryablesConfig
    {
        [TreeValue("Anvil")] public bool Anvil { get; set; } = true;
        [TreeValue("Barrel")] public bool Barrel { get; set; } = true;
        [TreeValue("Bookshelf")] public bool Bookshelf { get; set; }
        [TreeValue("BunchOCandles")] public bool BunchOCandles { get; set; }
        [TreeValue("Cabinet")] public bool Cabinet { get; set; } = true;
        [TreeValue("Chandelier")] public bool Chandelier { get; set; }
        [TreeValue("ChestTrunk")] public bool ChestTrunk { get; set; }
        [TreeValue("Chest")] public bool Chest { get; set; } = true;
        [TreeValue("Clutter")] public bool Clutter { get; set; }
        [TreeValue("Crate")] public bool Crate { get; set; } = true;
        [TreeValue("DisplayCase")] public bool DisplayCase { get; set; }
        [TreeValue("Flowerpot")] public bool Flowerpot { get; set; } = true;
        [TreeValue("Forge")] public bool Forge { get; set; }
        [TreeValue("Henbox")] public bool Henbox { get; set; }
        [TreeValue("LogWithResin")] public bool LogWithResin { get; set; }
        [TreeValue("LootVessel")] public bool LootVessel { get; set; } = true;
        [TreeValue("MoldRack")] public bool MoldRack { get; set; }
        [TreeValue("Mold")] public bool Mold { get; set; }
        [TreeValue("Oven")] public bool Oven { get; set; }
        [TreeValue("Planter")] public bool Planter { get; set; } = true;
        [TreeValue("Quern")] public bool Quern { get; set; } = true;
        [TreeValue("ReedChest")] public bool ReedChest { get; set; } = true;
        [TreeValue("Resonator")] public bool Resonator { get; set; } = true;
        [TreeValue("Shelf")] public bool Shelf { get; set; }
        [TreeValue("Sign")] public bool Sign { get; set; }
        [TreeValue("StorageVessel")] public bool StorageVessel { get; set; } = true;
        [TreeValue("ToolRack")] public bool ToolRack { get; set; }
        [TreeValue("TorchHolder")] public bool TorchHolder { get; set; }
    }

    public class CarryablesOnBackConfig
    {
        [TreeValue("Barrel")] public bool Barrel { get; set; } = true;
        [TreeValue("ChestTrunk")] public bool ChestTrunk { get; set; }
        [TreeValue("Chest")] public bool Chest { get; set; } = true;
        [TreeValue("Crate")] public bool Crate { get; set; }
        [TreeValue("Flowerpot")] public bool Flowerpot { get; set; } = true;
        [TreeValue("LogWithResin")] public bool LogWithResin { get; set; }
        [TreeValue("LootVessel")] public bool LootVessel { get; set; } = true;
        [TreeValue("Planter")] public bool Planter { get; set; } = true;
        [TreeValue("ReedChest")] public bool ReedChest { get; set; } = true;
        [TreeValue("Resonator")] public bool Resonator { get; set; } = true;
        [TreeValue("StorageVessel")] public bool StorageVessel { get; set; } = true;
    }

    public class InteractablesConfig
    {
        [TreeValue("Door")] public bool Door { get; set; } = true;
        [TreeValue("Barrel")] public bool Barrel { get; set; } = true;
        [TreeValue("Storage")] public bool Storage { get; set; } = true;
    }

    public class CarryWalkSpeedConfig
    {
        [TreeValue("HandsEnabled")] public bool HandsEnabled { get; set; } = true;
        [TreeValue("BackEnabled")] public bool BackEnabled { get; set; } = true;
        [TreeValue("HandsAllowSprint")] public bool HandsAllowSprint { get; set; }
        [TreeValue("BackAllowSprint")] public bool BackAllowSprint { get; set; } = true;

        public ModifierOverridesConfig ModifierOverrides { get; set; } = new ModifierOverridesConfig();
    }

    public class CarryHungerRateConfig
    {
        [TreeValue("HandsEnabled")] public bool HandsEnabled { get; set; } = false;
        [TreeValue("BackEnabled")] public bool BackEnabled { get; set; } = true;
        [TreeValue("MinSaturationThreshold")] public float MinSaturationThreshold { get; set; } = Default.MinSaturationThreshold;

        public ModifierOverridesConfig ModifierOverrides { get; set; } = new ModifierOverridesConfig();
    }

    public class CarryablesFiltersConfig
    {
        [TreeValue("AutoMapSimilar")] public bool AutoMapSimilar { get; set; } = true;
        [TreeValue("AutoMatchIgnoreMods")] public string[] AutoMatchIgnoreMods { get; set; } = ["mcrate"];
        [TreeValue("AllowedShapeOnlyMatches")] public string[] AllowedShapeOnlyMatches { get; set; } = ["block/clay/lootvessel", "block/wood/chest/normal", "block/wood/trunk/normal", "block/reed/basket-normal"];
        [TreeValue("RemoveBaseCarryableBehaviour")] public string[] RemoveBaseCarryableBehaviour { get; set; } = ["woodchests:wtrunk"];
        [TreeValue("RemoveCarryableBehaviour")] public string[] RemoveCarryableBehaviour { get; set; } = ["game:banner", "game:clutter-devastation"];
    }

    public class SlotModifierConfig
    {
        public float? Hands { get; set; }
        public float? Back { get; set; }

        [JsonIgnore]
        public bool IsEmpty => Hands == null && Back == null;
    }

    public class ModifierOverridesConfig
    {
        public IDictionary<string, SlotModifierConfig> ByBlockCode { get; set; }
            = new Dictionary<string, SlotModifierConfig>();

        public IDictionary<string, SlotModifierConfig> ByBlockClass { get; set; }
            = new Dictionary<string, SlotModifierConfig>();

        public SlotModifierConfig SlotDefaults { get; set; } = new SlotModifierConfig();
    }

    public class DropCarriedOnDamageConfig
    {
        [TreeValue("Enabled")]
        public bool Enabled { get; set; } = true;

        [TreeValue("DamageThreshold")]
        public float DamageThreshold { get; set; } = 1.0f;

        [TreeValue("DropRange")]
        public int DropRange { get; set; } = 2;
    }

    public class CarryOptionsConfig
    {
        [TreeValue("RemoveInteractDelayWhileCarrying")]    public bool RemoveInteractDelayWhileCarrying { get; set; } = false;
        [TreeValue("InteractSpeedMultiplier")] public float InteractSpeedMultiplier { get; set; } = 1.0f;
        [TreeValue("MaxInteractionDistance")] public int MaxInteractionDistance { get; set; } = Default.MaxInteractionDistance;
        [TreeValue("BackSlotEnabled")] public bool BackSlotEnabled { get; set; } = true;
        [TreeValue("AllowHighCapacityStorageOnBack")] public bool AllowHighCapacityStorageOnBack { get; set; } = false;
        [TreeValue("PreventSwapFromBackOnTarget")] public string[] PreventSwapFromBackOnTarget { get; set; } = ["behavior::Container", "behavior::Door", "class::portals.portal", "code::groundstorage", "class::BlockGroundStorage"];
        [TreeValue("TooHotToCarry")] public bool TooHotToCarry { get; set; } = true;
        [TreeValue("TooHotToCarryTemperature")] public int TooHotToCarryTemperature { get; set; } = 50;

        [TreeValue("CarryAttachedWallSigns")] public bool CarryAttachedWallSigns { get; set; } = false;

        [JsonProperty("BackpackSelectionMode")]
        [TreeValue("BackpackSelectionMode")]
        public string BackpackSelectionModeString { get; set; } = "LastFound";

        [JsonIgnore]
        public BackpackSelectionMode BackpackSelectionModeEnum
            => Enum.TryParse<BackpackSelectionMode>(BackpackSelectionModeString, true, out var mode)
                ? mode : BackpackSelectionMode.LastFound;

        [JsonExtensionData(ReadData = true, WriteData = false)]
        internal Dictionary<string, JToken>? Legacy { get; set; }
    }

    public class DebuggingOptionsConfig
    {
        [TreeValue("LoggingEnabled")] public bool LoggingEnabled { get; set; } = false;
        [TreeValue("DisableHarmonyPatch")] public bool DisableHarmonyPatch { get; set; } = false;
        [TreeValue("EnablePackAdjustmentTool")] public bool EnablePackAdjustmentTool { get; set; } = false;
    }

    public class CarryOnConfig
    {
        private IDictionary<string, string>? backpackMapping;

        public int? ConfigVersion { get; set; }
        public CarryablesConfig Carryables { get; set; } = new CarryablesConfig();
        public CarryablesOnBackConfig CarryablesOnBack { get; set; } = new CarryablesOnBackConfig();
        public InteractablesConfig Interactables { get; set; } = new InteractablesConfig();
        public CarryHungerRateConfig CarryHungerRate { get; set; } = new CarryHungerRateConfig();
        public CarryWalkSpeedConfig CarryWalkSpeed { get; set; } = new CarryWalkSpeedConfig();
        public DropCarriedOnDamageConfig DropCarriedOnDamage { get; set; } = new DropCarriedOnDamageConfig();

        public CarryOptionsConfig CarryOptions { get; set; } = new CarryOptionsConfig();
        public CarryablesFiltersConfig CarryablesFilters { get; set; } = new CarryablesFiltersConfig();

        [JsonIgnore]
        public IDictionary<string, string> BackpackMapping
        {
            get
            {
                if (backpackMapping == null)
                {
                    backpackMapping = new Dictionary<string, string>();
                    foreach (var type in BackpackTypes)
                    {
                        foreach (var code in type.Value)
                        {
                            if (!backpackMapping.ContainsKey(code))
                            {
                                backpackMapping[code] = type.Key;
                            }
                        }
                    }

                }
                return backpackMapping;
            }
        }

        public IDictionary<string, string[]> BackpackTypes { get; set; }
            = new Dictionary<string, string[]>()
            {
                ["small"] = ["game:hunterbackpack"],
                ["large"] = ["game:backpack-normal", "game:backpack-sturdy"]
            };

        public DebuggingOptionsConfig DebuggingOptions { get; set; } = new DebuggingOptionsConfig();

        [JsonExtensionData(ReadData = true, WriteData = false)]
        internal Dictionary<string, JToken>? Legacy { get; set; }

        public CarryOnConfig()
        {

        }

        public CarryOnConfig(int version)
        {
            ConfigVersion = version;
        }

        public void UpgradeVersion()
        {
            try
            {
                // Upgrade from version 1 to 2
                if (ConfigVersion == null)
                {
                    // Perform upgrade actions
                    ConfigVersion = 2;
                    if (Legacy != null)
                    {
                        // Carryables
                        Carryables.Anvil = Legacy.TryGetBool("AnvilEnabled", Carryables.Anvil);
                        Carryables.Barrel = Legacy.TryGetBool("BarrelEnabled", Carryables.Barrel);
                        Carryables.Bookshelf = Legacy.TryGetBool("BookshelfEnabled", Carryables.Bookshelf);
                        Carryables.BunchOCandles = Legacy.TryGetBool("BunchOCandlesEnabled", Carryables.BunchOCandles);
                        Carryables.Chandelier = Legacy.TryGetBool("ChandelierEnabled", Carryables.Chandelier);
                        Carryables.ChestTrunk = Legacy.TryGetBool("ChestTrunkEnabled", Carryables.ChestTrunk);
                        Carryables.Chest = Legacy.TryGetBool("ChestEnabled", Carryables.Chest);
                        Carryables.Clutter = Legacy.TryGetBool("ClutterEnabled", Carryables.Clutter);
                        Carryables.Crate = Legacy.TryGetBool("CrateEnabled", Carryables.Crate);
                        Carryables.DisplayCase = Legacy.TryGetBool("DisplayCaseEnabled", Carryables.DisplayCase);
                        Carryables.Flowerpot = Legacy.TryGetBool("FlowerpotEnabled", Carryables.Flowerpot);
                        Carryables.Forge = Legacy.TryGetBool("ForgeEnabled", Carryables.Forge);
                        Carryables.Henbox = Legacy.TryGetBool("HenboxEnabled", Carryables.Henbox);
                        Carryables.LogWithResin = Legacy.TryGetBool("LogWithResinEnabled", Carryables.LogWithResin);
                        Carryables.LootVessel = Legacy.TryGetBool("LootVesselEnabled", Carryables.LootVessel);
                        Carryables.MoldRack = Legacy.TryGetBool("MoldRackEnabled", Carryables.MoldRack);
                        Carryables.Mold = Legacy.TryGetBool("MoldsEnabled", Carryables.Mold);
                        Carryables.Oven = Legacy.TryGetBool("OvenEnabled", Carryables.Oven);
                        Carryables.Planter = Legacy.TryGetBool("PlanterEnabled", Carryables.Planter);
                        Carryables.Quern = Legacy.TryGetBool("QuernEnabled", Carryables.Quern);
                        Carryables.ReedChest = Legacy.TryGetBool("ReedBasketEnabled", Carryables.ReedChest);
                        Carryables.Shelf = Legacy.TryGetBool("ShelfEnabled", Carryables.Shelf);
                        Carryables.Sign = Legacy.TryGetBool("SignEnabled", Carryables.Sign);
                        Carryables.StorageVessel = Legacy.TryGetBool("StorageVesselEnabled", Carryables.StorageVessel);
                        Carryables.ToolRack = Legacy.TryGetBool("ToolRackEnabled", Carryables.ToolRack);
                        Carryables.TorchHolder = Legacy.TryGetBool("TorchHolderEnabled", Carryables.TorchHolder);
                        Carryables.Resonator = Legacy.TryGetBool("ResonatorEnabled", Carryables.Resonator);

                        // Interactables
                        Interactables.Door = Legacy.TryGetBool("InteractDoorEnabled", Interactables.Door);
                        Interactables.Storage = Legacy.TryGetBool("InteractStorageEnabled", Interactables.Storage);

                        // CarryOptions
                        CarryOptions.BackSlotEnabled = Legacy.TryGetBool("BackSlotEnabled", CarryOptions.BackSlotEnabled);
                        CarryablesOnBack.ChestTrunk = Legacy.TryGetBool("AllowChestTrunksOnBack", CarryablesOnBack.ChestTrunk);
                        CarryOptions.AllowHighCapacityStorageOnBack = Legacy.TryGetBool("AllowLargeChestsOnBack", CarryOptions.AllowHighCapacityStorageOnBack);
                        CarryablesOnBack.Crate = Legacy.TryGetBool("AllowCratesOnBack", CarryablesOnBack.Crate);
                        CarryOptions.RemoveInteractDelayWhileCarrying = Legacy.TryGetBool("RemoveInteractDelayWhileCarrying", CarryOptions.RemoveInteractDelayWhileCarrying);
                        CarryOptions.InteractSpeedMultiplier = Legacy.TryGetFloat("InteractSpeedMultiplier", CarryOptions.InteractSpeedMultiplier);

                        // Debugging Options
                        DebuggingOptions.LoggingEnabled = Legacy.TryGetBool("LoggingEnabled", DebuggingOptions.LoggingEnabled);
                        DebuggingOptions.DisableHarmonyPatch = !Legacy.TryGetBool("HarmonyPatchEnabled", !DebuggingOptions.DisableHarmonyPatch);

                        // CarryablesFilters
                        CarryablesFilters.AutoMatchIgnoreMods = Legacy.TryGetStringArray("AutoMatchIgnoreMods", CarryablesFilters.AutoMatchIgnoreMods);
                        CarryablesFilters.AllowedShapeOnlyMatches = Legacy.TryGetStringArray("AllowedShapeOnlyMatches", CarryablesFilters.AllowedShapeOnlyMatches);
                        CarryablesFilters.RemoveBaseCarryableBehaviour = Legacy.TryGetStringArray("RemoveBaseCarryableBehaviour", CarryablesFilters.RemoveBaseCarryableBehaviour);
                        CarryablesFilters.RemoveCarryableBehaviour = Legacy.TryGetStringArray("RemoveCarryableBehaviour", CarryablesFilters.RemoveCarryableBehaviour);
                    }
                }
                if (ConfigVersion == 2)
                {
                    ConfigVersion = 3;

                    if (CarryOptions?.Legacy == null) return;

                    if (CarryOptions.Legacy.ContainsKey("AllowLargeChestsOnBack"))
                    {
                        CarryOptions.AllowHighCapacityStorageOnBack = CarryOptions.Legacy.TryGetBool("AllowLargeChestsOnBack", CarryOptions.AllowHighCapacityStorageOnBack);
                    }
                    if (CarryOptions.Legacy.ContainsKey("AllowChestTrunksOnBack"))
                    {
                        CarryablesOnBack.ChestTrunk = CarryOptions.Legacy.TryGetBool("AllowChestTrunksOnBack", CarryablesOnBack.ChestTrunk);
                    }
                    if (CarryOptions.Legacy.ContainsKey("AllowCratesOnBack"))
                    {
                        CarryablesOnBack.Crate = CarryOptions.Legacy.TryGetBool("AllowCratesOnBack", CarryablesOnBack.Crate);
                    }

                }
                if (ConfigVersion == 3)
                {
                    ConfigVersion = 4;

                    if (CarryOptions?.Legacy == null) return;

                    CarryWalkSpeed.HandsEnabled = !CarryOptions.Legacy.TryGetBool("IgnoreCarrySpeedPenalty", false);
                    CarryWalkSpeed.BackEnabled = !CarryOptions.Legacy.TryGetBool("IgnoreCarrySpeedPenalty", false);
                    CarryWalkSpeed.HandsAllowSprint = CarryOptions.Legacy.TryGetBool("AllowSprintWhileCarrying", false);
                    CarryWalkSpeed.BackAllowSprint = CarryOptions.Legacy.TryGetBool("AllowSprintWhileCarrying", false);

                    if (CarryOptions.Legacy.TryGetValue("WalkSpeedOverrides", out var overridesToken)
                        && overridesToken is JObject overridesObj)
                    {
                        CarryWalkSpeed.ModifierOverrides = overridesObj.ToObject<ModifierOverridesConfig>()
                            ?? new ModifierOverridesConfig();
                    }

                }
            }
            finally
            {
                ConfigVersion = CurrentConfigVersion;
            }
        }


        public ITreeAttribute ToTreeAttribute()
        {
            var tree = new TreeAttribute();
            tree.SetInt("ConfigVersion", ConfigVersion ?? 2);

            tree["Carryables"] = TreeSerializer.ToTree(Carryables);
            tree["CarryablesOnBack"] = TreeSerializer.ToTree(CarryablesOnBack);
            tree["Interactables"] = TreeSerializer.ToTree(Interactables);
            tree["CarryHungerRate"] = ToCarryHungerRateTree(CarryHungerRate);
            tree["CarryWalkSpeed"] = ToCarryWalkSpeedTree(CarryWalkSpeed);
            tree["DropCarriedOnDamage"] = ToDropCarriedOnDamageTree(DropCarriedOnDamage);
            tree["CarryOptions"] = ToCarryOptionsTree();
            tree["CarryableFilters"] = TreeSerializer.ToTree(CarryablesFilters);
            tree["DebuggingOptions"] = TreeSerializer.ToTree(DebuggingOptions);


            return tree;
        }

        public static CarryOnConfig FromTreeAttribute(ITreeAttribute tree)
        {
            var config = new CarryOnConfig();

            if (tree.HasAttribute("ConfigVersion"))
                config.ConfigVersion = tree.GetInt("ConfigVersion");

            TreeSerializer.FromTree(tree["Carryables"] as ITreeAttribute, config.Carryables);
            TreeSerializer.FromTree(tree["CarryablesOnBack"] as ITreeAttribute, config.CarryablesOnBack);
            TreeSerializer.FromTree(tree["Interactables"] as ITreeAttribute, config.Interactables);
            config.CarryHungerRate = FromCarryHungerRateTree(tree["CarryHungerRate"] as ITreeAttribute);
            config.CarryWalkSpeed = FromCarryWalkSpeedTree(tree["CarryWalkSpeed"] as ITreeAttribute);
            config.DropCarriedOnDamage = FromDropCarriedOnDamageTree(tree["DropCarriedOnDamage"] as ITreeAttribute);
            FromCarryOptionsTree(tree["CarryOptions"] as ITreeAttribute, config.CarryOptions);
            TreeSerializer.FromTree(tree["CarryableFilters"] as ITreeAttribute, config.CarryablesFilters);
            TreeSerializer.FromTree(tree["DebuggingOptions"] as ITreeAttribute, config.DebuggingOptions);

            return config;
        }

        private ITreeAttribute ToCarryOptionsTree()
        {
            return (TreeAttribute)TreeSerializer.ToTree(CarryOptions);
        }

        private static ITreeAttribute ToCarryHungerRateTree(CarryHungerRateConfig config)
        {
            var tree = (TreeAttribute)TreeSerializer.ToTree(config);
            tree["ModifierOverrides"] = ToWalkSpeedOverridesTree(config.ModifierOverrides);
            return tree;
        }

        private static CarryHungerRateConfig FromCarryHungerRateTree(ITreeAttribute? tree)
        {
            var config = new CarryHungerRateConfig();
            if (tree == null) return config;

            TreeSerializer.FromTree(tree, config);
            config.ModifierOverrides = FromWalkSpeedOverridesTree(tree["ModifierOverrides"] as ITreeAttribute);
            return config;
        }

        private static void FromCarryOptionsTree(ITreeAttribute? tree, CarryOptionsConfig carryOptions)
        {
            if (tree == null) return;

            TreeSerializer.FromTree(tree, carryOptions);
        }

        private static ITreeAttribute ToCarryWalkSpeedTree(CarryWalkSpeedConfig config)
        {
            var tree = (TreeAttribute)TreeSerializer.ToTree(config);
            tree["ModifierOverrides"] = ToWalkSpeedOverridesTree(config.ModifierOverrides);
            return tree;
        }

        private static CarryWalkSpeedConfig FromCarryWalkSpeedTree(ITreeAttribute? tree)
        {
            var config = new CarryWalkSpeedConfig();
            if (tree == null) return config;

            TreeSerializer.FromTree(tree, config);
            config.ModifierOverrides = FromWalkSpeedOverridesTree(tree["ModifierOverrides"] as ITreeAttribute);
            return config;
        }

        private static ITreeAttribute ToWalkSpeedOverridesTree(ModifierOverridesConfig overrides)
        {
            var tree = new TreeAttribute();
            overrides ??= new ModifierOverridesConfig();

            tree["ByBlockCode"] = ToSpeedMapTree(overrides.ByBlockCode);
            tree["ByBlockClass"] = ToSpeedMapTree(overrides.ByBlockClass);
            tree["SlotDefaults"] = ToSlotSpeedTree(overrides.SlotDefaults);

            return tree;
        }

        private static ModifierOverridesConfig FromWalkSpeedOverridesTree(ITreeAttribute? tree)
        {
            if (tree == null)
            {
                return new ModifierOverridesConfig();
            }

            return new ModifierOverridesConfig
            {
                ByBlockCode = FromSpeedMapTree(tree["ByBlockCode"] as ITreeAttribute),
                ByBlockClass = FromSpeedMapTree(tree["ByBlockClass"] as ITreeAttribute),
                SlotDefaults = FromSlotSpeedTree(tree["SlotDefaults"] as ITreeAttribute)
            };
        }

        private static ITreeAttribute ToSpeedMapTree(IDictionary<string, SlotModifierConfig> map)
        {
            var tree = new TreeAttribute();

            if (map == null)
            {
                return tree;
            }

            foreach (var entry in map)
            {
                if (string.IsNullOrWhiteSpace(entry.Key) || entry.Value == null || entry.Value.IsEmpty)
                {
                    continue;
                }

                tree[entry.Key.Trim()] = ToSlotSpeedTree(entry.Value);
            }

            return tree;
        }

        private static IDictionary<string, SlotModifierConfig> FromSpeedMapTree(ITreeAttribute? tree)
        {
            var map = new Dictionary<string, SlotModifierConfig>();

            if (tree is not TreeAttribute speedTree)
            {
                return map;
            }

            foreach (var key in speedTree.Keys)
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                var value = FromSlotSpeedTree(speedTree[key] as ITreeAttribute);
                if (value.IsEmpty)
                {
                    continue;
                }

                map[key.Trim()] = value;
            }

            return map;
        }

        private static ITreeAttribute ToSlotSpeedTree(SlotModifierConfig speed)
        {
            var tree = new TreeAttribute();
            speed ??= new SlotModifierConfig();

            if (speed.Hands.HasValue)
            {
                tree.SetFloat(CarrySlot.Hands.ToString(), speed.Hands.Value);
            }

            if (speed.Back.HasValue)
            {
                tree.SetFloat(CarrySlot.Back.ToString(), speed.Back.Value);
            }

            return tree;
        }

        private static SlotModifierConfig FromSlotSpeedTree(ITreeAttribute? tree)
        {
            var speed = new SlotModifierConfig();

            if (tree == null)
            {
                return speed;
            }

            if (tree.HasAttribute(CarrySlot.Hands.ToString()))
            {
                speed.Hands = tree.GetFloat(CarrySlot.Hands.ToString());
            }

            if (tree.HasAttribute(CarrySlot.Back.ToString()))
            {
                speed.Back = tree.GetFloat(CarrySlot.Back.ToString());
            }

            return speed;
        }

        private static ITreeAttribute ToDropCarriedOnDamageTree(DropCarriedOnDamageConfig config)
        {
            return (TreeAttribute)TreeSerializer.ToTree(config);
        }

        private static DropCarriedOnDamageConfig FromDropCarriedOnDamageTree(ITreeAttribute? tree)
        {
            var config = new DropCarriedOnDamageConfig();
            if (tree != null)
            {
                TreeSerializer.FromTree(tree, config);
            }
            return config;
        }
    }
}
