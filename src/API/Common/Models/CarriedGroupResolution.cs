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

        public string AssetNameIfUnset { get; set; }

        // Explicit link back to source container slot key ("0", "1", etc.)
        public string SourceSlotKey { get; set; }
    }

    public class CarriedGroupResolution
    {
        public IList<string> PrimaryGroupCandidates { get; set; } = new List<string>();

        public string PrimaryGroup
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
