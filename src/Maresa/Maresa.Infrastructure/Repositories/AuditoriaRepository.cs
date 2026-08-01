using Maresa.Application.Interfaces;
using Maresa.Domain.Entities;
using Maresa.Infrastructure.Data;

namespace Maresa.Infrastructure.Repositories;

public class AuditoriaRepository : IAuditoriaRepository
{
    private readonly MaresaDbContext _dbContext;

    public AuditoriaRepository(MaresaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task RegistrarAsync(LogAuditoria log, CancellationToken cancellationToken = default)
    {
        await _dbContext.LogsAuditoria.AddAsync(log, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
