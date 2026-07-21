using System.Collections.Generic;
using System.Linq;

namespace CarryOn.API.Common.Models
{
    public class AttachmentResolveResult
    {
        public IReadOnlyList<CarriedGroupCandidateSet> Candidates { get; }

        public bool EnableVertexWarp { get; set; }

        public AttachmentResolveResult(IEnumerable<CarriedGroupCandidateSet> candidates)
        {
            Candidates = candidates.ToList().AsReadOnly();
        }
    }
}
