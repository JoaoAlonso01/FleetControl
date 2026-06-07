using FleetControl.Application.Manutencoes;
using Microsoft.AspNetCore.Mvc;

namespace FleetControl.Api.Controllers;

[ApiController]
[Route("api/v1/manutencoes")]
public sealed class ManutencoesController : ControllerBase
{
    private readonly IManutencaoService _service;

    public ManutencoesController(IManutencaoService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ManutencaoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        var result = await _service.ListarAsync(cancellationToken);
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ManutencaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.ObterPorIdAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return Problem(result.Error, statusCode: result.StatusCode);

        return Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ManutencaoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Agendar([FromBody] AgendarManutencaoRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.AgendarAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return Problem(result.Error, statusCode: result.StatusCode);

        return CreatedAtAction(nameof(ObterPorId), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPatch("{id:guid}/iniciar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Iniciar(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.IniciarAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return Problem(result.Error, statusCode: result.StatusCode);

        return NoContent();
    }

    [HttpPatch("{id:guid}/concluir")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Concluir(Guid id, [FromBody] ConcluirManutencaoRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.ConcluirAsync(id, request, cancellationToken);
        if (!result.IsSuccess)
            return Problem(result.Error, statusCode: result.StatusCode);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Cancelar(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.CancelarAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return Problem(result.Error, statusCode: result.StatusCode);

        return NoContent();
    }
}
