using CP5.Application.UseCases;
using CP5.Domain.Entities;
using CP5.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace CP5.Tests;

public class UserUseCaseTests
{
    private static UserUseCase CriarUseCase(Mock<IUserRepository> repository)
    {
        var config = new ConfigurationBuilder().Build();
        return new UserUseCase(repository.Object, config);
    }

    [Fact]
    public async Task AdicionarAsync_CadastroPublico_AtribuiPerfilLeitor()
    {
        // Arrange
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.AdicionarAsync(It.IsAny<UserEntity>()))
            .ReturnsAsync((UserEntity usuario) => usuario);
        var useCase = CriarUseCase(repository);

        // Act
        var usuario = await useCase.AdicionarAsync("Julia", "julia@teste.com", "Teste123");

        // Assert
        Assert.NotNull(usuario);
        Assert.Equal("leitor", usuario.Role);
    }

    [Fact]
    public async Task AdicionarAdminAsync_CadastroDeAdmin_AtribuiPerfilAdmin()
    {
        // Arrange
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.AdicionarAsync(It.IsAny<UserEntity>()))
            .ReturnsAsync((UserEntity usuario) => usuario);
        var useCase = CriarUseCase(repository);

        // Act
        var usuario = await useCase.AdicionarAdminAsync("Professor", "professor@teste.com", "Teste12345");

        // Assert
        Assert.NotNull(usuario);
        Assert.Equal("admin", usuario.Role);
    }

    [Fact]
    public async Task AdicionarAdminAsync_EmailExistente_DevolveNulo()
    {
        // Arrange
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.AdicionarAsync(It.IsAny<UserEntity>()))
            .ReturnsAsync((UserEntity?)null);
        var useCase = CriarUseCase(repository);

        // Act
        var usuario = await useCase.AdicionarAdminAsync("Joao", "joao@teste.com", "Joao123!");

        // Assert
        Assert.Null(usuario);
    }
}
