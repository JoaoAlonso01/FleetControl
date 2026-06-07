using FleetControl.Domain.Shared;

namespace FleetControl.Domain.Usuarios;

public sealed class Usuario : AggregateRoot
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public PerfilUsuario Perfil { get; private set; }
    public bool Ativo { get; private set; }
    public string? Telefone { get; private set; }
    public string? NumeroCnh { get; private set; }
    public DateTimeOffset CriadoEm { get; private set; }
    public DateTimeOffset AtualizadoEm { get; private set; }

    private Usuario() { }

    public static Usuario Criar(string nome, string email, PerfilUsuario perfil, string? telefone = null, string? numeroCnh = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new UsuarioDomainException("Nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new UsuarioDomainException("Email inválido.");

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = nome.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            Perfil = perfil,
            Ativo = true,
            Telefone = telefone?.Trim(),
            NumeroCnh = numeroCnh?.Trim(),
            CriadoEm = DateTimeOffset.UtcNow,
            AtualizadoEm = DateTimeOffset.UtcNow
        };

        usuario.AddDomainEvent(new UsuarioCriadoEvent(usuario.Id, email));
        return usuario;
    }

    public void Atualizar(string nome, string? telefone, string? numeroCnh)
    {
        Nome = nome.Trim();
        Telefone = telefone?.Trim();
        NumeroCnh = numeroCnh?.Trim();
        AtualizadoEm = DateTimeOffset.UtcNow;
    }

    public void AlterarPerfil(PerfilUsuario perfil)
    {
        Perfil = perfil;
        AtualizadoEm = DateTimeOffset.UtcNow;
    }

    public void Desativar()
    {
        Ativo = false;
        AtualizadoEm = DateTimeOffset.UtcNow;
    }

    public void Ativar()
    {
        Ativo = true;
        AtualizadoEm = DateTimeOffset.UtcNow;
    }
}
