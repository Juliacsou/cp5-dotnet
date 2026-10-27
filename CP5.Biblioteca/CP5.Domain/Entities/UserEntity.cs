using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CP5.Domain.Entities;

[Table("CP5_USUARIO")]
public class UserEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID")]
    public int Id { get; set; }

    [Required, MaxLength(120)]
    [Column("NOME")]
    public string Nome { get; set; } = "";

    [Required, MaxLength(160)]
    [Column("EMAIL")]
    public string Email { get; set; } = "";

    [Required, MaxLength(120)]
    [Column("PASSWORD")]
    public string Password { get; set; } = "";

    [Required, MaxLength(20)]
    [Column("ROLE")]
    public string Role { get; set; } = "leitor";
}
