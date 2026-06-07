using FleetControl.Domain.Shared;
using FleetControl.Domain.Usuarios;

namespace FleetControl.Application.Usuarios;

public sealed record CriarUsuarioRequest(string Nome, string Email, string Perfil, string? Telefone, string? NumeroCnh);
public sealed record AtualizarUsuarioRequest(string Nome, string Perfil, string? Telefone, string? NumeroCnh);

public sealed record UsuarioResponse(
    Guid Id,
    string Nome,
    string Email,
    string Perfil,
    bool Ativo,
    string? Telefone,
    string? NumeroCnh,
    DateTimeOffset CriadoEm
);

public interface IUsuarioService
{
    Task<Result<UsuarioResponse>> CriarAsync(CriarUsuarioRequest request, CancellationToken cancellationToken);
    Task<Result<UsuarioResponse>> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<IReadOnlyCollection<UsuarioResponse>>> ListarAsync(CancellationToken cancellationToken);
    Task<Result> AtualizarAsync(Guid id, AtualizarUsuarioRequest request, CancellationToken cancellationToken);
    Task<Result> DesativarAsync(Guid id, CancellationToken cancellationToken);
    Task<Result> AtivarAsync(Guid id, CancellationToken cancellationToken);
}
