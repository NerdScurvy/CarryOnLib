using CarryOn.API.Common.Models;

namespace CarryOn.API.Common.Interfaces
{
    /// <summary>
    /// Optional policy hook that allows transfer behaviors to dynamically filter which interaction hints are shown.
    /// Implementing this interface does not affect carry behavior logic; it only controls hint visibility.
    /// </summary>
    public interface ICarryableHintPolicy
    {
        CarryHintType GetAllowedHints(CarryHintContext context, CarryHintType defaultHints);
    }
}