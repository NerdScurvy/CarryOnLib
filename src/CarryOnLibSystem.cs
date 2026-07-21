using CarryOn.API.Common.Interfaces;
using Vintagestory.API.Common;

namespace CarryOn.CarryOnLib
{
    public class CarryOnLibSystem : ModSystem
    {
        /// <summary>
        /// The active carry manager instance, populated by the CarryOn mod during initialization.
        /// Returns null if CarryOn is not loaded.
        /// </summary>
        /// <remarks>
        /// This property should not be set by consuming mods. It is populated automatically
        /// by the CarryOn mod when it initializes.
        /// </remarks>
        public ICarryManager? CarryManager { get; set; }
    }
}