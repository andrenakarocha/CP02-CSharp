using BancoDigital.API.Data;
using BancoDigital.API.DTOs;
using BancoDigital.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoDigital.API.Controllers;

[ApiController]
[Route("api/agencias")]
public class AgenciasController : ControllerBase
{
    private readonly AppDbContext _context;

    public AgenciasController(AppDbContext context) => _context = context;

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] AgenciaCreateDto dto)
    {
        var agencia = new Agencia
        {
            Nome = dto.Nome,
            Endereco = dto.Endereco,
            Numero = dto.Numero
        };

        _context.Agencias.Add(agencia);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Obter), new { id = agencia.Id },
            new AgenciaResponseDto(agencia.Id, agencia.Nome, agencia.Endereco, agencia.Numero));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obter(int id)
    {
        var agencia = await _context.Agencias.FindAsync(id);
        if (agencia is null) return NotFound();

        return Ok(new AgenciaResponseDto(agencia.Id, agencia.Nome, agencia.Endereco, agencia.Numero));
    }
}