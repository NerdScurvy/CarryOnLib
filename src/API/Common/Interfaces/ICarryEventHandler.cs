namespace CarryOn.API.Common.Interfaces
{
    /// <summary>
    /// Bootstrap interface for event subscribers. Implement this to register event handlers
    /// during carry system initialization.
    /// </summary>
    public interface ICarryEventHandler
    {
        void Init(ICarryManager carryManager);
    }
}