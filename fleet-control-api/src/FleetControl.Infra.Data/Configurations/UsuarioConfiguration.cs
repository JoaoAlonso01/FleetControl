using FleetControl.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetControl.Infra.Data.Configurations;

public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Nome).IsRequired().HasMaxLength(150);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(200);
        builder.Property(u => u.Perfil).IsRequired().HasConversion<int>();
        builder.Property(u => u.Ativo).IsRequired();
        builder.Property(u => u.Telefone).HasMaxLength(20);
        builder.Property(u => u.NumeroCnh).HasMaxLength(20);
        builder.Property(u => u.CriadoEm).IsRequired();
        builder.Property(u => u.AtualizadoEm).IsRequired();

        builder.HasIndex(u => u.Email).IsUnique();
    }
}
