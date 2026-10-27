using CP5.Application.Dtos;
using CP5.Domain.Entities;
using CP5.Domain.Errors;
using CP5.Domain.Interfaces;
namespace CP5.Application.UseCases;
// Segue a abordagem do professor: UseCase recebe repositório por injeção e devolve OperationResult.
public class CadastroUseCase<T> where T:class
{
 private readonly IRepository<T> _repository;
 public CadastroUseCase(IRepository<T> repository) { _repository=repository; }
 public async Task<OperationResult<PageResponseDto<T>>> ListarAsync(int pagina, int tamanho, string baseUrl)
 {
  if (pagina < 1 || tamanho < 1 || tamanho > 100) return OperationResult<PageResponseDto<T>>.Failure("Informe pagina >= 1 e tamanho de 1 a 100.",400);
  try {
   var (items,total)=await _repository.ListarAsync(pagina,tamanho);
   var p=new PageResponseDto<T> { Items=items,Pagina=pagina,Tamanho=tamanho,TotalRegistros=total,TotalPaginas=(int)Math.Ceiling(total/(double)tamanho) };
   p.Links.Add(new("self",$"{baseUrl}?pagina={pagina}&tamanho={tamanho}","GET"));
   if(pagina>1)p.Links.Add(new("previous",$"{baseUrl}?pagina={pagina-1}&tamanho={tamanho}","GET"));
   if(pagina<p.TotalPaginas)p.Links.Add(new("next",$"{baseUrl}?pagina={pagina+1}&tamanho={tamanho}","GET"));
   return OperationResult<PageResponseDto<T>>.Success(p);
  } catch(Exception) { return OperationResult<PageResponseDto<T>>.Failure("Falha ao consultar os registros.",500); }
 }
 public async Task<OperationResult<RecursoDto<T>>> ObterAsync(int id,string baseUrl)
 {
  if(id < 1)return OperationResult<RecursoDto<T>>.Failure("Informe um ID válido.",400);
  try {
   var value=await _repository.ObterAsync(id);
   if(value==null)return OperationResult<RecursoDto<T>>.Failure("Registro não encontrado.",404);
   return OperationResult<RecursoDto<T>>.Success(new RecursoDto<T>{ Dados=value,Links=new(){new("self",$"{baseUrl}/{id}","GET"),new("update",$"{baseUrl}/{id}","PUT"),new("delete",$"{baseUrl}/{id}","DELETE")} });
  } catch(NoContentException e){return OperationResult<RecursoDto<T>>.Failure(e.Message,404);}catch(Exception){return OperationResult<RecursoDto<T>>.Failure("Erro ao consultar registro.",500);}
 }
 public async Task<OperationResult<T>> CriarAsync(T entity)
 {
  try {return OperationResult<T>.Success(await _repository.AdicionarAsync(entity),201);}
  catch(NoContentException e){return OperationResult<T>.Failure(e.Message,404);}
  catch(Exception){return OperationResult<T>.Failure("Erro ao cadastrar.",500);}
 }
 public async Task<OperationResult<T>> EditarAsync(T entity)
 {
  try {return OperationResult<T>.Success(await _repository.EditarAsync(entity));}
  catch(NoContentException e){return OperationResult<T>.Failure(e.Message,404);}
  catch(Exception){return OperationResult<T>.Failure("Erro ao editar.",500);}
 }
 public async Task<OperationResult<string>> RemoverAsync(int id)
 {
  if(id < 1)return OperationResult<string>.Failure("Informe um ID válido.",400);
  try {await _repository.RemoverAsync(id);return OperationResult<string>.Success("Registro excluído.");}
  catch(NoContentException e){return OperationResult<string>.Failure(e.Message,404);}
  catch(Exception){return OperationResult<string>.Failure("Erro ao excluir.",500);}
 }
}
