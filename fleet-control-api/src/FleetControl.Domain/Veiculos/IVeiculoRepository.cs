namespace FleetControl.Domain.Veiculos;

public interface IVeiculoRepository
{
    Task<Veiculo?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Veiculo?> ObterPorPlacaAsync(string placa, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Veiculo>> ListarAsync(CancellationToken cancellationToken);
    Task AdicionarAsync(Veiculo veiculo, CancellationToken cancellationToken);
    Task AtualizarAsync(Veiculo veiculo, CancellationToken cancellationToken);
}
