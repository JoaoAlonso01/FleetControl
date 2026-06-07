using FleetControl.Application.Usuarios;
using Microsoft.AspNetCore.Mvc;

namespace FleetControl.Api.Controllers;

[ApiController]
[Route("api/v1/usuarios")]
public sealed class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _service;

    public UsuariosController(IUsuarioService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<UsuarioResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        var result = await _service.ListarAsync(cancellationToken);
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.ObterPorIdAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return Problem(result.Error, statusCode: result.StatusCode);

        return Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Criar([FromBody] CriarUsuarioRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.CriarAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return Problem(result.Error, statusCode: result.StatusCode);

        return CreatedAtAction(nameof(ObterPorId), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarUsuarioRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.AtualizarAsync(id, request, cancellationToken);
        if (!result.IsSuccess)
            return Problem(result.Error, statusCode: result.StatusCode);

        return NoContent();
    }

    [HttpPatch("{id:guid}/desativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Desativar(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.DesativarAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return Problem(result.Error, statusCode: result.StatusCode);

        return NoContent();
    }

    [HttpPatch("{id:guid}/ativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Ativar(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.AtivarAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return Problem(result.Error, statusCode: result.StatusCode);

        return NoContent();
    }
}
