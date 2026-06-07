using FleetControl.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace FleetControl.Infra.Data.Repositories;

public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly FleetControlDbContext _context;

    public UsuarioRepository(FleetControlDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken) =>
        await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);

    public async Task<IReadOnlyCollection<Usuario>> ListarAsync(CancellationToken cancellationToken) =>
        await _context.Usuarios.OrderBy(u => u.Nome).ToListAsync(cancellationToken);

    public async Task AdicionarAsync(Usuario usuario, CancellationToken cancellationToken)
    {
        await _context.Usuarios.AddAsync(usuario, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Usuario usuario, CancellationToken cancellationToken)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
