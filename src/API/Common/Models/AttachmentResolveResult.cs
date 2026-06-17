using System.Collections.Generic;

namespace CarryOn.API.Common.Models
{
    public class AttachmentResolveResult
    {
        public IList<CarriedGroupCandidateSet> Candidates { get; set; } = new List<CarriedGroupCandidateSet>();

        public bool EnableVertexWarp { get; set; }
    }
}
