using CarryOn.API.Common.Interfaces;

namespace CarryOn.API.Common.Models
{
    public sealed class RegisteredTransformGroupResolver
    {
        public string? ModId { get; init; }

        public string? ResolverCode { get; init; }

        public ICarriedTransformGroupResolver? Resolver { get; init; }
    }
}