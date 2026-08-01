namespace Maresa.Application.Interfaces;

public interface IClienteValidacionService
{
    Task<bool> ValidarClienteAsync(int clienteId, CancellationToken cancellationToken = default);
}
