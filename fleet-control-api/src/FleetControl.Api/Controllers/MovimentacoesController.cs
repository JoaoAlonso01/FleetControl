using FleetControl.Application.Movimentacoes;
using Microsoft.AspNetCore.Mvc;

namespace FleetControl.Api.Controllers;

[ApiController]
[Route("api/v1/movimentacoes")]
public sealed class MovimentacoesController : ControllerBase
{
    private readonly IMovimentacaoService _service;

    public MovimentacoesController(IMovimentacaoService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<MovimentacaoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        var result = await _service.ListarAsync(cancellationToken);
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MovimentacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.ObterPorIdAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return Problem(result.Error, statusCode: result.StatusCode);

        return Ok(result.Value);
    }

    [HttpPost("checkout")]
    [ProducesResponseType(typeof(MovimentacaoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckOut([FromBody] RegistrarCheckOutRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.RegistrarCheckOutAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return Problem(result.Error, statusCode: result.StatusCode);

        return CreatedAtAction(nameof(ObterPorId), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPost("checkin")]
    [ProducesResponseType(typeof(MovimentacaoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckIn([FromBody] RegistrarCheckInRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.RegistrarCheckInAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return Problem(result.Error, statusCode: result.StatusCode);

        return CreatedAtAction(nameof(ObterPorId), new { id = result.Value!.Id }, result.Value);
    }
}
