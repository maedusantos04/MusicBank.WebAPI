using System;
using System.Collections.Generic;

namespace MusicBank.WebAPI.Models;

public partial class Musica
{
    public int IdMusica { get; set; }

    public string NomeMusica { get; set; } = null!;

    public string? ClipeMusica { get; set; }

    public string? NomeArtista { get; set; }

    public DateOnly DataLancamento { get; set; }

    public int Duracao { get; set; }

    public string? ImagemCapa { get; set; }

    public virtual ICollection<GeneroMusical> IdGeneroMusicals { get; set; } = new List<GeneroMusical>();
}
