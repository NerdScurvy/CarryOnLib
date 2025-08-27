using Vintagestory.API.Common;

namespace CarryOn.Common.Behaviors
{
    public interface IConditionalBlockBehavior
    {
        string EnabledCondition { get; set; }

        void ProcessConditions(ICoreAPI api, Block block);
    }
}