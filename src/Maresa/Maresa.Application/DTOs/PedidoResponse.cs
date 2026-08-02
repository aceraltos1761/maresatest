namespace Maresa.Application.DTOs;

public class PedidoResponse
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public List<PedidoItemResponse> Items { get; set; } = new();
}
