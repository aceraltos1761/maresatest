using Maresa.Application.DTOs;

namespace Maresa.Application.Interfaces;

public interface IPedidoService
{
    Task<PedidoResponse> RegistrarPedidoAsync(PedidoRequest request, CancellationToken cancellationToken = default);
}
