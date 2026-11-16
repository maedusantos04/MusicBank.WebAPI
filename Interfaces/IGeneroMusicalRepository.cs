using MusicBank.WebAPI.Models;

namespace MusicBank.WebAPI.Interfaces;

public interface IGeneroMusicalRepository
{
    Task Cadastrar(GeneroMusical genero);
    Task Atualizar(int id, GeneroMusical genero);
    Task Excluir(int id);
    Task<GeneroMusical?> BuscarPorId(int id);
    Task<List<GeneroMusical>> Listar();
    Task<List<GeneroMusical>> BuscarPorIds(IReadOnlyCollection<int> ids);
}
