using FleetControl.Domain.Shared;

namespace FleetControl.Domain.Veiculos;

public sealed class VeiculoDomainException : DomainException
{
    public VeiculoDomainException(string message) : base(message) { }
}

public sealed record VeiculoRegistradoEvent(Guid VeiculoId, string Placa) : IDomainEvent;
