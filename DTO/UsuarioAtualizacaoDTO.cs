using System.ComponentModel.DataAnnotations;

namespace MusicBank.WebAPI.DTO;

public class UsuarioAtualizacaoDTO
{
    [Required(ErrorMessage = "O nome é obrigatório."), StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório."), EmailAddress, StringLength(50)]
    public string Email { get; set; } = string.Empty;

    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
    public string? Senha { get; set; }
}
