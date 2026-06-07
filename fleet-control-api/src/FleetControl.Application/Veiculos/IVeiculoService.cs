using FleetControl.Domain.Shared;
using FleetControl.Domain.Veiculos;

namespace FleetControl.Application.Veiculos;

public sealed record CriarVeiculoRequest(string Modelo, string Marca, int Ano, string Placa);

public sealed record AtualizarVeiculoRequest(string Modelo, string Marca, int Ano, string Placa);

public sealed record AtualizarKilometragemRequest(int Quilometragem);

public sealed record VeiculoResponse(
    Guid Id,
    string Modelo,
    string Marca,
    int Ano,
    string Placa,
    string Status,
    int QuilometragemAtual,
    DateTimeOffset CriadoEm,
    DateTimeOffset AtualizadoEm
);

public interface IVeiculoService
{
    Task<Result<VeiculoResponse>> CriarAsync(CriarVeiculoRequest request, CancellationToken cancellationToken);
    Task<Result<VeiculoResponse>> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<IReadOnlyCollection<VeiculoResponse>>> ListarAsync(CancellationToken cancellationToken);
    Task<Result> AtualizarAsync(Guid id, AtualizarVeiculoRequest request, CancellationToken cancellationToken);
    Task<Result> AtualizarKilometragemAsync(Guid id, AtualizarKilometragemRequest request, CancellationToken cancellationToken);
    Task<Result> EnviarParaManutencaoAsync(Guid id, CancellationToken cancellationToken);
    Task<Result> RetornarDaManutencaoAsync(Guid id, CancellationToken cancellationToken);
    Task<Result> DesativarAsync(Guid id, CancellationToken cancellationToken);
}
