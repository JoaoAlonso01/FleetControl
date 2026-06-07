using FleetControl.Domain.Shared;

namespace FleetControl.Application.Movimentacoes;

public sealed record RegistrarCheckOutRequest(Guid VeiculoId, Guid UsuarioId, int QuilometragemSaida, string? Destino, string? Observacao);
public sealed record RegistrarCheckInRequest(Guid VeiculoId, Guid UsuarioId, int QuilometragemRetorno, string? Observacao);

public sealed record MovimentacaoResponse(
    Guid Id,
    Guid VeiculoId,
    string VeiculoPlaca,
    Guid UsuarioId,
    string UsuarioNome,
    string Tipo,
    int QuilometragemSaida,
    int? QuilometragemRetorno,
    string? Destino,
    string? Observacao,
    DateTimeOffset DataCheckOut,
    DateTimeOffset? DataCheckIn
);

public interface IMovimentacaoService
{
    Task<Result<MovimentacaoResponse>> RegistrarCheckOutAsync(RegistrarCheckOutRequest request, CancellationToken cancellationToken);
    Task<Result<MovimentacaoResponse>> RegistrarCheckInAsync(RegistrarCheckInRequest request, CancellationToken cancellationToken);
    Task<Result<IReadOnlyCollection<MovimentacaoResponse>>> ListarAsync(CancellationToken cancellationToken);
    Task<Result<MovimentacaoResponse>> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
}
