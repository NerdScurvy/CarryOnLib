using System.Collections.Generic;

namespace CarryOn.API.Common.Models
{
    public enum CarriedGroupAssetType
    {
        None,
        Block,
        Item
    }

    public class CarriedGroupCandidateSet
    {
        public IList<string> Groups { get; set; } = new List<string>();

        public bool AddAllMatches { get; set; } = false;

        public CarriedGroupAssetType AssetTypeIfUnset { get; set; } = CarriedGroupAssetType.None;

        public string? AssetNameIfUnset { get; set; }

        public string? SourceSlotKey { get; set; }

        public bool ApplyDisplaySlotYaw { get; set; } = false;

        public bool ApplyDisplayCaseYawOffset { get; set; } = false;

        public bool ApplyOnDisplayTransform { get; set; } = false;
    }
}
