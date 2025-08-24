using System;
using CarryOn.API.Common;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace CarryOn.API.Event.Data
{
    public class BlockDroppedEventArgs : EventArgs
    {
        public Entity Entity { get; set; }
        public BlockPos Position { get; set; }
        public CarriedBlock CarriedBlock { get; set; }
        public bool Destroyed { get; set; }
        public bool HadContents { get; set; }
        public bool BlockPlaced { get; set; }
    }
}