using System.ComponentModel.DataAnnotations;
using CP5.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CP5.API.Controllers;

public record PostUserDto([Required] string Nome, [Required, EmailAddress] string Email, [Required] string Password);
public record PostAuthDto([Required, EmailAddress] string Email, [Required] string Senha);

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserUseCase _useCase;

    public AuthController(UserUseCase useCase)
    {
        _useCase = useCase;
    }

    // Cadastro publico de leitor, como ensinado na aula.
    [HttpPost("registrar")]
    [AllowAnonymous]
    public async Task<IActionResult> Registrar(PostUserDto dto)
    {
        if (dto.Password.Length < 6) return BadRequest("A senha precisa ter pelo menos 6 caracteres.");
        var usuario = await _useCase.AdicionarAsync(dto.Nome, dto.Email, dto.Password);
        if (usuario is null) return Conflict("Email ja cadastrado.");
        return Ok(new { usuario.Id, usuario.Nome, usuario.Email, usuario.Role });
    }

    // Cadastro publico de administrador para demonstracao academica do CP5.
    // Nao exige JWT ou chave de instalacao.
    [HttpPost("registrar-admin")]
    [AllowAnonymous]
    public async Task<IActionResult> RegistrarAdmin(PostUserDto dto)
    {
        if (dto.Password.Length < 6) return BadRequest("A senha precisa ter pelo menos 6 caracteres.");
        var usuario = await _useCase.AdicionarAdminAsync(dto.Nome, dto.Email, dto.Password);
        if (usuario is null) return Conflict("Email ja cadastrado.");
        return Ok(new { usuario.Id, usuario.Nome, usuario.Email, usuario.Role });
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(PostAuthDto dto)
    {
        var token = await _useCase.AutenticarAsync(dto.Email, dto.Senha);
        if (token is null) return Unauthorized("Credenciais invalidas.");
        return Ok(new { Jwt = token });
    }
}
