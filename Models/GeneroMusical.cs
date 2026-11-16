using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MusicBank.WebAPI.Models;

public partial class GeneroMusical
{
    public int IdGeneroMusical { get; set; }

    public string GeneroMusical1 { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Musica> IdMusicas { get; set; } = new List<Musica>();
}
