using Vintagestory.API.Common;

namespace CarryOn.API.Common.Interfaces
{
    /// <summary>
    /// Marker interface for block behaviors that should only be active when a specific
    /// condition is met. The mod system calls <see cref="ProcessConditions"/> during
    /// loading so behaviors can enable or disable themselves based on world configuration.
    /// </summary>
    public interface IConditionalBlockBehavior
    {
        string EnabledCondition { get; set; }

        void ProcessConditions(ICoreAPI api, Block block);
    }
}