using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CP5.Domain.Entities;
[Table("CP5_USUARIO")]
public class UserEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int Id { get; set; }
    [Required, MaxLength(120)] public string Nome { get; set; } = "";
    [Required, MaxLength(160)] public string Email { get; set; } = "";
    [Required, MaxLength(120)] public string Password { get; set; } = "";
    [Required, MaxLength(20)] public string Role { get; set; } = "leitor";
}
