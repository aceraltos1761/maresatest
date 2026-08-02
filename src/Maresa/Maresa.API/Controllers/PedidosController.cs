using Maresa.Application.DTOs;
using Maresa.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Maresa.API.Controllers;

[ApiController]
[Route("api/pedidos")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;

    public PedidosController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    [HttpPost]
    public async Task<ActionResult<PedidoResponse>> Post([FromBody] PedidoRequest request, CancellationToken cancellationToken)
    {
        var response = await _pedidoService.RegistrarPedidoAsync(request, cancellationToken);
        return Created($"/api/pedidos/{response.Id}", response);
    }
}
