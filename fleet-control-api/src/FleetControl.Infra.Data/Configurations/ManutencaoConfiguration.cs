using FleetControl.Domain.Manutencoes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetControl.Infra.Data.Configurations;

public sealed class ManutencaoConfiguration : IEntityTypeConfiguration<Manutencao>
{
    public void Configure(EntityTypeBuilder<Manutencao> builder)
    {
        builder.ToTable("Manutencoes");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.VeiculoId).IsRequired();
        builder.Property(m => m.Tipo).IsRequired().HasConversion<int>();
        builder.Property(m => m.Status).IsRequired().HasConversion<int>();
        builder.Property(m => m.Descricao).IsRequired().HasMaxLength(500);
        builder.Property(m => m.Oficina).HasMaxLength(200);
        builder.Property(m => m.CustoEstimado).HasColumnType("decimal(10,2)");
        builder.Property(m => m.CustoReal).HasColumnType("decimal(10,2)");
        builder.Property(m => m.DataAgendada).IsRequired();
        builder.Property(m => m.CriadoEm).IsRequired();
        builder.Property(m => m.AtualizadoEm).IsRequired();

        builder.HasIndex(m => m.VeiculoId);
    }
}
