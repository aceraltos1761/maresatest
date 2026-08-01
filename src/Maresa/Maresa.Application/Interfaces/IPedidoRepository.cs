using Maresa.Domain.Entities;

namespace Maresa.Application.Interfaces;

public interface IPedidoRepository
{
    Task AgregarAsync(PedidoCabecera pedido, CancellationToken cancellationToken = default);
}
