using System;
using Vintagestory.API.MathTools;

namespace CarryOn.API.Event.Data
{
    public class BlockRemovedEventArgs : EventArgs
    {
        public BlockPos? Position { get; set; }
    }
}