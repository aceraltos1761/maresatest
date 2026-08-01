using Maresa.Application.Interfaces;
using Maresa.Domain.Entities;
using Maresa.Infrastructure.Data;

namespace Maresa.Infrastructure.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly MaresaDbContext _dbContext;

    public PedidoRepository(MaresaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AgregarAsync(PedidoCabecera pedido, CancellationToken cancellationToken = default)
    {
        await _dbContext.Pedidos.AddAsync(pedido, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
