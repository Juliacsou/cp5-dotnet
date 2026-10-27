using CP5.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace CP5.Infrastructure.Data;
public class ApplicationContext : DbContext
{
 public ApplicationContext(DbContextOptions<ApplicationContext> options):base(options) {}
 public DbSet<UserEntity> Usuarios => Set<UserEntity>();
 public DbSet<LivroEntity> Livros => Set<LivroEntity>();
 public DbSet<EmprestimoEntity> Emprestimos => Set<EmprestimoEntity>();
 protected override void OnModelCreating(ModelBuilder b)
 {
  b.Entity<LivroEntity>().HasIndex(x=>x.Titulo);
  b.Entity<EmprestimoEntity>().HasIndex(x=>x.LivroId);
  b.Entity<EmprestimoEntity>().HasOne(x=>x.Livro).WithMany().HasForeignKey(x=>x.LivroId).OnDelete(DeleteBehavior.Restrict);
 }
}
