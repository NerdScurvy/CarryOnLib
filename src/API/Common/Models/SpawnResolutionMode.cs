namespace CarryOn.API.Common.Models
{
    /// <summary>
    /// Controls what <see cref="SpawnPositionHelper.GetAdjustedSpawnPosition"/>
    /// returns when the entity cannot be fully cleared of collisions after
    /// depenetration.
    /// </summary>
    public enum SpawnResolutionMode
    {
        /// <summary>
        /// Return the best-effort position even if the entity is still
        /// partially intersecting terrain or another entity.
        /// </summary>
        BestEffort = 0,

        /// <summary>
        /// Return null when the entity cannot be fully cleared, signalling
        /// the caller to abort spawning (e.g. return a null entity and
        /// report insufficient space to the player).
        /// </summary>
        RequireClearSpace = 1,
    }
}
