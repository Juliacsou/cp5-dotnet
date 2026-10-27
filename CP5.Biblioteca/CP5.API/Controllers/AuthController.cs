using System.ComponentModel.DataAnnotations;
using CP5.Application.UseCases;
using CP5.Domain.Entities;
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

    public AuthController(UserUseCase useCase) => _useCase = useCase;

    [HttpPost("registrar")]
    [AllowAnonymous]
    public async Task<IActionResult> Registrar(PostUserDto dto)
    {
        if (dto.Password.Length < 6) return BadRequest("A senha precisa ter pelo menos 6 caracteres.");
        return RespostaCadastro(await _useCase.AdicionarAsync(dto.Nome, dto.Email, dto.Password));
    }

    // Endpoint público de administrador solicitado para o exercício acadêmico.
    [HttpPost("registrar-admin")]
    [AllowAnonymous]
    public async Task<IActionResult> RegistrarAdmin(PostUserDto dto)
    {
        if (dto.Password.Length < 6) return BadRequest("A senha precisa ter pelo menos 6 caracteres.");
        return RespostaCadastro(await _useCase.AdicionarAdminAsync(dto.Nome, dto.Email, dto.Password));
    }

    private IActionResult RespostaCadastro(OperationResult<UserEntity> resultado)
    {
        if (!resultado.IsSuccess) return StatusCode(resultado.StatusCode, resultado.Error);
        var usuario = resultado.Value!;
        // Mantém HTTP 200 para não alterar o fluxo já demonstrado no Swagger.
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
