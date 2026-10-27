using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CP5.Domain.Entities;
using CP5.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CP5.Application.UseCases;

public class UserUseCase
{
    private readonly IUserRepository _repository;
    private readonly IConfiguration _configuration;

    public UserUseCase(IUserRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    // Cadastro público de leitor (simplificação acadêmica aprovada para o CP5).
    public Task<OperationResult<UserEntity>> AdicionarAsync(string nome, string email, string senha)
    {
        return _repository.AdicionarAsync(new UserEntity
        {
            Nome = nome, Email = email, Password = senha, Role = "leitor"
        });
    }

    // Cadastro público de admin: mesmo formato do cadastro de leitor, sem chave.
    public Task<OperationResult<UserEntity>> AdicionarAdminAsync(string nome, string email, string senha)
    {
        return _repository.AdicionarAsync(new UserEntity
        {
            Nome = nome, Email = email, Password = senha, Role = "admin"
        });
    }

    public async Task<string?> AutenticarAsync(string email, string senha)
    {
        var usuario = await _repository.AutenticarAsync(email, senha);
        if (usuario is null) return null;

        var secret = _configuration["Secretkey"]!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Role)
        };
        var token = new JwtSecurityToken(claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
