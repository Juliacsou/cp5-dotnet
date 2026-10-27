using CP5.Domain.Errors;
using CP5.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace CP5.API.Controllers;
[ApiController][Route("api/endereco")][Authorize]
public class EnderecoController:ControllerBase
{
 private readonly IEnderecoService _service;
 public EnderecoController(IEnderecoService service){_service=service;}
 [HttpGet("{cep}")]public async Task<IActionResult> Get(string cep)
 {
  try{return Ok(await _service.ObterPorCepAsync(cep));}
  catch(ArgumentException ex){return BadRequest(ex.Message);}
  catch(NoContentException ex){return NotFound(ex.Message);}
  catch(Exception){return StatusCode(503,"Serviço externo indisponível.");}
 }
}
