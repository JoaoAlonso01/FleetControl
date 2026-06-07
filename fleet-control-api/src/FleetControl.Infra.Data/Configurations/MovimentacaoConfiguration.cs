using FleetControl.Domain.Movimentacoes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetControl.Infra.Data.Configurations;

public sealed class MovimentacaoConfiguration : IEntityTypeConfiguration<Movimentacao>
{
    public void Configure(EntityTypeBuilder<Movimentacao> builder)
    {
        builder.ToTable("Movimentacoes");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.VeiculoId).IsRequired();
        builder.Property(m => m.UsuarioId).IsRequired();
        builder.Property(m => m.Tipo).IsRequired().HasConversion<int>();
        builder.Property(m => m.KilometragemSaida).IsRequired();
        builder.Property(m => m.Destino).HasMaxLength(300);
        builder.Property(m => m.Observacao).HasMaxLength(500);
        builder.Property(m => m.DataHora).IsRequired();
        builder.Property(m => m.CriadoEm).IsRequired();

        builder.HasIndex(m => m.VeiculoId);
        builder.HasIndex(m => m.UsuarioId);
    }
}
