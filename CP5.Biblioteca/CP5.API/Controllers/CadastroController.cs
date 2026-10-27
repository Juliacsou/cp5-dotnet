using CP5.Application.UseCases;
using CP5.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace CP5.API.Controllers;
[ApiController]
[Route("api/livros")]
[Authorize]
public class LivrosController:ControllerBase
{
 private readonly CadastroUseCase<LivroEntity> _useCase;
 public LivrosController(CadastroUseCase<LivroEntity> useCase){_useCase=useCase;}
 [HttpGet] public async Task<IActionResult> Get(int pagina=1,int tamanho=10){var r=await _useCase.ListarAsync(pagina,tamanho,$"{Request.Scheme}://{Request.Host}{Request.Path}");return StatusCode(r.StatusCode,r.IsSuccess?(object?)r.Value:r.Error);}
 [HttpGet("{id}")]public async Task<IActionResult> Get(int id){var r=await _useCase.ObterAsync(id,$"{Request.Scheme}://{Request.Host}{Request.PathBase}/api/livros");return StatusCode(r.StatusCode,r.IsSuccess?(object?)r.Value:r.Error);}
 [HttpPost][Authorize(Roles="admin")] public async Task<IActionResult> Post(LivroEntity livro){livro.Id=0;var r=await _useCase.CriarAsync(livro);return StatusCode(r.StatusCode,r.IsSuccess?(object?)r.Value:r.Error);}
 [HttpPut("{id}")][Authorize(Roles="admin")]public async Task<IActionResult> Put(int id,LivroEntity livro){livro.Id=id;var r=await _useCase.EditarAsync(livro);return StatusCode(r.StatusCode,r.IsSuccess?(object?)r.Value:r.Error);}
 [HttpDelete("{id}")][Authorize(Roles="admin")]public async Task<IActionResult> Delete(int id){var r=await _useCase.RemoverAsync(id);return StatusCode(r.StatusCode,r.IsSuccess?(object?)r.Value:r.Error);}
}
[ApiController]
[Route("api/emprestimos")]
[Authorize]
public class EmprestimosController:ControllerBase
{
 private readonly CadastroUseCase<EmprestimoEntity> _useCase;
 public EmprestimosController(CadastroUseCase<EmprestimoEntity> useCase){_useCase=useCase;}
 [HttpGet] public async Task<IActionResult> Get(int pagina=1,int tamanho=10){var r=await _useCase.ListarAsync(pagina,tamanho,$"{Request.Scheme}://{Request.Host}{Request.Path}");return StatusCode(r.StatusCode,r.IsSuccess?(object?)r.Value:r.Error);}
 [HttpGet("{id}")]public async Task<IActionResult> Get(int id){var r=await _useCase.ObterAsync(id,$"{Request.Scheme}://{Request.Host}{Request.PathBase}/api/emprestimos");return StatusCode(r.StatusCode,r.IsSuccess?(object?)r.Value:r.Error);}
 [HttpPost][Authorize(Roles="admin")] public async Task<IActionResult> Post(EmprestimoEntity model){model.Id=0;model.Livro=null;var r=await _useCase.CriarAsync(model);return StatusCode(r.StatusCode,r.IsSuccess?(object?)r.Value:r.Error);}
 [HttpPut("{id}")][Authorize(Roles="admin")]public async Task<IActionResult> Put(int id,EmprestimoEntity model){model.Id=id;model.Livro=null;var r=await _useCase.EditarAsync(model);return StatusCode(r.StatusCode,r.IsSuccess?(object?)r.Value:r.Error);}
 [HttpDelete("{id}")][Authorize(Roles="admin")]public async Task<IActionResult> Delete(int id){var r=await _useCase.RemoverAsync(id);return StatusCode(r.StatusCode,r.IsSuccess?(object?)r.Value:r.Error);}
}
