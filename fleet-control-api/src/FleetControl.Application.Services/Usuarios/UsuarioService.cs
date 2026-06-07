using FleetControl.Application.Usuarios;
using FleetControl.Domain.Shared;
using FleetControl.Domain.Usuarios;

namespace FleetControl.Application.Services.Usuarios;

public sealed class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;

    public UsuarioService(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<UsuarioResponse>> CriarAsync(CriarUsuarioRequest request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<PerfilUsuario>(request.Perfil, ignoreCase: true, out var perfil))
            return Result<UsuarioResponse>.Failure($"Perfil '{request.Perfil}' inválido. Use: Administrador, Motorista ou Gestor.");

        var existente = await _repository.ObterPorEmailAsync(request.Email, cancellationToken);
        if (existente is not null)
            return Result<UsuarioResponse>.Failure($"Já existe um usuário com o email '{request.Email}'.", 409);

        var usuario = Usuario.Criar(request.Nome, request.Email, perfil, request.Telefone, request.NumeroCnh);
        await _repository.AdicionarAsync(usuario, cancellationToken);

        return Result<UsuarioResponse>.Success(MapToResponse(usuario));
    }

    public async Task<Result<UsuarioResponse>> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var usuario = await _repository.ObterPorIdAsync(id, cancellationToken);
        if (usuario is null)
            return Result<UsuarioResponse>.NotFound($"Usuário '{id}' não encontrado.");

        return Result<UsuarioResponse>.Success(MapToResponse(usuario));
    }

    public async Task<Result<IReadOnlyCollection<UsuarioResponse>>> ListarAsync(CancellationToken cancellationToken)
    {
        var usuarios = await _repository.ListarAsync(cancellationToken);
        var response = usuarios.Select(MapToResponse).ToList().AsReadOnly();
        return Result<IReadOnlyCollection<UsuarioResponse>>.Success(response);
    }

    public async Task<Result> AtualizarAsync(Guid id, AtualizarUsuarioRequest request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<PerfilUsuario>(request.Perfil, ignoreCase: true, out var perfil))
            return Result.Failure($"Perfil '{request.Perfil}' invalido. Use: Administrador, Motorista ou Gestor.");

        var usuario = await _repository.ObterPorIdAsync(id, cancellationToken);
        if (usuario is null)
            return Result.NotFound($"Usuário '{id}' não encontrado.");

        usuario.Atualizar(request.Nome, request.Telefone, request.NumeroCnh);
        usuario.AlterarPerfil(perfil);
        await _repository.AtualizarAsync(usuario, cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DesativarAsync(Guid id, CancellationToken cancellationToken)
    {
        var usuario = await _repository.ObterPorIdAsync(id, cancellationToken);
        if (usuario is null)
            return Result.NotFound($"Usuário '{id}' não encontrado.");

        usuario.Desativar();
        await _repository.AtualizarAsync(usuario, cancellationToken);

        return Result.Success();
    }

    public async Task<Result> AtivarAsync(Guid id, CancellationToken cancellationToken)
    {
        var usuario = await _repository.ObterPorIdAsync(id, cancellationToken);
        if (usuario is null)
            return Result.NotFound($"Usuário '{id}' não encontrado.");

        usuario.Ativar();
        await _repository.AtualizarAsync(usuario, cancellationToken);

        return Result.Success();
    }

    private static UsuarioResponse MapToResponse(Usuario u) =>
        new(u.Id, u.Nome, u.Email, u.Perfil.ToString(), u.Ativo, u.Telefone, u.NumeroCnh, u.CriadoEm);
}
