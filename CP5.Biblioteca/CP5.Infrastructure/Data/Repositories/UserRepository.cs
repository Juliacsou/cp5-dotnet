using CP5.Domain.Entities;
using CP5.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace CP5.Infrastructure.Data.Repositories;
public class UserRepository : IUserRepository
{
    private readonly ApplicationContext _context;
    public UserRepository(ApplicationContext context) => _context = context;
    public async Task<UserEntity?> AdicionarAsync(UserEntity entity)
    {
        if (await _context.Set<UserEntity>().AnyAsync(u => u.Email == entity.Email)) return null;
        _context.Set<UserEntity>().Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    public async Task<UserEntity?> AutenticarAsync(string email, string senha) =>
        await _context.Set<UserEntity>().AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email && u.Password == senha);
}
