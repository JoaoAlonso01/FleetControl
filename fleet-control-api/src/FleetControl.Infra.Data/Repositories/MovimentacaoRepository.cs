using FleetControl.Domain.Movimentacoes;
using Microsoft.EntityFrameworkCore;

namespace FleetControl.Infra.Data.Repositories;

public sealed class MovimentacaoRepository : IMovimentacaoRepository
{
    private readonly FleetControlDbContext _context;

    public MovimentacaoRepository(FleetControlDbContext context)
    {
        _context = context;
    }

    public async Task<Movimentacao?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        await _context.Movimentacoes.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<Movimentacao>> ListarAsync(CancellationToken cancellationToken)
    {
        var movimentacoes = await _context.Movimentacoes.ToListAsync(cancellationToken);
        return movimentacoes.OrderByDescending(m => m.DataHora).ToList();
    }

    public async Task<IReadOnlyCollection<Movimentacao>> ListarPorVeiculoAsync(Guid veiculoId, CancellationToken cancellationToken)
    {
        var movimentacoes = await _context.Movimentacoes.Where(m => m.VeiculoId == veiculoId).ToListAsync(cancellationToken);
        return movimentacoes.OrderByDescending(m => m.DataHora).ToList();
    }

    public async Task<IReadOnlyCollection<Movimentacao>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken)
    {
        var movimentacoes = await _context.Movimentacoes.Where(m => m.UsuarioId == usuarioId).ToListAsync(cancellationToken);
        return movimentacoes.OrderByDescending(m => m.DataHora).ToList();
    }

    public async Task<Movimentacao?> ObterUltimoCheckOutAberto(Guid veiculoId, CancellationToken cancellationToken)
    {
        var movimentacoes = await _context.Movimentacoes
            .Where(m => m.VeiculoId == veiculoId && m.Tipo == TipoMovimento.CheckOut && m.DataHoraRetorno == null)
            .ToListAsync(cancellationToken);

        return movimentacoes.OrderByDescending(m => m.DataHora).FirstOrDefault();
    }

    public async Task AdicionarAsync(Movimentacao movimentacao, CancellationToken cancellationToken)
    {
        await _context.Movimentacoes.AddAsync(movimentacao, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Movimentacao movimentacao, CancellationToken cancellationToken)
    {
        _context.Movimentacoes.Update(movimentacao);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
