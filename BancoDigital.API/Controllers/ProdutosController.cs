using BancoDigital.API.Data;
using BancoDigital.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoDigital.API.Controllers;

[ApiController]
[Route("api/produtos")]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutosController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var produtos = await _context.Produtos.ToListAsync();

        var response = produtos.Select(p => new
        {
            p.Id,
            p.Nome,
            Tipo = p switch
            {
                Emprestimo e => new
                {
                    Categoria = "EMPRESTIMO",
                    e.ValorMaximo,
                    e.PrazoMaximoMeses,
                    TabelaDeTaxas = new[]
                    {
                        new { ScoreMinimo = 800, ScoreMaximo = 1000, TaxaMensal = "1,50%" },
                        new { ScoreMinimo = 600, ScoreMaximo = 799, TaxaMensal = "2,50%" },
                        new { ScoreMinimo = 400, ScoreMaximo = 599, TaxaMensal = "4,00%" },
                        new { ScoreMinimo = 0,   ScoreMaximo = 399, TaxaMensal = "6,00%" },
                    }
                } as object,
                ReceberSalario rs => new
                {
                    Categoria = "RECEBER_SALARIO",
                    EmpresasConveniadas = new[]
                    {
                        "FIAP", "ACCENTURE", "ITAU", "BRADESCO", "AMBEV", "PETROBRAS"
                    }
                } as object,
                MaquinaDeCartao mc => new
                {
                    Categoria = "MAQUINA_CARTAO",
                    mc.Bandeira
                } as object,
                _ => new { Categoria = "DESCONHECIDO" } as object
            }
        });

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obter(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto is null) return NotFound();

        return Ok(produto);
    }
}