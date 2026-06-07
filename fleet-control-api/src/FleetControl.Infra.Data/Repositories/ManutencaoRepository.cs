using FleetControl.Domain.Manutencoes;
using Microsoft.EntityFrameworkCore;

namespace FleetControl.Infra.Data.Repositories;

public sealed class ManutencaoRepository : IManutencaoRepository
{
    private readonly FleetControlDbContext _context;

    public ManutencaoRepository(FleetControlDbContext context)
    {
        _context = context;
    }

    public async Task<Manutencao?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        await _context.Manutencoes.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<Manutencao>> ListarAsync(CancellationToken cancellationToken)
    {
        var manutencoes = await _context.Manutencoes.ToListAsync(cancellationToken);
        return manutencoes.OrderByDescending(m => m.DataAgendada).ToList();
    }

    public async Task<IReadOnlyCollection<Manutencao>> ListarPorVeiculoAsync(Guid veiculoId, CancellationToken cancellationToken)
    {
        var manutencoes = await _context.Manutencoes.Where(m => m.VeiculoId == veiculoId).ToListAsync(cancellationToken);
        return manutencoes.OrderByDescending(m => m.DataAgendada).ToList();
    }

    public async Task AdicionarAsync(Manutencao manutencao, CancellationToken cancellationToken)
    {
        await _context.Manutencoes.AddAsync(manutencao, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Manutencao manutencao, CancellationToken cancellationToken)
    {
        _context.Manutencoes.Update(manutencao);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
