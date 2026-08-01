using Maresa.Domain.Entities;

namespace Maresa.Application.Interfaces;

public interface IAuditoriaRepository
{
    Task RegistrarAsync(LogAuditoria log, CancellationToken cancellationToken = default);
}
