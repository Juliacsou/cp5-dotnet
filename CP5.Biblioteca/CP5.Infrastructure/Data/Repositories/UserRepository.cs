using CP5.Domain.Entities;
using CP5.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CP5.Infrastructure.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationContext _context;
    public UserRepository(ApplicationContext context) => _context = context;

    public async Task<OperationResult<UserEntity>> AdicionarAsync(UserEntity entity)
    {
        // Exemplo de erro retornado pelo Repository, sem lançar exceção.
        if (await _context.Usuarios.AsNoTracking().AnyAsync(u => u.Email == entity.Email))
            return OperationResult<UserEntity>.Failure("Email ja cadastrado.", 409);

        _context.Usuarios.Add(entity);
        await _context.SaveChangesAsync();
        return OperationResult<UserEntity>.Success(entity, 201);
    }

    public async Task<UserEntity?> AutenticarAsync(string email, string senha) =>
        await _context.Usuarios.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email && u.Password == senha);
}
