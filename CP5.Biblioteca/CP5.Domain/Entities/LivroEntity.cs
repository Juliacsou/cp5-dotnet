using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CP5.Domain.Entities;
[Table("CP5_LIVRO")]

public class LivroEntity
{
 [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int Id { get; set; }
 [Required, MaxLength(150)] public string Titulo { get; set; } = "";
 [Required, MaxLength(120)] public string Autor { get; set; } = "";
 public int AnoPublicacao { get; set; }
}
