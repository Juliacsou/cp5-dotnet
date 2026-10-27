using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CP5.Domain.Entities;

[Table("CP5_LIVRO")]
public class LivroEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID")]
    public int Id { get; set; }

    [Required, MaxLength(150)]
    [Column("TITULO")]
    public string Titulo { get; set; } = "";

    [Required, MaxLength(120)]
    [Column("AUTOR")]
    public string Autor { get; set; } = "";

    [Column("ANOPUBLICACAO")]
    public int AnoPublicacao { get; set; }
}
