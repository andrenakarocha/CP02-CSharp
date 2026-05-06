using BancoDigital.API.Data;
using BancoDigital.API.DTOs;
using BancoDigital.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoDigital.API.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ClientesController(AppDbContext context) => _context = context;

    [HttpPost("pf")]
    public async Task<IActionResult> CriarPF([FromBody] PessoaFisicaCreateDto dto)
    {
        if (!await _context.Agencias.AnyAsync(a => a.Id == dto.AgenciaId))
            return NotFound(new { message = "Agência não encontrada." });

        if (await _context.Clientes.OfType<PessoaFisica>().AnyAsync(p => p.Cpf == dto.Cpf))
            return BadRequest(new { message = "CPF já cadastrado." });

        var pf = new PessoaFisica
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Telefone = dto.Telefone,
            AgenciaId = dto.AgenciaId,
            Cpf = dto.Cpf,
            DataNascimento = dto.DataNascimento
        };

        _context.Clientes.Add(pf);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Obter), new { id = pf.Id }, MapCliente(pf, "PF"));
    }

    [HttpPost("pj")]
    public async Task<IActionResult> CriarPJ([FromBody] PessoaJuridicaCreateDto dto)
    {
        if (!await _context.Agencias.AnyAsync(a => a.Id == dto.AgenciaId))
            return NotFound(new { message = "Agência não encontrada." });

        if (await _context.Clientes.OfType<PessoaJuridica>().AnyAsync(p => p.Cnpj == dto.Cnpj))
            return BadRequest(new { message = "CNPJ já cadastrado." });

        var pj = new PessoaJuridica
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Telefone = dto.Telefone,
            AgenciaId = dto.AgenciaId,
            Cnpj = dto.Cnpj,
            RazaoSocial = dto.RazaoSocial
        };

        _context.Clientes.Add(pj);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Obter), new { id = pj.Id }, MapCliente(pj, "PJ"));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obter(int id)
    {
        var cliente = await _context.Clientes
            .Include(c => c.Agencia)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cliente is null) return NotFound();

        return cliente switch
        {
            PessoaFisica pf => Ok(new ClienteResponseDto(
                pf.Id, pf.Nome, pf.Email, pf.Telefone, "PF",
                pf.AgenciaId, pf.Agencia.Nome,
                pf.Cpf, pf.DataNascimento, null, null)),
            PessoaJuridica pj => Ok(new ClienteResponseDto(
                pj.Id, pj.Nome, pj.Email, pj.Telefone, "PJ",
                pj.AgenciaId, pj.Agencia.Nome,
                null, null, pj.Cnpj, pj.RazaoSocial)),
            _ => NotFound()
        };
    }

    private static ClienteResponseDto MapCliente(Cliente c, string tipo) => c switch
    {
        PessoaFisica pf => new ClienteResponseDto(
            pf.Id, pf.Nome, pf.Email, pf.Telefone, tipo,
            pf.AgenciaId, pf.Agencia?.Nome ?? "",
            pf.Cpf, pf.DataNascimento, null, null),
        PessoaJuridica pj => new ClienteResponseDto(
            pj.Id, pj.Nome, pj.Email, pj.Telefone, tipo,
            pj.AgenciaId, pj.Agencia?.Nome ?? "",
            null, null, pj.Cnpj, pj.RazaoSocial),
        _ => throw new InvalidOperationException("Tipo de cliente desconhecido.")
    };
}