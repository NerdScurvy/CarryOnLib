using System;
using CarryOn.API.Common.Models;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace CarryOn.API.Event.Data
{
    public class BlockDroppedEventArgs : EventArgs
    {
        public Entity Entity { get; init; } = null!;
        public BlockPos Position { get; init; } = null!;
        public CarriedBlock CarriedBlock { get; init; } = null!;
        public bool Destroyed { get; init; }
        public bool HadContents { get; init; }
        public bool BlockPlaced { get; init; }
    }
}