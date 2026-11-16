using Microsoft.EntityFrameworkCore;
using MusicBank.WebAPI.Context;
using MusicBank.WebAPI.Interfaces;
using MusicBank.WebAPI.Models;

namespace MusicBank.WebAPI.Repositories;

public class GeneroMusicalRepository : IGeneroMusicalRepository
{
    private readonly MusicBankDbContext _context;

    public GeneroMusicalRepository(MusicBankDbContext context)
    {
        _context = context;
    }

    public async Task Atualizar(int id, GeneroMusical genero)
    {
        var generoExistente = await _context.GeneroMusicals.FindAsync(id);
        if (generoExistente != null)
        {
            generoExistente.GeneroMusical1 = genero.GeneroMusical1;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<GeneroMusical?> BuscarPorId(int id)
    {
        return await _context.GeneroMusicals.AsNoTracking().FirstOrDefaultAsync(g => g.IdGeneroMusical == id);
    }

    public Task<List<GeneroMusical>> BuscarPorIds(IReadOnlyCollection<int> ids)
    {
        var idsArray = ids.ToArray();
        return _context.GeneroMusicals.Where(g => idsArray.Contains(g.IdGeneroMusical)).ToListAsync();
    }

    public async Task Cadastrar(GeneroMusical genero)
    {
        await _context.GeneroMusicals.AddAsync(genero);
        await _context.SaveChangesAsync();
    }

    public async Task Excluir(int id)
    {
        var generoBuscado = await _context.GeneroMusicals.FindAsync(id);
        if (generoBuscado != null)
        {
            _context.GeneroMusicals.Remove(generoBuscado);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<GeneroMusical>> Listar()
    {
        return await _context.GeneroMusicals.AsNoTracking().OrderBy(g => g.GeneroMusical1).ToListAsync();
    }
}
