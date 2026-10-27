using CP5.Domain.Entities;
namespace CP5.Domain.Interfaces;
public interface IRepository<T> where T:class
{
 Task<(List<T> Items, int Total)> ListarAsync(int pagina, int tamanho);
 Task<T?> ObterAsync(int id);
 Task<T> AdicionarAsync(T entity);
 Task<T> EditarAsync(T entity);
 Task RemoverAsync(int id);
}
public interface ILivroRepository : IRepository<LivroEntity> {}
public interface IEmprestimoRepository : IRepository<EmprestimoEntity> {}
public interface IEnderecoService { Task<EnderecoModel> ObterPorCepAsync(string cep); }
public class EnderecoModel { public string Cep { get; set; } = ""; public string Logradouro { get; set; } = ""; public string Localidade { get; set; } = ""; public bool Erro { get; set; } }
