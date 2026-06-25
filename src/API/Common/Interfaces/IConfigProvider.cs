using CarryOn.API.Common.Models;

namespace CarryOn.API.Common.Interfaces
{
    public interface IConfigProvider
    {
        CarryOnConfig Config { get; }
    }
}
