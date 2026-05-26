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
        public bool Anvil { get; set; } = true;
        public bool Barrel { get; set; } = true;
        public bool Bookshelf { get; set; }
        public bool BunchOCandles { get; set; }
        public bool Chandelier { get; set; }
        public bool ChestTrunk { get; set; }
        public bool Chest { get; set; } = true;
        public bool Clutter { get; set; }
        public bool Crate { get; set; } = true;
        public bool DisplayCase { get; set; }
        public bool Flowerpot { get; set; } = true;
        public bool Forge { get; set; }
        public bool Henbox { get; set; }
        public bool LogWithResin { get; set; }
        public bool LootVessel { get; set; } = true;
        public bool MoldRack { get; set; }
        public bool Mold { get; set; }
        public bool Oven { get; set; }
        public bool Planter { get; set; } = true;
        public bool Quern { get; set; } = true;
        public bool ReedChest { get; set; } = true;
        public bool Resonator { get; set; } = true;
        public bool Shelf { get; set; }
        public bool Sign { get; set; }
        public bool StorageVessel { get; set; } = true;
        public bool ToolRack { get; set; }
        public bool TorchHolder { get; set; }
    }

    public class CarryablesOnBackConfig
    {
        public bool Barrel { get; set; } = true;
        public bool ChestLabeled { get; set; } = true;
        public bool ChestTrunk { get; set; }
        public bool Chest { get; set; } = true;
        public bool Crate { get; set; }
        public bool Flowerpot { get; set; } = true;
        public bool LogWithResin { get; set; }
        public bool LootVessel { get; set; } = true;
        public bool Planter { get; set; } = true;
        public bool ReedChest { get; set; } = true;
        public bool Resonator { get; set; } = true;
        public bool StorageVessel { get; set; } = true;
    }

    public class InteractablesConfig
    {
        public bool Door { get; set; } = true;
        public bool Barrel { get; set; } = true;
        public bool Storage { get; set; } = true;
    }

    public class CarryablesFiltersConfig
    {
        public bool AutoMapSimilar { get; set; } = true;

        public string[] AutoMatchIgnoreMods { get; set; } = ["mcrate"];

        public string[] AllowedShapeOnlyMatches { get; set; } = ["block/clay/lootvessel", "block/wood/chest/normal", "block/wood/trunk/normal", "block/reed/basket-normal"];

        public string[] RemoveBaseCarryableBehaviour { get; set; } = ["woodchests:wtrunk"];

        public string[] RemoveCarryableBehaviour { get; set; } = ["game:banner", "game:clutter-devastation"];
    }

    public class CarryOptionsConfig
    {
        public bool AllowSprintWhileCarrying { get; set; } = false;
        public bool IgnoreCarrySpeedPenalty { get; set; } = false;
        public bool RemoveInteractDelayWhileCarrying { get; set; } = false;
        public float InteractSpeedMultiplier { get; set; } = 1.0f;

        // Maximum distance (in world units) that interactions with carried/attachable
        // entities/blocks are allowed from the player.
        public int MaxInteractionDistance { get; set; } = Default.MaxInteractionDistance;

        public bool BackSlotEnabled { get; set; } = true;
        public bool AllowHighCapacityStorageOnBack { get; set; } = false;
        public string[] PreventSwapFromBackOnTarget { get; set; } = ["behavior::Container", "behavior::Door", "class::portals.portal", "code::groundstorage", "class::BlockGroundStorage"];

        public bool TooHotToCarry { get; set; } = true;
        public int TooHotToCarryTemperature { get; set; } = 50;

        [JsonExtensionData(ReadData = true, WriteData = false)]
        internal Dictionary<string, JToken> Legacy { get; set; }
    }

    public class DebuggingOptionsConfig
    {
        public bool LoggingEnabled { get; set; } = false;
        public bool DisableHarmonyPatch { get; set; } = false;
        public bool EnablePackAdjustmentTool { get; set; } = false;
    }


    public class CarryOnConfig
    {
        private IDictionary<string, string> backpackMapping;

        public int? ConfigVersion { get; set; }
        public CarryablesConfig Carryables { get; set; } = new CarryablesConfig();

        public CarryablesOnBackConfig CarryablesOnBack { get; set; } = new CarryablesOnBackConfig();

        public InteractablesConfig Interactables { get; set; } = new InteractablesConfig();

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
        internal Dictionary<string, JToken> Legacy { get; set; }

        public CarryOnConfig()
        {

        }

        public CarryOnConfig(int version)
        {
            ConfigVersion = version;
        }

        public void UpgradeVersion()
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
                    CarryOptions.AllowSprintWhileCarrying = Legacy.TryGetBool("AllowSprintWhileCarrying", CarryOptions.AllowSprintWhileCarrying);
                    CarryOptions.IgnoreCarrySpeedPenalty = Legacy.TryGetBool("IgnoreCarrySpeedPenalty", CarryOptions.IgnoreCarrySpeedPenalty);
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
        }


        public ITreeAttribute ToTreeAttribute()
        {
            var tree = new TreeAttribute();
            tree.SetInt(ConfigKey.ConfigVersionKey, ConfigVersion ?? 2);

            // Carryables
            var carryables = new TreeAttribute();
            carryables.SetBool(ConfigKey.Carryables.AnvilKey, Carryables.Anvil);
            carryables.SetBool(ConfigKey.Carryables.BarrelKey, Carryables.Barrel);
            carryables.SetBool(ConfigKey.Carryables.BookshelfKey, Carryables.Bookshelf);
            carryables.SetBool(ConfigKey.Carryables.BunchOCandlesKey, Carryables.BunchOCandles);
            carryables.SetBool(ConfigKey.Carryables.ChandelierKey, Carryables.Chandelier);
            carryables.SetBool(ConfigKey.Carryables.ChestTrunkKey, Carryables.ChestTrunk);
            carryables.SetBool(ConfigKey.Carryables.ChestKey, Carryables.Chest);
            carryables.SetBool(ConfigKey.Carryables.ClutterKey, Carryables.Clutter);
            carryables.SetBool(ConfigKey.Carryables.CrateKey, Carryables.Crate);
            carryables.SetBool(ConfigKey.Carryables.DisplayCaseKey, Carryables.DisplayCase);
            carryables.SetBool(ConfigKey.Carryables.FlowerpotKey, Carryables.Flowerpot);
            carryables.SetBool(ConfigKey.Carryables.ForgeKey, Carryables.Forge);
            carryables.SetBool(ConfigKey.Carryables.HenboxKey, Carryables.Henbox);
            carryables.SetBool(ConfigKey.Carryables.LogWithResinKey, Carryables.LogWithResin);
            carryables.SetBool(ConfigKey.Carryables.LootVesselKey, Carryables.LootVessel);
            carryables.SetBool(ConfigKey.Carryables.MoldRackKey, Carryables.MoldRack);
            carryables.SetBool(ConfigKey.Carryables.MoldKey, Carryables.Mold);
            carryables.SetBool(ConfigKey.Carryables.OvenKey, Carryables.Oven);
            carryables.SetBool(ConfigKey.Carryables.PlanterKey, Carryables.Planter);
            carryables.SetBool(ConfigKey.Carryables.QuernKey, Carryables.Quern);
            carryables.SetBool(ConfigKey.Carryables.ReedChestKey, Carryables.ReedChest);
            carryables.SetBool(ConfigKey.Carryables.ResonatorKey, Carryables.Resonator);
            carryables.SetBool(ConfigKey.Carryables.ShelfKey, Carryables.Shelf);
            carryables.SetBool(ConfigKey.Carryables.SignKey, Carryables.Sign);
            carryables.SetBool(ConfigKey.Carryables.StorageVesselKey, Carryables.StorageVessel);
            carryables.SetBool(ConfigKey.Carryables.ToolRackKey, Carryables.ToolRack);
            carryables.SetBool(ConfigKey.Carryables.TorchHolderKey, Carryables.TorchHolder);
            tree[ConfigKey.CarryablesKey] = carryables;


            // Carryables on Back
            var carryablesOnBack = new TreeAttribute();
            carryablesOnBack.SetBool(ConfigKey.Carryables.BarrelKey, CarryablesOnBack.Barrel);
            carryablesOnBack.SetBool(ConfigKey.Carryables.ChestTrunkKey, CarryablesOnBack.ChestTrunk);
            carryablesOnBack.SetBool(ConfigKey.Carryables.ChestKey, CarryablesOnBack.Chest);
            carryablesOnBack.SetBool(ConfigKey.Carryables.CrateKey, CarryablesOnBack.Crate);
            carryablesOnBack.SetBool(ConfigKey.Carryables.FlowerpotKey, CarryablesOnBack.Flowerpot);
            carryablesOnBack.SetBool(ConfigKey.Carryables.LogWithResinKey, CarryablesOnBack.LogWithResin);
            carryablesOnBack.SetBool(ConfigKey.Carryables.LootVesselKey, CarryablesOnBack.LootVessel);
            carryablesOnBack.SetBool(ConfigKey.Carryables.PlanterKey, CarryablesOnBack.Planter);
            carryablesOnBack.SetBool(ConfigKey.Carryables.ReedChestKey, CarryablesOnBack.ReedChest);
            carryablesOnBack.SetBool(ConfigKey.Carryables.StorageVesselKey, CarryablesOnBack.StorageVessel);
            tree[ConfigKey.CarryablesOnBackKey] = carryablesOnBack;

            // Interactables
            var interactables = new TreeAttribute();
            interactables.SetBool(ConfigKey.Interactables.DoorKey, Interactables.Door);
            interactables.SetBool(ConfigKey.Interactables.BarrelKey, Interactables.Barrel);
            interactables.SetBool(ConfigKey.Interactables.StorageKey, Interactables.Storage);
            tree[ConfigKey.InteractablesKey] = interactables;

            // CarryOptions
            var carryOptions = new TreeAttribute();
            carryOptions.SetBool(ConfigKey.CarryOptions.AllowSprintWhileCarryingKey, CarryOptions.AllowSprintWhileCarrying);
            carryOptions.SetBool(ConfigKey.CarryOptions.IgnoreCarrySpeedPenaltyKey, CarryOptions.IgnoreCarrySpeedPenalty);
            carryOptions.SetBool(ConfigKey.CarryOptions.RemoveInteractDelayWhileCarryingKey, CarryOptions.RemoveInteractDelayWhileCarrying);
            carryOptions.SetFloat(ConfigKey.CarryOptions.InteractSpeedMultiplierKey, CarryOptions.InteractSpeedMultiplier);
            carryOptions.SetInt(ConfigKey.CarryOptions.MaxInteractionDistanceKey, CarryOptions.MaxInteractionDistance);
            carryOptions.SetBool(ConfigKey.CarryOptions.BackSlotEnabledKey, CarryOptions.BackSlotEnabled);
            carryOptions.SetBool(ConfigKey.CarryOptions.AllowHighCapacityStorageOnBackKey, CarryOptions.AllowHighCapacityStorageOnBack);
            carryOptions.SetStringArray(ConfigKey.CarryOptions.PreventSwapFromBackOnTargetKey, CarryOptions.PreventSwapFromBackOnTarget);
            carryOptions.SetBool(ConfigKey.CarryOptions.TooHotToCarryKey, CarryOptions.TooHotToCarry);
            carryOptions.SetInt(ConfigKey.CarryOptions.TooHotToCarryTemperatureKey, CarryOptions.TooHotToCarryTemperature);
            tree[ConfigKey.CarryOptionsKey] = carryOptions;

            // CarryablesFilters
            var filters = new TreeAttribute();
            filters.SetBool(ConfigKey.CarryableFilters.AutoMapSimilarKey, CarryablesFilters.AutoMapSimilar);
            filters.SetStringArray(ConfigKey.CarryableFilters.AutoMatchIgnoreModsKey, CarryablesFilters.AutoMatchIgnoreMods);
            filters.SetStringArray(ConfigKey.CarryableFilters.AllowedShapeOnlyMatchesKey, CarryablesFilters.AllowedShapeOnlyMatches);
            filters.SetStringArray(ConfigKey.CarryableFilters.RemoveBaseCarryableBehaviourKey, CarryablesFilters.RemoveBaseCarryableBehaviour);
            filters.SetStringArray(ConfigKey.CarryableFilters.RemoveCarryableBehaviourKey, CarryablesFilters.RemoveCarryableBehaviour);
            tree[ConfigKey.CarryableFiltersKey] = filters;

            // DebuggingOptions
            var debug = new TreeAttribute();
            debug.SetBool(ConfigKey.DebuggingOptions.LoggingEnabledKey, DebuggingOptions.LoggingEnabled);
            debug.SetBool(ConfigKey.DebuggingOptions.DisableHarmonyPatchKey, DebuggingOptions.DisableHarmonyPatch);
            debug.SetBool(ConfigKey.DebuggingOptions.EnablePackAdjustmentToolKey, DebuggingOptions.EnablePackAdjustmentTool);
            tree[ConfigKey.DebuggingOptionsKey] = debug;

            return tree;
        }

        public static CarryOnConfig FromTreeAttribute(ITreeAttribute tree)
        {
            var config = new CarryOnConfig();

            // ConfigVersion
            if (tree.HasAttribute(ConfigKey.ConfigVersionKey))
                config.ConfigVersion = tree.GetInt(ConfigKey.ConfigVersionKey);

            // Carryables
            var carryables = tree[ConfigKey.CarryablesKey] as ITreeAttribute;
            if (carryables != null)
            {
                config.Carryables.Anvil = carryables.GetBool(ConfigKey.Carryables.AnvilKey);
                config.Carryables.Barrel = carryables.GetBool(ConfigKey.Carryables.BarrelKey);
                config.Carryables.Bookshelf = carryables.GetBool(ConfigKey.Carryables.BookshelfKey);
                config.Carryables.BunchOCandles = carryables.GetBool(ConfigKey.Carryables.BunchOCandlesKey);
                config.Carryables.Chandelier = carryables.GetBool(ConfigKey.Carryables.ChandelierKey);
                config.Carryables.ChestTrunk = carryables.GetBool(ConfigKey.Carryables.ChestTrunkKey);
                config.Carryables.Chest = carryables.GetBool(ConfigKey.Carryables.ChestKey);
                config.Carryables.Clutter = carryables.GetBool(ConfigKey.Carryables.ClutterKey);
                config.Carryables.Crate = carryables.GetBool(ConfigKey.Carryables.CrateKey);
                config.Carryables.DisplayCase = carryables.GetBool(ConfigKey.Carryables.DisplayCaseKey);
                config.Carryables.Flowerpot = carryables.GetBool(ConfigKey.Carryables.FlowerpotKey);
                config.Carryables.Forge = carryables.GetBool(ConfigKey.Carryables.ForgeKey);
                config.Carryables.Henbox = carryables.GetBool(ConfigKey.Carryables.HenboxKey);
                config.Carryables.LogWithResin = carryables.GetBool(ConfigKey.Carryables.LogWithResinKey);
                config.Carryables.LootVessel = carryables.GetBool(ConfigKey.Carryables.LootVesselKey);
                config.Carryables.MoldRack = carryables.GetBool(ConfigKey.Carryables.MoldRackKey);
                config.Carryables.Mold = carryables.GetBool(ConfigKey.Carryables.MoldKey);
                config.Carryables.Oven = carryables.GetBool(ConfigKey.Carryables.OvenKey);
                config.Carryables.Planter = carryables.GetBool(ConfigKey.Carryables.PlanterKey);
                config.Carryables.Quern = carryables.GetBool(ConfigKey.Carryables.QuernKey);
                config.Carryables.ReedChest = carryables.GetBool(ConfigKey.Carryables.ReedChestKey);
                config.Carryables.Resonator = carryables.GetBool(ConfigKey.Carryables.ResonatorKey);
                config.Carryables.Shelf = carryables.GetBool(ConfigKey.Carryables.ShelfKey);
                config.Carryables.Sign = carryables.GetBool(ConfigKey.Carryables.SignKey);
                config.Carryables.StorageVessel = carryables.GetBool(ConfigKey.Carryables.StorageVesselKey);
                config.Carryables.ToolRack = carryables.GetBool(ConfigKey.Carryables.ToolRackKey);
                config.Carryables.TorchHolder = carryables.GetBool(ConfigKey.Carryables.TorchHolderKey);
            }

            // Carryables
            var carryablesOnBack = tree[ConfigKey.CarryablesOnBackKey] as ITreeAttribute;
            if (carryablesOnBack != null)
            {
                config.CarryablesOnBack.Barrel = carryablesOnBack.GetBool(ConfigKey.Carryables.BarrelKey);
                config.CarryablesOnBack.ChestTrunk = carryablesOnBack.GetBool(ConfigKey.Carryables.ChestTrunkKey);
                config.CarryablesOnBack.Chest = carryablesOnBack.GetBool(ConfigKey.Carryables.ChestKey);
                config.CarryablesOnBack.Crate = carryablesOnBack.GetBool(ConfigKey.Carryables.CrateKey);
                config.CarryablesOnBack.Flowerpot = carryablesOnBack.GetBool(ConfigKey.Carryables.FlowerpotKey);
                config.CarryablesOnBack.LogWithResin = carryablesOnBack.GetBool(ConfigKey.Carryables.LogWithResinKey);
                config.CarryablesOnBack.LootVessel = carryablesOnBack.GetBool(ConfigKey.Carryables.LootVesselKey);
                config.CarryablesOnBack.Planter = carryablesOnBack.GetBool(ConfigKey.Carryables.PlanterKey);
                config.CarryablesOnBack.ReedChest = carryablesOnBack.GetBool(ConfigKey.Carryables.ReedChestKey);
                config.CarryablesOnBack.Resonator = carryablesOnBack.GetBool(ConfigKey.Carryables.ResonatorKey);
                config.CarryablesOnBack.StorageVessel = carryablesOnBack.GetBool(ConfigKey.Carryables.StorageVesselKey);
            }            

            // Interactables
            var interactables = tree[ConfigKey.InteractablesKey] as ITreeAttribute;
            if (interactables != null)
            {
                config.Interactables.Door = interactables.GetBool(ConfigKey.Interactables.DoorKey);
                config.Interactables.Barrel = interactables.GetBool(ConfigKey.Interactables.BarrelKey);
                config.Interactables.Storage = interactables.GetBool(ConfigKey.Interactables.StorageKey);
            }

            // CarryOptions
            var carryOptions = tree[ConfigKey.CarryOptionsKey] as ITreeAttribute;
            if (carryOptions != null)
            {
                config.CarryOptions.AllowSprintWhileCarrying = carryOptions.GetBool(ConfigKey.CarryOptions.AllowSprintWhileCarryingKey);
                config.CarryOptions.IgnoreCarrySpeedPenalty = carryOptions.GetBool(ConfigKey.CarryOptions.IgnoreCarrySpeedPenaltyKey);
                config.CarryOptions.RemoveInteractDelayWhileCarrying = carryOptions.GetBool(ConfigKey.CarryOptions.RemoveInteractDelayWhileCarryingKey);
                config.CarryOptions.InteractSpeedMultiplier = carryOptions.GetFloat(ConfigKey.CarryOptions.InteractSpeedMultiplierKey);
                if (carryOptions.HasAttribute(ConfigKey.CarryOptions.MaxInteractionDistanceKey))
                {
                    config.CarryOptions.MaxInteractionDistance = carryOptions.GetInt(ConfigKey.CarryOptions.MaxInteractionDistanceKey);
                }
                config.CarryOptions.BackSlotEnabled = carryOptions.GetBool(ConfigKey.CarryOptions.BackSlotEnabledKey);
                config.CarryOptions.AllowHighCapacityStorageOnBack = carryOptions.GetBool(ConfigKey.CarryOptions.AllowHighCapacityStorageOnBackKey);

                // Get array from carryOptions ConfigKey.CarryOptions.PreventSwapFromBackOnTargetKey - GetStringArray does not exist
                config.CarryOptions.PreventSwapFromBackOnTarget = (carryOptions[ConfigKey.CarryOptions.PreventSwapFromBackOnTargetKey] as StringArrayAttribute)?.value ?? [];
                config.CarryOptions.TooHotToCarry = carryOptions.GetBool(ConfigKey.CarryOptions.TooHotToCarryKey);
                config.CarryOptions.TooHotToCarryTemperature = carryOptions.GetInt(ConfigKey.CarryOptions.TooHotToCarryTemperatureKey);
            }

            // CarryablesFilters
            var filters = tree[ConfigKey.CarryableFiltersKey] as TreeAttribute;
            if (filters != null)
            {
                config.CarryablesFilters.AutoMapSimilar = filters.GetBool(ConfigKey.CarryableFilters.AutoMapSimilarKey);
                config.CarryablesFilters.AutoMatchIgnoreMods = filters.GetStringArray(ConfigKey.CarryableFilters.AutoMatchIgnoreModsKey);
                config.CarryablesFilters.AllowedShapeOnlyMatches = filters.GetStringArray(ConfigKey.CarryableFilters.AllowedShapeOnlyMatchesKey);
                config.CarryablesFilters.RemoveBaseCarryableBehaviour = filters.GetStringArray(ConfigKey.CarryableFilters.RemoveBaseCarryableBehaviourKey);
                config.CarryablesFilters.RemoveCarryableBehaviour = (filters[ConfigKey.CarryableFilters.RemoveCarryableBehaviourKey] as StringArrayAttribute)?.value ?? [];
            }

            // DebuggingOptions
            var debug = tree[ConfigKey.DebuggingOptionsKey] as ITreeAttribute;
            if (debug != null)
            {
                config.DebuggingOptions.LoggingEnabled = debug.GetBool(ConfigKey.DebuggingOptions.LoggingEnabledKey);
                config.DebuggingOptions.DisableHarmonyPatch = debug.GetBool(ConfigKey.DebuggingOptions.DisableHarmonyPatchKey);
                config.DebuggingOptions.EnablePackAdjustmentTool = debug.GetBool(ConfigKey.DebuggingOptions.EnablePackAdjustmentToolKey);
            }

            return config;
        }
    }
}