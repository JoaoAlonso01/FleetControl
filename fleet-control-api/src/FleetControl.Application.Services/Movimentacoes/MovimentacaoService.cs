using FleetControl.Application.Movimentacoes;
using FleetControl.Domain.Movimentacoes;
using FleetControl.Domain.Shared;
using FleetControl.Domain.Usuarios;
using FleetControl.Domain.Veiculos;

namespace FleetControl.Application.Services.Movimentacoes;

public sealed class MovimentacaoService : IMovimentacaoService
{
    private readonly IMovimentacaoRepository _repository;
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public MovimentacaoService(
        IMovimentacaoRepository repository,
        IVeiculoRepository veiculoRepository,
        IUsuarioRepository usuarioRepository)
    {
        _repository = repository;
        _veiculoRepository = veiculoRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<Result<MovimentacaoResponse>> RegistrarCheckOutAsync(RegistrarCheckOutRequest request, CancellationToken cancellationToken)
    {
        var veiculo = await _veiculoRepository.ObterPorIdAsync(request.VeiculoId, cancellationToken);
        if (veiculo is null)
            return Result<MovimentacaoResponse>.NotFound($"Veículo '{request.VeiculoId}' não encontrado.");

        var usuario = await _usuarioRepository.ObterPorIdAsync(request.UsuarioId, cancellationToken);
        if (usuario is null)
            return Result<MovimentacaoResponse>.NotFound($"Usuário '{request.UsuarioId}' não encontrado.");

        veiculo.IniciarUso();

        var movimentacao = Movimentacao.RegistrarCheckOut(
            request.VeiculoId, request.UsuarioId, request.QuilometragemSaida, request.Destino, request.Observacao);

        await _repository.AdicionarAsync(movimentacao, cancellationToken);
        await _veiculoRepository.AtualizarAsync(veiculo, cancellationToken);

        return Result<MovimentacaoResponse>.Success(MapToResponse(movimentacao, veiculo.Placa.ToString(), usuario.Nome));
    }

    public async Task<Result<MovimentacaoResponse>> RegistrarCheckInAsync(RegistrarCheckInRequest request, CancellationToken cancellationToken)
    {
        var veiculo = await _veiculoRepository.ObterPorIdAsync(request.VeiculoId, cancellationToken);
        if (veiculo is null)
            return Result<MovimentacaoResponse>.NotFound($"Veículo '{request.VeiculoId}' não encontrado.");

        var usuario = await _usuarioRepository.ObterPorIdAsync(request.UsuarioId, cancellationToken);
        if (usuario is null)
            return Result<MovimentacaoResponse>.NotFound($"Usuário '{request.UsuarioId}' não encontrado.");

        var movimentacao = await _repository.ObterUltimoCheckOutAberto(request.VeiculoId, cancellationToken);
        if (movimentacao is null)
            return Result<MovimentacaoResponse>.NotFound($"Check-out aberto para o veÃ­culo '{request.VeiculoId}' nÃ£o encontrado.");

        movimentacao.RegistrarRetorno(request.QuilometragemRetorno, request.Observacao);
        veiculo.FinalizarUso();
        veiculo.AtualizarKilometragem(request.QuilometragemRetorno);

        await _repository.AtualizarAsync(movimentacao, cancellationToken);
        await _veiculoRepository.AtualizarAsync(veiculo, cancellationToken);

        return Result<MovimentacaoResponse>.Success(MapToResponse(movimentacao, veiculo.Placa.ToString(), usuario.Nome));
    }

    public async Task<Result<IReadOnlyCollection<MovimentacaoResponse>>> ListarAsync(CancellationToken cancellationToken)
    {
        var movimentacoes = await _repository.ListarAsync(cancellationToken);
        var veiculos = await _veiculoRepository.ListarAsync(cancellationToken);
        var usuarios = await _usuarioRepository.ListarAsync(cancellationToken);

        var veiculoDict = veiculos.ToDictionary(v => v.Id, v => v.Placa.ToString());
        var usuarioDict = usuarios.ToDictionary(u => u.Id, u => u.Nome);

        var response = movimentacoes
            .Select(m => MapToResponse(m, veiculoDict.GetValueOrDefault(m.VeiculoId, ""), usuarioDict.GetValueOrDefault(m.UsuarioId, "")))
            .ToList().AsReadOnly();

        return Result<IReadOnlyCollection<MovimentacaoResponse>>.Success(response);
    }

    public async Task<Result<MovimentacaoResponse>> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var movimentacao = await _repository.ObterPorIdAsync(id, cancellationToken);
        if (movimentacao is null)
            return Result<MovimentacaoResponse>.NotFound($"Movimentação '{id}' não encontrada.");

        var veiculo = await _veiculoRepository.ObterPorIdAsync(movimentacao.VeiculoId, cancellationToken);
        var usuario = await _usuarioRepository.ObterPorIdAsync(movimentacao.UsuarioId, cancellationToken);

        return Result<MovimentacaoResponse>.Success(
            MapToResponse(movimentacao, veiculo?.Placa.ToString() ?? "", usuario?.Nome ?? ""));
    }

    private static MovimentacaoResponse MapToResponse(Movimentacao m, string placa, string nomeUsuario) =>
        new(m.Id, m.VeiculoId, placa, m.UsuarioId, nomeUsuario, m.Tipo.ToString(),
            m.KilometragemSaida, m.KilometragemRetorno, m.Destino, m.Observacao, m.DataHora, m.DataHoraRetorno);
}
