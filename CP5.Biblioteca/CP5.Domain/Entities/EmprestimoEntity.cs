using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CP5.Domain.Entities;
[Table("CP5_EMPRESTIMO")]
public class EmprestimoEntity
{
 [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int Id { get; set; }
 public int LivroId { get; set; }
 [Required, MaxLength(120)] public string Leitor { get; set; } = "";
 public DateTime DataEmprestimo { get; set; } = DateTime.UtcNow;
 public DateTime? DataDevolucao { get; set; }
 public LivroEntity? Livro { get; set; }
}
