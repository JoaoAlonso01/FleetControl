using FleetControl.Domain.Shared;

namespace FleetControl.Domain.Manutencoes;

public sealed class ManutencaoDomainException : DomainException
{
    public ManutencaoDomainException(string message) : base(message) { }
}

public sealed record ManutencaoAgendadaEvent(Guid ManutencaoId, Guid VeiculoId, DateTimeOffset DataAgendada) : IDomainEvent;
