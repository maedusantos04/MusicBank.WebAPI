using System.ComponentModel.DataAnnotations;

namespace MusicBank.WebAPI.DTO;

public class MusicaDTO : IValidatableObject
{
    [Required(ErrorMessage = "O nome da música é obrigatório.")]
    [StringLength(50, ErrorMessage = "O nome da música não pode ter mais de 50 caracteres.")]
    public string NomeMusica { get; set; } = string.Empty;

    public string? ClipeMusica { get; set; }

    [StringLength(100, ErrorMessage = "O nome do artista não pode ter mais de 100 caracteres.")]
    public string? NomeArtista { get; set; }

    public DateOnly DataLancamento { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "A duração deve ser positiva (em segundos).")]
    public int Duracao { get; set; }

    public string? ImagemCapa { get; set; }

    public IFormFile? ArquivoImagem { get; set; }

    public List<int>? IdGeneros { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ArquivoImagem is not null && !string.IsNullOrWhiteSpace(ImagemCapa))
        {
            yield return new ValidationResult(
                "Envie ArquivoImagem ou ImagemCapa, não os dois.",
                new[] { nameof(ArquivoImagem), nameof(ImagemCapa) });
        }

        if (DataLancamento.Year > DateTime.UtcNow.Year)
        {
            yield return new ValidationResult(
                "O ano de lançamento não pode ser posterior ao ano atual.",
                new[] { nameof(DataLancamento) });
        }
    }
}
