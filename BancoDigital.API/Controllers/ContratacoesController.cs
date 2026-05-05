using BancoDigital.API.Data;
using BancoDigital.API.DTOs;
using BancoDigital.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoDigital.API.Controllers;

[ApiController]
[Route("api/contratacoes")]
public class ContratacoesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ContratacoesController(AppDbContext context) => _context = context;

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] ContratacaoCreateDto dto)
    {
        if (!await _context.Clientes.AnyAsync(c => c.Id == dto.ClienteId))
            return NotFound(new { message = "Cliente não encontrado." });

        var produto = await _context.Produtos.FindAsync(dto.ProdutoId);
        if (produto is null)
            return NotFound(new { message = "Produto não encontrado." });

        var contratacao = new Contratacao
        {
            ClienteId = dto.ClienteId,
            ProdutoId = dto.ProdutoId,
            DataSolicitacao = DateTime.UtcNow,
            ValorSolicitado = dto.ValorSolicitado,
            ScoreCredito = dto.ScoreCredito,
            EmpresaEmpregadora = dto.EmpresaEmpregadora,
            Status = StatusContratacao.Pendente
        };

        if (produto is Emprestimo emprestimo)
        {
            var score = dto.ScoreCredito ?? 0;
            var taxa = emprestimo.CalcularTaxaMensal(score);
            contratacao.TaxaMensalCalculada = taxa;
            contratacao.Status = StatusContratacao.Aprovada;
            contratacao.Observacao = $"Taxa mensal calculada: {taxa:P2}";
        }
        else if (produto is ReceberSalario receberSalario)
        {
            var empresa = dto.EmpresaEmpregadora ?? string.Empty;
            if (!receberSalario.ValidarConvenio(empresa))
                return BadRequest(new { message = $"Empresa '{empresa}' não possui convênio com o banco." });

            contratacao.Status = StatusContratacao.Aprovada;
            contratacao.Observacao = $"Convênio validado para empresa: {empresa.ToUpper()}";
        }
        else
        {
            contratacao.Status = StatusContratacao.Aprovada;
        }

        contratacao.DataProcessamento = DateTime.UtcNow;

        _context.Contratacoes.Add(contratacao);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Obter), new { id = contratacao.Id }, MapContratacao(contratacao));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obter(int id)
    {
        var contratacao = await _context.Contratacoes
            .Include(c => c.Cliente)
            .Include(c => c.Produto)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contratacao is null) return NotFound();

        return Ok(MapContratacao(contratacao));
    }

    private static ContratacaoResponseDto MapContratacao(Contratacao c) => new(
        c.Id,
        c.ClienteId,
        c.ProdutoId,
        c.Status.ToString(),
        c.DataSolicitacao,
        c.DataProcessamento,
        c.Observacao,
        c.ValorSolicitado,
        c.ScoreCredito,
        c.TaxaMensalCalculada,
        c.EmpresaEmpregadora
    );
}