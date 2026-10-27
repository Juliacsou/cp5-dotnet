using CP5.Domain.Entities;
using CP5.Domain.Errors;
using CP5.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace CP5.Infrastructure.Data.Repositories;
public class Repository<T>:IRepository<T> where T:class
{
 protected readonly ApplicationContext Context;
 public Repository(ApplicationContext context){Context=context;}
 public async Task<(List<T> Items,int Total)> ListarAsync(int pagina,int tamanho)
 {
  var query=Context.Set<T>().AsNoTracking();
  return (await query.OrderBy(x=>EF.Property<int>(x,"Id")).Skip((pagina-1)*tamanho).Take(tamanho).ToListAsync(),await query.CountAsync());
 }
 public async Task<T?> ObterAsync(int id)=>await Context.Set<T>().AsNoTracking().FirstOrDefaultAsync(x=>EF.Property<int>(x,"Id")==id);
 public virtual async Task<T> AdicionarAsync(T entity){Context.Set<T>().Add(entity);await Context.SaveChangesAsync();return entity;}
 public async Task<T> EditarAsync(T entity)
 {
  int id=(int)(typeof(T).GetProperty("Id")?.GetValue(entity)??0);
  var antigo=await Context.Set<T>().FindAsync(id);
  if(antigo==null)throw new NoContentException("Registro não encontrado para edição.");
  Context.Entry(antigo).CurrentValues.SetValues(entity);
  await Context.SaveChangesAsync();return antigo;
 }
 public async Task RemoverAsync(int id)
 {
  var value=await Context.Set<T>().FindAsync(id);
  if(value==null)throw new NoContentException("Registro não encontrado para exclusão.");
  Context.Set<T>().Remove(value);await Context.SaveChangesAsync();
 }
}
public class LivroRepository:Repository<LivroEntity>,ILivroRepository {public LivroRepository(ApplicationContext c):base(c){} }
public class EmprestimoRepository:Repository<EmprestimoEntity>,IEmprestimoRepository
{
 public EmprestimoRepository(ApplicationContext c):base(c){}
 public override async Task<EmprestimoEntity> AdicionarAsync(EmprestimoEntity value)
 {
  if(!await Context.Livros.AnyAsync(x=>x.Id==value.LivroId))throw new NoContentException("Livro informado não existe.");
  return await base.AdicionarAsync(value);
 }
}
