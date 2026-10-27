using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CP5.Domain.Entities;

[Table("CP5_EMPRESTIMO")]
public class EmprestimoEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID")]
    public int Id { get; set; }

    [Column("LIVROID")]
    public int LivroId { get; set; }

    [Required, MaxLength(120)]
    [Column("LEITOR")]
    public string Leitor { get; set; } = "";

    [Column("DATAEMPRESTIMO")]
    public DateTime DataEmprestimo { get; set; } = DateTime.UtcNow;

    [Column("DATADEVOLUCAO")]
    public DateTime? DataDevolucao { get; set; }

    public LivroEntity? Livro { get; set; }
}
