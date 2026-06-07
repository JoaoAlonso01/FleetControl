using FleetControl.Application.Veiculos;
using FleetControl.Domain.Shared;
using FleetControl.Domain.Veiculos;

namespace FleetControl.Application.Services.Veiculos;

public sealed class VeiculoService : IVeiculoService
{
    private readonly IVeiculoRepository _repository;

    public VeiculoService(IVeiculoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<VeiculoResponse>> CriarAsync(CriarVeiculoRequest request, CancellationToken cancellationToken)
    {
        var existente = await _repository.ObterPorPlacaAsync(request.Placa, cancellationToken);
        if (existente is not null)
            return Result<VeiculoResponse>.Failure($"Já existe um veículo cadastrado com a placa '{request.Placa}'.", 409);

        var veiculo = Veiculo.Criar(request.Modelo, request.Marca, request.Ano, request.Placa);
        await _repository.AdicionarAsync(veiculo, cancellationToken);

        return Result<VeiculoResponse>.Success(MapToResponse(veiculo));
    }

    public async Task<Result<VeiculoResponse>> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var veiculo = await _repository.ObterPorIdAsync(id, cancellationToken);
        if (veiculo is null)
            return Result<VeiculoResponse>.NotFound($"Veículo '{id}' não encontrado.");

        return Result<VeiculoResponse>.Success(MapToResponse(veiculo));
    }

    public async Task<Result<IReadOnlyCollection<VeiculoResponse>>> ListarAsync(CancellationToken cancellationToken)
    {
        var veiculos = await _repository.ListarAsync(cancellationToken);
        var response = veiculos.Select(MapToResponse).ToList().AsReadOnly();
        return Result<IReadOnlyCollection<VeiculoResponse>>.Success(response);
    }

    public async Task<Result> AtualizarAsync(Guid id, AtualizarVeiculoRequest request, CancellationToken cancellationToken)
    {
        var veiculo = await _repository.ObterPorIdAsync(id, cancellationToken);
        if (veiculo is null)
            return Result.NotFound($"VeÃ­culo '{id}' nÃ£o encontrado.");

        var existente = await _repository.ObterPorPlacaAsync(request.Placa, cancellationToken);
        if (existente is not null && existente.Id != id)
            return Result.Failure($"JÃ¡ existe um veÃ­culo cadastrado com a placa '{request.Placa}'.", 409);

        veiculo.AtualizarDados(request.Modelo, request.Marca, request.Ano, request.Placa);
        await _repository.AtualizarAsync(veiculo, cancellationToken);

        return Result.Success();
    }

    public async Task<Result> AtualizarKilometragemAsync(Guid id, AtualizarKilometragemRequest request, CancellationToken cancellationToken)
    {
        var veiculo = await _repository.ObterPorIdAsync(id, cancellationToken);
        if (veiculo is null)
            return Result.NotFound($"Veículo '{id}' não encontrado.");

        veiculo.AtualizarKilometragem(request.Quilometragem);
        await _repository.AtualizarAsync(veiculo, cancellationToken);

        return Result.Success();
    }

    public async Task<Result> EnviarParaManutencaoAsync(Guid id, CancellationToken cancellationToken)
    {
        var veiculo = await _repository.ObterPorIdAsync(id, cancellationToken);
        if (veiculo is null)
            return Result.NotFound($"Veículo '{id}' não encontrado.");

        veiculo.EnviarParaManutencao();
        await _repository.AtualizarAsync(veiculo, cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RetornarDaManutencaoAsync(Guid id, CancellationToken cancellationToken)
    {
        var veiculo = await _repository.ObterPorIdAsync(id, cancellationToken);
        if (veiculo is null)
            return Result.NotFound($"Veículo '{id}' não encontrado.");

        veiculo.RetornarDaManutencao();
        await _repository.AtualizarAsync(veiculo, cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DesativarAsync(Guid id, CancellationToken cancellationToken)
    {
        var veiculo = await _repository.ObterPorIdAsync(id, cancellationToken);
        if (veiculo is null)
            return Result.NotFound($"Veículo '{id}' não encontrado.");

        veiculo.Desativar();
        await _repository.AtualizarAsync(veiculo, cancellationToken);

        return Result.Success();
    }

    private static VeiculoResponse MapToResponse(Veiculo v) =>
        new(v.Id, v.Modelo, v.Marca, v.Ano, v.Placa.ToString(), v.Status.ToString(), v.KilometragemAtual, v.CriadoEm, v.AtualizadoEm);
}
