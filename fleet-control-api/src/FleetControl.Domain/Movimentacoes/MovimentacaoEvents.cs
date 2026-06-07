using FleetControl.Domain.Shared;

namespace FleetControl.Domain.Movimentacoes;

public sealed class MovimentacaoDomainException : DomainException
{
    public MovimentacaoDomainException(string message) : base(message) { }
}

public sealed record MovimentoRegistradoEvent(Guid MovimentacaoId, Guid VeiculoId, Guid UsuarioId, TipoMovimento Tipo) : IDomainEvent;
