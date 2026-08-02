using System.ComponentModel.DataAnnotations;

namespace Maresa.Application.DTOs;

public class PedidoItemRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "El productoId debe ser un valor positivo.")]
    public int ProductoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
    public int Cantidad { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a cero.")]
    public decimal Precio { get; set; }
}
