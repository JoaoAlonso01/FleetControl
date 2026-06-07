using FleetControl.Application.Manutencoes;
using FleetControl.Domain.Manutencoes;
using FleetControl.Domain.Shared;
using FleetControl.Domain.Veiculos;

namespace FleetControl.Application.Services.Manutencoes;

public sealed class ManutencaoService : IManutencaoService
{
    private readonly IManutencaoRepository _repository;
    private readonly IVeiculoRepository _veiculoRepository;

    public ManutencaoService(IManutencaoRepository repository, IVeiculoRepository veiculoRepository)
    {
        _repository = repository;
        _veiculoRepository = veiculoRepository;
    }

    public async Task<Result<ManutencaoResponse>> AgendarAsync(AgendarManutencaoRequest request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<TipoManutencao>(request.Tipo, ignoreCase: true, out var tipo))
            return Result<ManutencaoResponse>.Failure($"Tipo '{request.Tipo}' inválido. Use: Preventiva, Corretiva ou Revisao.");

        if (request.DataAgendada <= DateTimeOffset.UtcNow)
            return Result<ManutencaoResponse>.Failure("Data agendada deve ser no futuro.");

        var veiculo = await _veiculoRepository.ObterPorIdAsync(request.VeiculoId, cancellationToken);
        if (veiculo is null)
            return Result<ManutencaoResponse>.NotFound($"Veículo '{request.VeiculoId}' não encontrado.");

        Manutencao manutencao;
        try
        {
            manutencao = Manutencao.Agendar(request.VeiculoId, tipo, request.Descricao, request.DataAgendada, request.Oficina, request.CustoEstimado);
            veiculo.EnviarParaManutencao();
        }
        catch (ManutencaoDomainException ex)
        {
            return Result<ManutencaoResponse>.Failure(ex.Message);
        }
        catch (VeiculoDomainException ex)
        {
            return Result<ManutencaoResponse>.Failure(ex.Message);
        }

        await _repository.AdicionarAsync(manutencao, cancellationToken);
        await _veiculoRepository.AtualizarAsync(veiculo, cancellationToken);

        return Result<ManutencaoResponse>.Success(MapToResponse(manutencao, veiculo.Placa.ToString()));
    }

    public async Task<Result<ManutencaoResponse>> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var manutencao = await _repository.ObterPorIdAsync(id, cancellationToken);
        if (manutencao is null)
            return Result<ManutencaoResponse>.NotFound($"Manutenção '{id}' não encontrada.");

        var veiculo = await _veiculoRepository.ObterPorIdAsync(manutencao.VeiculoId, cancellationToken);
        return Result<ManutencaoResponse>.Success(MapToResponse(manutencao, veiculo?.Placa.ToString() ?? ""));
    }

    public async Task<Result<IReadOnlyCollection<ManutencaoResponse>>> ListarAsync(CancellationToken cancellationToken)
    {
        var manutencoes = await _repository.ListarAsync(cancellationToken);
        var veiculos = await _veiculoRepository.ListarAsync(cancellationToken);
        var veiculoDict = veiculos.ToDictionary(v => v.Id, v => v.Placa.ToString());

        var response = manutencoes.Select(m => MapToResponse(m, veiculoDict.GetValueOrDefault(m.VeiculoId, ""))).ToList().AsReadOnly();
        return Result<IReadOnlyCollection<ManutencaoResponse>>.Success(response);
    }

    public async Task<Result> IniciarAsync(Guid id, CancellationToken cancellationToken)
    {
        var manutencao = await _repository.ObterPorIdAsync(id, cancellationToken);
        if (manutencao is null)
            return Result.NotFound($"Manutenção '{id}' não encontrada.");

        try
        {
            manutencao.Iniciar();
        }
        catch (ManutencaoDomainException ex)
        {
            return Result.Failure(ex.Message);
        }
        await _repository.AtualizarAsync(manutencao, cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ConcluirAsync(Guid id, ConcluirManutencaoRequest request, CancellationToken cancellationToken)
    {
        var manutencao = await _repository.ObterPorIdAsync(id, cancellationToken);
        if (manutencao is null)
            return Result.NotFound($"Manutenção '{id}' não encontrada.");

        var veiculo = await _veiculoRepository.ObterPorIdAsync(manutencao.VeiculoId, cancellationToken);
        try
        {
            manutencao.Concluir(request.CustoReal);
            veiculo?.RetornarDaManutencao();
        }
        catch (ManutencaoDomainException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (VeiculoDomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _repository.AtualizarAsync(manutencao, cancellationToken);
        if (veiculo is not null)
            await _veiculoRepository.AtualizarAsync(veiculo, cancellationToken);

        return Result.Success();
    }

    public async Task<Result> CancelarAsync(Guid id, CancellationToken cancellationToken)
    {
        var manutencao = await _repository.ObterPorIdAsync(id, cancellationToken);
        if (manutencao is null)
            return Result.NotFound($"Manutenção '{id}' não encontrada.");

        var veiculo = await _veiculoRepository.ObterPorIdAsync(manutencao.VeiculoId, cancellationToken);
        try
        {
            manutencao.Cancelar();
            if (veiculo?.Status == StatusVeiculo.EmManutencao)
                veiculo.RetornarDaManutencao();
        }
        catch (ManutencaoDomainException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (VeiculoDomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _repository.AtualizarAsync(manutencao, cancellationToken);
        if (veiculo is not null)
            await _veiculoRepository.AtualizarAsync(veiculo, cancellationToken);

        return Result.Success();
    }

    private static ManutencaoResponse MapToResponse(Manutencao m, string placaVeiculo) =>
        new(m.Id, m.VeiculoId, placaVeiculo, m.Tipo.ToString(), m.Status.ToString(),
            m.Descricao, m.Oficina, m.CustoEstimado, m.CustoReal, m.DataAgendada, m.DataConclusao);
}
