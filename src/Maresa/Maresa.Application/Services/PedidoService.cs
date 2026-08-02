using Maresa.Application.DTOs;
using Maresa.Application.Interfaces;
using Maresa.Domain.Entities;
using Maresa.Domain.Exceptions;

namespace Maresa.Application.Services;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IAuditoriaRepository _auditoriaRepository;
    private readonly IClienteValidacionService _clienteValidacionService;
    private readonly IUnitOfWork _unitOfWork;

    public PedidoService(
        IPedidoRepository pedidoRepository,
        IAuditoriaRepository auditoriaRepository,
        IClienteValidacionService clienteValidacionService,
        IUnitOfWork unitOfWork)
    {
        _pedidoRepository = pedidoRepository;
        _auditoriaRepository = auditoriaRepository;
        _clienteValidacionService = clienteValidacionService;
        _unitOfWork = unitOfWork;
    }

    public async Task<PedidoResponse> RegistrarPedidoAsync(PedidoRequest request, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            await _auditoriaRepository.RegistrarAsync(new LogAuditoria
            {
                Fecha = DateTime.UtcNow,
                Evento = "PedidoRecibido",
                Descripcion = $"Se recibio una solicitud de pedido para el cliente {request.ClienteId}."
            }, cancellationToken);

            var clienteValido = await _clienteValidacionService.ValidarClienteAsync(request.ClienteId, cancellationToken);

            if (!clienteValido)
            {
                throw new ClienteInvalidoException($"El cliente {request.ClienteId} no es valido.");
            }

            await _auditoriaRepository.RegistrarAsync(new LogAuditoria
            {
                Fecha = DateTime.UtcNow,
                Evento = "ClienteValidado",
                Descripcion = $"El cliente {request.ClienteId} fue validado correctamente."
            }, cancellationToken);

            var pedido = new PedidoCabecera
            {
                ClienteId = request.ClienteId,
                Usuario = request.Usuario,
                Fecha = DateTime.UtcNow,
                Estado = EstadoPedido.Confirmado,
                Total = request.Items.Sum(item => item.Cantidad * item.Precio),
                Detalles = request.Items.Select(item => new PedidoDetalle
                {
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    Precio = item.Precio
                }).ToList()
            };

            await _pedidoRepository.AgregarAsync(pedido, cancellationToken);

            await _auditoriaRepository.RegistrarAsync(new LogAuditoria
            {
                PedidoId = pedido.Id,
                Fecha = DateTime.UtcNow,
                Evento = "PedidoConfirmado",
                Descripcion = $"Pedido {pedido.Id} confirmado para el cliente {pedido.ClienteId}."
            }, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            return new PedidoResponse
            {
                Id = pedido.Id,
                ClienteId = pedido.ClienteId,
                Fecha = pedido.Fecha,
                Total = pedido.Total,
                Usuario = pedido.Usuario,
                Estado = pedido.Estado.ToString(),
                Items = pedido.Detalles.Select(detalle => new PedidoItemResponse
                {
                    ProductoId = detalle.ProductoId,
                    Cantidad = detalle.Cantidad,
                    Precio = detalle.Precio
                }).ToList()
            };
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
