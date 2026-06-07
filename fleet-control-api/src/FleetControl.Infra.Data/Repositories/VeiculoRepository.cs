using FleetControl.Domain.Veiculos;
using Microsoft.EntityFrameworkCore;

namespace FleetControl.Infra.Data.Repositories;

public sealed class VeiculoRepository : IVeiculoRepository
{
    private readonly FleetControlDbContext _context;

    public VeiculoRepository(FleetControlDbContext context)
    {
        _context = context;
    }

    public async Task<Veiculo?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        await _context.Veiculos.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

    public async Task<Veiculo?> ObterPorPlacaAsync(string placa, CancellationToken cancellationToken)
    {
        var normalizado = placa.Trim().ToUpperInvariant().Replace("-", "");
        return await _context.Veiculos.FirstOrDefaultAsync(v => v.Placa.Valor == normalizado, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Veiculo>> ListarAsync(CancellationToken cancellationToken) =>
        await _context.Veiculos.OrderBy(v => v.Modelo).ToListAsync(cancellationToken);

    public async Task AdicionarAsync(Veiculo veiculo, CancellationToken cancellationToken)
    {
        await _context.Veiculos.AddAsync(veiculo, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Veiculo veiculo, CancellationToken cancellationToken)
    {
        _context.Veiculos.Update(veiculo);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
