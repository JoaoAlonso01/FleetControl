using FleetControl.Domain.Veiculos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetControl.Infra.Data.Configurations;

public sealed class VeiculoConfiguration : IEntityTypeConfiguration<Veiculo>
{
    public void Configure(EntityTypeBuilder<Veiculo> builder)
    {
        builder.ToTable("Veiculos");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Modelo).IsRequired().HasMaxLength(100);
        builder.Property(v => v.Marca).IsRequired().HasMaxLength(100);
        builder.Property(v => v.Ano).IsRequired();
        builder.Property(v => v.Status).IsRequired().HasConversion<int>();
        builder.Property(v => v.KilometragemAtual).IsRequired();
        builder.Property(v => v.CriadoEm).IsRequired();
        builder.Property(v => v.AtualizadoEm).IsRequired();

        builder.OwnsOne(v => v.Placa, placa =>
        {
            placa.Property(p => p.Valor)
                 .HasColumnName("PlacaValor")
                 .IsRequired()
                 .HasMaxLength(10);

            placa.HasIndex(p => p.Valor).IsUnique();
        });
    }
}
