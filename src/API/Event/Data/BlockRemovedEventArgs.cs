using System;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace CarryOn.API.Event.Data
{
    public class BlockRemovedEventArgs : EventArgs
    {
        public IWorldAccessor World { get; set; }

        public BlockPos Position  {get;set;}
    }
}