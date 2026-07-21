using System;

namespace CarryOn.API.Common.Models
{
    /// <summary>
    /// Flags indicating which interaction hints to display to the player.
    /// Multiple hints can be combined via bitwise OR.
    /// </summary>
    [Flags]
    public enum CarryHintType
    {
        /// <summary> No hints displayed. </summary>
        None = 0,

        /// <summary> Hint for standard pickup interaction (interact with empty hand on a carryable block). </summary>
        BasePickup = 1,

        /// <summary> Hint for forced pickup override (e.g., sneak + interact to bypass normal restrictions). </summary>
        ForcePickup = 2,

        /// <summary> Hint for transferring items into a carried block's inventory (e.g., shelving, displays). </summary>
        TransferPut = 4,

        /// <summary> Hint for taking items from a carried block's inventory. </summary>
        TransferTake = 8,
    }
}