using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Pedidos.Application.Pedidos;

namespace Pedidos.Api.Controllers;

[ApiController]
[Route("api/pedidos")]
[Authorize(Roles = "Admin")]
[EnableRateLimiting("api")]
public class PedidosController(PedidoService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<PedidoDto>>> List(CancellationToken ct)
        => Ok(await service.ListAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PedidoDto>> Get(int id, CancellationToken ct)
        => Ok(await service.GetOrThrowAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<PedidoDto>> Create(CrearPedidoRequest request, CancellationToken ct)
    {
        var pedido = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = pedido.Id }, pedido);
    }
    [HttpPatch("{id:int}/estado")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PedidoDto>> CambiarEstado(int id, CambiarEstadoRequest request, CancellationToken ct)
    => Ok(await service.CambiarEstadoAsync(id, request, ct));

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PedidoDto>> Update(int id, ActualizarPedidoRequest request, CancellationToken ct)
        => Ok(await service.UpdateAsync(id, request, ct));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }
}