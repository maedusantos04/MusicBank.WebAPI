using Microsoft.EntityFrameworkCore;
using MusicBank.WebAPI.Context;
using MusicBank.WebAPI.Interfaces;
using MusicBank.WebAPI.Models;

namespace MusicBank.WebAPI.Repositories;

public class MusicaRepository : IMusicaRepository
{
    private readonly MusicBankDbContext _context;

    public MusicaRepository(MusicBankDbContext context)
    {
        _context = context;
    }

    public async Task Atualizar(int id, Musica musica)
    {
        var musicaExistente = await _context.Musicas.Include(m => m.IdGeneroMusicals).FirstOrDefaultAsync(m => m.IdMusica == id);

        if (musicaExistente != null)
        {
            musicaExistente.NomeMusica = musica.NomeMusica;
            musicaExistente.ClipeMusica = musica.ClipeMusica;
            musicaExistente.NomeArtista = musica.NomeArtista;
            musicaExistente.Duracao = musica.Duracao;
            musicaExistente.DataLancamento = musica.DataLancamento;
            musicaExistente.ImagemCapa = musica.ImagemCapa;
            musicaExistente.IdGeneroMusicals = musica.IdGeneroMusicals;

            await _context.SaveChangesAsync();
        }
    }

    public async Task<Musica?> BuscarPorId(int id)
    {
        return await _context.Musicas.AsNoTracking().Include(m => m.IdGeneroMusicals).FirstOrDefaultAsync(m => m.IdMusica == id);
    }

    public async Task Cadastrar(Musica musica)
    {
        await _context.Musicas.AddAsync(musica);
        await _context.SaveChangesAsync();
    }

    public async Task Excluir(int id)
    {
        var musicaBuscada = await _context.Musicas.FirstOrDefaultAsync(m => m.IdMusica == id);
        if (musicaBuscada != null)
        {
            _context.Musicas.Remove(musicaBuscada);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<(List<Musica> musicas, int total)> ListarPaginado(int pagina = 1, int tamanhoPagina = 10, string? busca = null)
    {
        var query = _context.Musicas.AsNoTracking().Include(m => m.IdGeneroMusicals).AsQueryable();

        if (!string.IsNullOrWhiteSpace(busca))
        {
            query = query.Where(m => m.NomeMusica.Contains(busca) 
                                  || (m.NomeArtista != null && m.NomeArtista.Contains(busca)));
        }

        int total = await query.CountAsync();

        var items = await query
            .OrderBy(m => m.NomeMusica).ThenBy(m => m.IdMusica)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return (items, total);
    }
}
