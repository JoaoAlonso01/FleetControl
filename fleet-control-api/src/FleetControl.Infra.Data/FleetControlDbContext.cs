using FleetControl.Domain.Manutencoes;
using FleetControl.Domain.Movimentacoes;
using FleetControl.Domain.Usuarios;
using FleetControl.Domain.Veiculos;
using Microsoft.EntityFrameworkCore;

namespace FleetControl.Infra.Data;

public sealed class FleetControlDbContext : DbContext
{
    public FleetControlDbContext(DbContextOptions<FleetControlDbContext> options) : base(options) { }

    public DbSet<Veiculo> Veiculos => Set<Veiculo>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Manutencao> Manutencoes => Set<Manutencao>();
    public DbSet<Movimentacao> Movimentacoes => Set<Movimentacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FleetControlDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
