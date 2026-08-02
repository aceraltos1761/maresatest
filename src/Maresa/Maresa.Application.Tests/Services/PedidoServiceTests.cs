using Maresa.Application.DTOs;
using Maresa.Application.Interfaces;
using Maresa.Application.Services;
using Maresa.Domain.Entities;
using Maresa.Domain.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Maresa.Application.Tests.Services;

public class PedidoServiceTests
{
    private readonly Mock<IPedidoRepository> _pedidoRepositoryMock = new();
    private readonly Mock<IAuditoriaRepository> _auditoriaRepositoryMock = new();
    private readonly Mock<IClienteValidacionService> _clienteValidacionServiceMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly PedidoService _sut;

    public PedidoServiceTests()
    {
        _sut = new PedidoService(
            _pedidoRepositoryMock.Object,
            _auditoriaRepositoryMock.Object,
            _clienteValidacionServiceMock.Object,
            _unitOfWorkMock.Object,
            NullLogger<PedidoService>.Instance);
    }

    private static PedidoRequest CrearRequestValido() => new()
    {
        ClienteId = 1,
        Usuario = "usuario.prueba",
        Items = new List<PedidoItemRequest>
        {
            new() { ProductoId = 1, Cantidad = 2, Precio = 10m },
            new() { ProductoId = 2, Cantidad = 1, Precio = 20m }
        }
    };

    [Fact]
    public async Task RegistrarPedidoAsync_CuandoClienteEsValido_GuardaPedidoYConfirmaTransaccion()
    {
        var request = CrearRequestValido();

        _clienteValidacionServiceMock
            .Setup(s => s.ValidarClienteAsync(request.ClienteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _pedidoRepositoryMock
            .Setup(r => r.AgregarAsync(It.IsAny<PedidoCabecera>(), It.IsAny<CancellationToken>()))
            .Callback<PedidoCabecera, CancellationToken>((pedido, _) => pedido.Id = 42)
            .Returns(Task.CompletedTask);

        var response = await _sut.RegistrarPedidoAsync(request, CancellationToken.None);

        Assert.Equal(42, response.Id);
        Assert.Equal(40m, response.Total);
        Assert.Equal("Confirmado", response.Estado);
        Assert.Equal(2, response.Items.Count);

        _pedidoRepositoryMock.Verify(r => r.AgregarAsync(It.IsAny<PedidoCabecera>(), It.IsAny<CancellationToken>()), Times.Once);
        _auditoriaRepositoryMock.Verify(r => r.RegistrarAsync(It.IsAny<LogAuditoria>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarPedidoAsync_CuandoServicioDeValidacionFalla_HaceRollbackYPropagaExcepcion()
    {
        var request = CrearRequestValido();

        _clienteValidacionServiceMock
            .Setup(s => s.ValidarClienteAsync(request.ClienteId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ClienteValidacionException("Tiempo de espera agotado al validar el cliente."));

        await Assert.ThrowsAsync<ClienteValidacionException>(() => _sut.RegistrarPedidoAsync(request, CancellationToken.None));

        _pedidoRepositoryMock.Verify(r => r.AgregarAsync(It.IsAny<PedidoCabecera>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarPedidoAsync_CuandoClienteEsInvalido_HaceRollbackYLanzaExcepcion()
    {
        var request = CrearRequestValido();

        _clienteValidacionServiceMock
            .Setup(s => s.ValidarClienteAsync(request.ClienteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<ClienteInvalidoException>(() => _sut.RegistrarPedidoAsync(request, CancellationToken.None));

        _pedidoRepositoryMock.Verify(r => r.AgregarAsync(It.IsAny<PedidoCabecera>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
