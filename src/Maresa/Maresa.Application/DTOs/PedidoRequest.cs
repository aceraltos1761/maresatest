using System.ComponentModel.DataAnnotations;

namespace Maresa.Application.DTOs;

public class PedidoRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "El clienteId debe ser un valor positivo.")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "El usuario es obligatorio.")]
    [MaxLength(100)]
    public string Usuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "El pedido debe incluir al menos un item.")]
    [MinLength(1, ErrorMessage = "El pedido debe incluir al menos un item.")]
    public List<PedidoItemRequest> Items { get; set; } = new();
}
