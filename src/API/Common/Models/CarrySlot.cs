namespace CarryOn.API.Common.Models
{
    /// <summary> Represents a location where something is or may be carried. </summary>
    public enum CarrySlot
    {
        /// <summary> Block carried in the player's hands (primary carry slot). </summary>
        Hands,

        /// <summary> Block carried on the player's back (secondary carry slot). </summary>
        Back,

        /// <summary> Child block attached to a primary carried block (cluster/attachment system). </summary>
        Attached
    }
}
