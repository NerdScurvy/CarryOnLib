using Vintagestory.API.Common;

namespace CarryOn.API.Common.Interfaces
{
    public interface IConditionalBlockBehavior
    {
        string EnabledCondition { get; set; }

        void ProcessConditions(ICoreAPI api, Block block);
    }
}