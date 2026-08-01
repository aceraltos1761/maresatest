using System.Net;
using System.Text.Json;
using Maresa.Application.Interfaces;
using Maresa.Domain.Exceptions;

namespace Maresa.Infrastructure.ExternalServices;

public class ClienteValidacionService : IClienteValidacionService
{
    private readonly HttpClient _httpClient;

    public ClienteValidacionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> ValidarClienteAsync(int clienteId, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response;

        try
        {
            response = await _httpClient.GetAsync($"users/{clienteId}", cancellationToken);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new ClienteValidacionException("Tiempo de espera agotado al validar el cliente.", ex);
        }
        catch (HttpRequestException ex)
        {
            throw new ClienteValidacionException("No se pudo conectar con el servicio de validacion de clientes.", ex);
        }

        using (response)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ClienteValidacionException(
                    $"El servicio de validacion de clientes respondio con el codigo {(int)response.StatusCode}.");
            }

            var contenido = await response.Content.ReadAsStringAsync(cancellationToken);

            try
            {
                using var documento = JsonDocument.Parse(contenido);
                return documento.RootElement.TryGetProperty("id", out _);
            }
            catch (JsonException ex)
            {
                throw new ClienteValidacionException("El servicio de validacion de clientes devolvio una respuesta invalida.", ex);
            }
        }
    }
}
