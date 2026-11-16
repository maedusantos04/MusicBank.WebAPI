using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicBank.WebAPI.DTO;
using MusicBank.WebAPI.Interfaces;
using MusicBank.WebAPI.Models;
using MusicBank.WebAPI.Services;

namespace MusicBank.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MusicaController : ControllerBase
{
    private readonly IMusicaRepository _musicaRepository;
    private readonly IGeneroMusicalRepository _generoRepository;
    private readonly IImageService _imageService;

    public MusicaController(IMusicaRepository musicaRepository, IGeneroMusicalRepository generoRepository, IImageService imageService)
    {
        _musicaRepository = musicaRepository;
        _generoRepository = generoRepository;
        _imageService = imageService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 10, [FromQuery] string? busca = null)
    {
        if (pagina < 1 || tamanhoPagina is < 1 or > 100)
            return BadRequest("Página deve ser positiva e o tamanhoPagina deve estar entre 1 e 100.");

        var (items, total) = await _musicaRepository.ListarPaginado(pagina, tamanhoPagina, busca);
        return Ok(new { items, total, pagina, tamanhoPagina });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var musica = await _musicaRepository.BuscarPorId(id);
        return musica is null ? NotFound() : Ok(musica);
    }

    [HttpPost]
    [Authorize]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(11 * 1024 * 1024)]
    public async Task<IActionResult> Cadastrar([FromForm] MusicaDTO dto)
    {
        var generos = await BuscarGeneros(dto.IdGeneros ?? new List<int>());
        if (generos is null) return BadRequest("Um ou mais IDs de gênero musical não existem.");

        if (dto.ArquivoImagem is not null && !_imageService.IsConfigured)
            return Problem("Configure as credenciais do Cloudinary para enviar imagens.", statusCode: 503);

        string? novaUrl = null;
        try
        {
            if (dto.ArquivoImagem is not null)
                novaUrl = await _imageService.UploadImageAsync(dto.ArquivoImagem, ImageFolder.Musicas);
        }
        catch (ArgumentException erro) { return BadRequest(erro.Message); }
        catch (HttpRequestException) { return Problem("Falha ao enviar a capa ao Cloudinary.", statusCode: 502); }

        var imagem = string.IsNullOrWhiteSpace(dto.ImagemCapa) ? null : dto.ImagemCapa;

        var musica = Converter(dto, generos, novaUrl ?? imagem);

        try
        {
            await _musicaRepository.Cadastrar(musica);
        }
        catch
        {
            if (novaUrl is not null) await _imageService.DeleteImageAsync(novaUrl);
            throw;
        }

        return CreatedAtAction(nameof(BuscarPorId), new { id = musica.IdMusica }, musica);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(11 * 1024 * 1024)]
    public async Task<IActionResult> Atualizar(int id, [FromForm] MusicaDTO dto)
    {
        var atual = await _musicaRepository.BuscarPorId(id);
        if (atual is null) return NotFound();

        var idsGeneros = dto.IdGeneros ?? atual.IdGeneroMusicals.Select(g => g.IdGeneroMusical).ToList();
        var generos = await BuscarGeneros(idsGeneros);
        if (generos is null) return BadRequest("Um ou mais IDs de gênero musical não existem.");

        if (dto.ArquivoImagem is not null && !_imageService.IsConfigured)
            return Problem("Configure as credenciais do Cloudinary para enviar imagens.", statusCode: 503);

        string? novaUrl = null;
        try
        {
            if (dto.ArquivoImagem is not null)
                novaUrl = await _imageService.UploadImageAsync(dto.ArquivoImagem, ImageFolder.Musicas);
        }
        catch (ArgumentException erro) { return BadRequest(erro.Message); }
        catch (HttpRequestException) { return Problem("Falha ao enviar a capa ao Cloudinary.", statusCode: 502); }

        var imagem = novaUrl ?? (string.IsNullOrWhiteSpace(dto.ImagemCapa) ? atual.ImagemCapa : dto.ImagemCapa);
        try
        {
            await _musicaRepository.Atualizar(id, Converter(dto, generos, imagem));
        }
        catch
        {
            if (novaUrl is not null) await _imageService.DeleteImageAsync(novaUrl);
            throw;
        }

        if (!string.IsNullOrWhiteSpace(atual.ImagemCapa) && atual.ImagemCapa != imagem)
            await _imageService.DeleteImageAsync(atual.ImagemCapa);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Excluir(int id)
    {
        var musica = await _musicaRepository.BuscarPorId(id);
        if (musica is null) return NotFound();

        await _musicaRepository.Excluir(id);

        if (!string.IsNullOrWhiteSpace(musica.ImagemCapa))
            await _imageService.DeleteImageAsync(musica.ImagemCapa);

        return NoContent();
    }

    private async Task<List<GeneroMusical>?> BuscarGeneros(IReadOnlyCollection<int> ids)
    {
        if (ids.Count == 0) return new List<GeneroMusical>();
        var unicos = ids.Distinct().ToList();
        var generos = await _generoRepository.BuscarPorIds(unicos);
        return generos.Count == unicos.Count ? generos : null;
    }

    private static Musica Converter(MusicaDTO dto, List<GeneroMusical> generos, string? imagemCapa) => new()
    {
        NomeMusica = dto.NomeMusica,
        ClipeMusica = dto.ClipeMusica,
        NomeArtista = dto.NomeArtista,
        DataLancamento = dto.DataLancamento,
        Duracao = dto.Duracao,
        ImagemCapa = imagemCapa,
        IdGeneroMusicals = generos
    };
}
