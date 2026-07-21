using System.Collections.Generic;
using System.Linq;

namespace CarryOn.API.Common.Models
{
    public class CarriedGroupCandidateSet
    {
        public IReadOnlyList<string> Groups { get; }

        public bool AddAllMatches { get; set; } = false;

        public CarriedGroupAssetType AssetTypeIfUnset { get; set; } = CarriedGroupAssetType.None;

        public string? AssetNameIfUnset { get; set; }

        public string? SourceSlotKey { get; set; }

        public bool ApplyDisplaySlotYaw { get; set; } = false;

        public bool ApplyDisplayCaseYawOffset { get; set; } = false;

        public bool ApplyOnDisplayTransform { get; set; } = false;

        public CarriedGroupCandidateSet(IEnumerable<string> groups)
        {
            Groups = groups.ToList().AsReadOnly();
        }
    }
}
