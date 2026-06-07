using FleetControl.Domain.Shared;
using FleetControl.Domain.Manutencoes;

namespace FleetControl.Application.Manutencoes;

public sealed record AgendarManutencaoRequest(
    Guid VeiculoId,
    string Tipo,
    string Descricao,
    DateTimeOffset DataAgendada,
    string? Oficina,
    decimal? CustoEstimado
);

public sealed record ConcluirManutencaoRequest(decimal CustoReal);

public sealed record ManutencaoResponse(
    Guid Id,
    Guid VeiculoId,
    string VeiculoPlaca,
    string Tipo,
    string Status,
    string Descricao,
    string? Oficina,
    decimal? CustoEstimado,
    decimal? CustoReal,
    DateTimeOffset DataAgendada,
    DateTimeOffset? DataConclusao
);

public interface IManutencaoService
{
    Task<Result<ManutencaoResponse>> AgendarAsync(AgendarManutencaoRequest request, CancellationToken cancellationToken);
    Task<Result<ManutencaoResponse>> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<IReadOnlyCollection<ManutencaoResponse>>> ListarAsync(CancellationToken cancellationToken);
    Task<Result> IniciarAsync(Guid id, CancellationToken cancellationToken);
    Task<Result> ConcluirAsync(Guid id, ConcluirManutencaoRequest request, CancellationToken cancellationToken);
    Task<Result> CancelarAsync(Guid id, CancellationToken cancellationToken);
}
