using CP5.Application.UseCases;
using CP5.Domain.Entities;
using CP5.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace CP5.Tests.Unit;

public class UserUseCaseTests
{
    private static UserUseCase CriarUseCase(Mock<IUserRepository> repository)
    {
        return new UserUseCase(repository.Object, new ConfigurationBuilder().Build());
    }

    [Fact]
    public async Task AdicionarAsync_CadastroPublico_AtribuiPerfilLeitor()
    {
        // Arrange
        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.AdicionarAsync(It.IsAny<UserEntity>()))
            .ReturnsAsync((UserEntity u) => OperationResult<UserEntity>.Success(u, 201));
        var sut = CriarUseCase(repository);

        // Act
        var result = await sut.AdicionarAsync("Ana", "ana@teste.com", "Teste123");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("leitor", result.Value!.Role);
    }

    [Fact]
    public async Task AdicionarAdminAsync_CadastroDeAdmin_AtribuiPerfilAdmin()
    {
        // Arrange
        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.AdicionarAsync(It.IsAny<UserEntity>()))
            .ReturnsAsync((UserEntity u) => OperationResult<UserEntity>.Success(u, 201));
        var sut = CriarUseCase(repository);

        // Act
        var result = await sut.AdicionarAdminAsync("Joao", "joao@teste.com", "Joao123!");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("admin", result.Value!.Role);
    }

    [Fact]
    public async Task AdicionarAdminAsync_EmailExistente_DevolveErro409()
    {
        // Arrange: repository devolve erro estruturado, sem exceção.
        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.AdicionarAsync(It.IsAny<UserEntity>()))
            .ReturnsAsync(OperationResult<UserEntity>.Failure("Email ja cadastrado.", 409));
        var sut = CriarUseCase(repository);

        // Act
        var result = await sut.AdicionarAdminAsync("Joao", "joao@teste.com", "Joao123!");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(409, result.StatusCode);
        Assert.Equal("Email ja cadastrado.", result.Error);
    }
}
