using CarryOn.Utility;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace CarryOn.Common.Behaviors
{
    /// <summary>
    /// Abstract BlockBehavior with an EnabledCondition property for conditional behaviors.
    /// </summary>
    public abstract class BlockBehaviorConditional : BlockBehavior
    {
        /// <summary>
        /// The enabled condition for this behavior. Nullable bool: true/false/null (unset).
        /// </summary>
        public string EnabledCondition { get; protected set; }

        protected BlockBehaviorConditional(Block block) : base(block) { }

        public override void Initialize(JsonObject properties)
        {
            base.Initialize(properties);
            if (JsonHelper.TryGetString(properties, "enabledCondition", out var e)) EnabledCondition = e;
        }
    }
}
