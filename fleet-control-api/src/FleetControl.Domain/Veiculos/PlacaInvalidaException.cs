using FleetControl.Domain.Shared;

namespace FleetControl.Domain.Veiculos;

public sealed class PlacaInvalidaException : DomainException
{
    public PlacaInvalidaException(string message) : base(message) { }
}
