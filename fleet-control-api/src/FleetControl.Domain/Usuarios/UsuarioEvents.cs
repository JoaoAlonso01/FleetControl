using FleetControl.Domain.Shared;

namespace FleetControl.Domain.Usuarios;

public sealed class UsuarioDomainException : DomainException
{
    public UsuarioDomainException(string message) : base(message) { }
}

public sealed record UsuarioCriadoEvent(Guid UsuarioId, string Email) : IDomainEvent;
