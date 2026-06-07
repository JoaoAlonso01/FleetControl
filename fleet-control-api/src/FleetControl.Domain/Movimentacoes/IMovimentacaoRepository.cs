namespace FleetControl.Domain.Movimentacoes;

public interface IMovimentacaoRepository
{
    Task<Movimentacao?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Movimentacao>> ListarAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Movimentacao>> ListarPorVeiculoAsync(Guid veiculoId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Movimentacao>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken);
    Task<Movimentacao?> ObterUltimoCheckOutAberto(Guid veiculoId, CancellationToken cancellationToken);
    Task AdicionarAsync(Movimentacao movimentacao, CancellationToken cancellationToken);
    Task AtualizarAsync(Movimentacao movimentacao, CancellationToken cancellationToken);
}
