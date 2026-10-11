using System;

namespace CarryOn.API.Common.Models
{
    /// <summary>
    /// Flags restricting where an entity may be placed.
    /// Multiple constraints can be combined via bitwise OR.
    /// </summary>
    [Flags]
    public enum PlacementConstraint
    {
        /// <summary> Placement allowed on any face. </summary>
        None = 0,

        /// <summary> Require a top-face target; drops and side/bottom faces fall back to the unadjusted position. </summary>
        MustBeOnGround = 1,

        /// <summary> Reject bottom-face targets. </summary>
        NoCeilingPlacement = 2,

        /// <summary> Reject side-face targets. </summary>
        NoWallPlacement = 4,
    }
}
