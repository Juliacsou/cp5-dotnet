using CP5.Application.UseCases;
using CP5.Domain.Entities;
using CP5.Domain.Interfaces;
using Moq;
using Xunit;
namespace CP5.Tests.Unit;
public class CadastroUseCaseTests
{
 [Fact] public async Task ObterAsync_RegistroInexistente_Retorna404()
 {
  // Arrange
  var repo=new Mock<IRepository<LivroEntity>>();
  repo.Setup(x=>x.ObterAsync(99)).ReturnsAsync((LivroEntity?)null);
  var sut=new CadastroUseCase<LivroEntity>(repo.Object);
  // Act
  var result=await sut.ObterAsync(99,"https://localhost/api/livros");
  // Assert
  Assert.False(result.IsSuccess);Assert.Equal(404,result.StatusCode);
 }
 [Fact] public async Task ListarAsync_PaginaInvalida_Retorna400()
 {
  var sut=new CadastroUseCase<LivroEntity>(new Mock<IRepository<LivroEntity>>().Object);
  var result=await sut.ListarAsync(0,10,"https://localhost/api/livros");
  Assert.Equal(400,result.StatusCode);
 }
 [Fact] public async Task ListarAsync_HaProximaPagina_RetornaLinkNext()
 {
  var repo=new Mock<IRepository<LivroEntity>>();
  repo.Setup(x=>x.ListarAsync(1,1)).ReturnsAsync((new List<LivroEntity>{new(){Id=1,Titulo="Livro",Autor="Autor"}},2));
  var sut=new CadastroUseCase<LivroEntity>(repo.Object);
  var result=await sut.ListarAsync(1,1,"https://localhost/api/livros");
  Assert.True(result.IsSuccess);Assert.Contains(result.Value!.Links,x=>x.Rel=="next");
 }
 [Fact] public async Task CriarAsync_RegistroValido_Retorna201()
 {
  var livro=new LivroEntity{Titulo="Teste",Autor="Pessoa",AnoPublicacao=2024};
  var repo=new Mock<IRepository<LivroEntity>>();repo.Setup(x=>x.AdicionarAsync(livro)).ReturnsAsync(livro);
  var result=await new CadastroUseCase<LivroEntity>(repo.Object).CriarAsync(livro);
  Assert.Equal(201,result.StatusCode);
 }
}
