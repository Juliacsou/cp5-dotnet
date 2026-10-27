using CP5.API.Controllers;
using CP5.Domain.Errors;
using CP5.Domain.Interfaces;
using CP5.Infrastructure.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CP5.Tests.Unit;

public class EnderecoControllerTests
{
    [Fact]
    public async Task Get_CepInexistente_Retorna404()
    {
        // Arrange
        var service = new Mock<IEnderecoService>();
        service.Setup(s => s.ObterPorCepAsync("00000000"))
            .ThrowsAsync(new NoContentException("CEP não encontrado."));
        var controller = new EnderecoController(service.Object);

        // Act
        var response = await controller.Get("00000000");

        // Assert
        Assert.IsType<NotFoundObjectResult>(response);
    }

    [Fact]
    public async Task Get_ServicoIndisponivel_Retorna503()
    {
        // Arrange
        var service = new Mock<IEnderecoService>();
        service.Setup(s => s.ObterPorCepAsync("01001000"))
            .ThrowsAsync(new ServicoExternoIndisponivelException("O serviço ViaCEP está indisponível."));
        var controller = new EnderecoController(service.Object);

        // Act
        var response = await controller.Get("01001000");

        // Assert
        var error = Assert.IsType<ObjectResult>(response);
        Assert.Equal(503, error.StatusCode);
    }

    [Fact]
    public async Task ObterPorCepAsync_CepComTresDigitos_LancaArgumentException()
    {
        // Arrange
        var service = new EnderecoService();

        // Act
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.ObterPorCepAsync("123"));

        // Assert
        Assert.Contains("oito", ex.Message);
    }
}
