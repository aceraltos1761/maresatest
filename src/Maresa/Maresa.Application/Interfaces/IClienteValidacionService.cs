namespace Maresa.Application.Interfaces;

public interface IClienteValidacionService
{
    /// <summary>
    /// Valida al cliente contra el servicio externo. Devuelve su username si es valido,
    /// o null si el cliente no existe.
    /// </summary>
    Task<string?> ValidarClienteAsync(int clienteId, CancellationToken cancellationToken = default);
}
