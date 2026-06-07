using FleetControl.Application.Veiculos;
using FleetControl.Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FleetControl.Api.Controllers;

[ApiController]
[Route("api/v1/veiculos")]
public sealed class VeiculosController : ControllerBase
{
    private readonly IVeiculoService _service;

    public VeiculosController(IVeiculoService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<VeiculoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        var result = await _service.ListarAsync(cancellationToken);
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VeiculoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.ObterPorIdAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return Problem(result.Error, statusCode: result.StatusCode);

        return Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(VeiculoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Criar([FromBody] CriarVeiculoRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.CriarAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return Problem(result.Error, statusCode: result.StatusCode);

        return CreatedAtAction(nameof(ObterPorId), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarVeiculoRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.AtualizarAsync(id, request, cancellationToken);
        if (!result.IsSuccess)
            return Problem(result.Error, statusCode: result.StatusCode);

        return NoContent();
    }

    [HttpPatch("{id:guid}/quilometragem")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtualizarKilometragem(Guid id, [FromBody] AtualizarKilometragemRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.AtualizarKilometragemAsync(id, request, cancellationToken);
        if (!result.IsSuccess)
            return Problem(result.Error, statusCode: result.StatusCode);

        return NoContent();
    }

    [HttpPatch("{id:guid}/manutencao/iniciar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> EnviarParaManutencao(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.EnviarParaManutencaoAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return Problem(result.Error, statusCode: result.StatusCode);

        return NoContent();
    }

    [HttpPatch("{id:guid}/manutencao/retornar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RetornarDaManutencao(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.RetornarDaManutencaoAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return Problem(result.Error, statusCode: result.StatusCode);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Desativar(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.DesativarAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return Problem(result.Error, statusCode: result.StatusCode);

        return NoContent();
    }
}
