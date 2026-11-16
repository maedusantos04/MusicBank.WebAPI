using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MusicBank.WebAPI.DTO;
using MusicBank.WebAPI.Interfaces;
using MusicBank.WebAPI.Models;

namespace MusicBank.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GeneroMusicalController : ControllerBase
{
    private readonly IGeneroMusicalRepository _generoRepository;

    public GeneroMusicalController(IGeneroMusicalRepository generoRepository)
    {
        _generoRepository = generoRepository;
    }

    /// <summary>
    /// Lista todos os gêneros musicais em ordem alfabética
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _generoRepository.Listar());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var genero = await _generoRepository.BuscarPorId(id);
        return genero is null ? NotFound() : Ok(genero);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Cadastrar([FromBody] GeneroMusicalDTO dto)
    {
        var genero = new GeneroMusical { GeneroMusical1 = dto.GeneroMusical };

        try
        {
            await _generoRepository.Cadastrar(genero);
        }
        catch (DbUpdateException erro)
            when (erro.GetBaseException() is SqlException { Number: 2601 or 2627 })
        {
            return Conflict("Já existe um gênero com esse nome");
        }

        return CreatedAtAction(nameof(BuscarPorId), new { id = genero.IdGeneroMusical }, genero);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Atualizar(int id, [FromBody] GeneroMusicalDTO dto)
    {
        if (await _generoRepository.BuscarPorId(id) is null) return NotFound();

        try
        {
            await _generoRepository.Atualizar(id, new GeneroMusical { GeneroMusical1 = dto.GeneroMusical });
        }
        catch (DbUpdateException erro)
            when (erro.GetBaseException() is SqlException { Number: 2601 or 2627 })
        {
            return Conflict("Já existe um gênero com esse nome");
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Excluir(int id)
    {
        if (await _generoRepository.BuscarPorId(id) is null) return NotFound();
        await _generoRepository.Excluir(id);
        return NoContent();
    }
}
