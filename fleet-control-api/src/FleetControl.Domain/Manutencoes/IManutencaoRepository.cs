namespace FleetControl.Domain.Manutencoes;

public interface IManutencaoRepository
{
    Task<Manutencao?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Manutencao>> ListarAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Manutencao>> ListarPorVeiculoAsync(Guid veiculoId, CancellationToken cancellationToken);
    Task AdicionarAsync(Manutencao manutencao, CancellationToken cancellationToken);
    Task AtualizarAsync(Manutencao manutencao, CancellationToken cancellationToken);
}
