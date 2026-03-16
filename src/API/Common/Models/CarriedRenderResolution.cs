using System.Collections.Generic;

namespace CarryOn.API.Common.Models
{
    public enum CarryRenderAssetType
    {
        None,
        Block,
        Item
    }

    public class CarriedGroupCandidateSet
    {
        public IList<string> Groups { get; set; } = new List<string>();

        public bool AddAllMatches { get; set; } = false;

        public CarryRenderAssetType AssetTypeIfUnset { get; set; } = CarryRenderAssetType.None;

        public string AssetNameIfUnset { get; set; }
    }

    public class CarriedRenderResolution
    {
        public string PrimaryGroup { get; set; }

        public IList<CarriedGroupCandidateSet> AdditionalGroupCandidates { get; set; } = new List<CarriedGroupCandidateSet>();

        public bool EnableVertexWarpForAdditionalTransforms { get; set; } = false;
    }
}
