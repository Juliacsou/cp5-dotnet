using CP5.Application.UseCases;
using CP5.Domain.Entities;
using CP5.Domain.Errors;
using CP5.Domain.Interfaces;
using Moq;
using Xunit;

namespace CP5.Tests.Unit;

public class ErrosCustomizadosTests
{
    [Fact]
    public async Task EditarAsync_RepositorioLancaNoContent_Retorna404()
    {
        // Arrange
        var livro = new LivroEntity { Id = 900, Titulo = "Nao existe", Autor = "Teste" };
        var repo = new Mock<IRepository<LivroEntity>>();
        repo.Setup(r => r.EditarAsync(livro))
            .ThrowsAsync(new NoContentException("Registro não encontrado para edição."));
        var sut = new CadastroUseCase<LivroEntity>(repo.Object);

        // Act
        var result = await sut.EditarAsync(livro);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task RemoverAsync_RepositorioLancaNoContent_Retorna404()
    {
        // Arrange
        var repo = new Mock<IRepository<LivroEntity>>();
        repo.Setup(r => r.RemoverAsync(900))
            .ThrowsAsync(new NoContentException("Registro não encontrado para exclusão."));
        var sut = new CadastroUseCase<LivroEntity>(repo.Object);

        // Act
        var result = await sut.RemoverAsync(900);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(404, result.StatusCode);
    }
}
