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

        // Explicit link back to source container slot key ("0", "1", etc.)
        public string? SourceSlotKey { get; set; }

        // Apply slot yaw from block entity rotation data (e.g. rotation0, rotation1).
        public bool ApplyDisplaySlotYaw { get; set; } = false;

        // Apply the display case basis offset (+45 degrees) when slot yaw is used.
        public bool ApplyDisplayCaseYawOffset { get; set; } = false;

        // Apply collectible onDisplayTransform as a secondary transform.
        public bool ApplyOnDisplayTransform { get; set; } = false;
    }

    public class CarriedGroupResolution
    {
        public IList<string> PrimaryGroupCandidates { get; set; } = new List<string>();

        public string? PrimaryGroup
        {
            get => PrimaryGroupCandidates != null && PrimaryGroupCandidates.Count > 0
                ? PrimaryGroupCandidates[0]
                : null;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                if (PrimaryGroupCandidates == null)
                {
                    PrimaryGroupCandidates = new List<string>();
                }

                if (PrimaryGroupCandidates.Count == 0)
                {
                    PrimaryGroupCandidates.Add(value);
                }
                else
                {
                    PrimaryGroupCandidates[0] = value;
                }
            }
        }

        public IList<CarriedGroupCandidateSet> AdditionalGroupCandidates { get; set; } = new List<CarriedGroupCandidateSet>();

        public bool EnableVertexWarpForAdditionalTransforms { get; set; } = false;
    }
}
