using CP5.Domain.Entities;

namespace CP5.Domain.Interfaces;

public interface IUserRepository
{
    // Retorno explícito do Repository: sucesso ou erro (e-mail duplicado).
    Task<OperationResult<UserEntity>> AdicionarAsync(UserEntity entity);
    Task<UserEntity?> AutenticarAsync(string email, string senha);
}
