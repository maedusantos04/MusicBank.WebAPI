using System.ComponentModel.DataAnnotations;

namespace MusicBank.WebAPI.DTO;

public class GeneroMusicalDTO
{
    [Required(ErrorMessage = "O nome do gênero musical é obrigatório.")]
    [StringLength(50, ErrorMessage = "O gênero musical não pode exceder 50 caracteres.")]
    public string GeneroMusical { get; set; } = string.Empty;
}
