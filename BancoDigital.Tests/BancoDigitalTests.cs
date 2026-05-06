using BancoDigital.API.Controllers;
using BancoDigital.API.Data;
using BancoDigital.API.DTOs;
using BancoDigital.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BancoDigital.Tests;

public class BancoDigitalTests
{
    private AppDbContext CriarContexto(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AppDbContext(options);
    }

    private async Task<Agencia> SeedAgencia(AppDbContext ctx)
    {
        var agencia = new Agencia { Nome = "Agência Central", Endereco = "Rua A, 1", Numero = "001" };
        ctx.Agencias.Add(agencia);
        await ctx.SaveChangesAsync();
        return agencia;
    }

    [Fact]
    public async Task CriarPF_CpfDuplicado_Retorna400()
    {
        using var ctx = CriarContexto(nameof(CriarPF_CpfDuplicado_Retorna400));
        var agencia = await SeedAgencia(ctx);
        var controller = new ClientesController(ctx);

        var dto = new PessoaFisicaCreateDto("João", "joao@email.com", "11999999999",
            agencia.Id, "111.111.111-11", new DateTime(1990, 1, 1));

        await controller.CriarPF(dto);
        var resultado = await controller.CriarPF(dto);

        Assert.IsType<BadRequestObjectResult>(resultado);
    }

    [Fact]
    public async Task CriarPJ_CnpjDuplicado_Retorna400()
    {
        using var ctx = CriarContexto(nameof(CriarPJ_CnpjDuplicado_Retorna400));
        var agencia = await SeedAgencia(ctx);
        var controller = new ClientesController(ctx);

        var dto = new PessoaJuridicaCreateDto("Empresa X", "empresa@email.com", "1133333333",
            agencia.Id, "00.000.000/0001-00", "Empresa X Ltda");

        await controller.CriarPJ(dto);
        var resultado = await controller.CriarPJ(dto);

        Assert.IsType<BadRequestObjectResult>(resultado);
    }

    [Fact]
    public async Task CriarPF_AgenciaInexistente_Retorna404()
    {
        using var ctx = CriarContexto(nameof(CriarPF_AgenciaInexistente_Retorna404));
        var controller = new ClientesController(ctx);

        var dto = new PessoaFisicaCreateDto("Maria", "maria@email.com", "11988888888",
            9999, "222.222.222-22", new DateTime(1985, 5, 15));

        var resultado = await controller.CriarPF(dto);

        Assert.IsType<NotFoundObjectResult>(resultado);
    }

    [Fact]
    public async Task SolicitarContratacaoEmprestimo_ScoreAlto_TaxaCorreta()
    {
        using var ctx = CriarContexto(nameof(SolicitarContratacaoEmprestimo_ScoreAlto_TaxaCorreta));
        var agencia = await SeedAgencia(ctx);

        var cliente = new PessoaFisica
        {
            Nome = "Carlos", Email = "carlos@email.com", Telefone = "11977777777",
            AgenciaId = agencia.Id, Cpf = "333.333.333-33", DataNascimento = new DateTime(1988, 3, 20)
        };
        ctx.Clientes.Add(cliente);

        var emprestimo = new Emprestimo
        {
            Nome = "Empréstimo Pessoal", ValorMaximo = 50000m, PrazoMaximoMeses = 60
        };
        ctx.Produtos.Add(emprestimo);
        await ctx.SaveChangesAsync();

        var controller = new ContratacoesController(ctx);
        var dto = new ContratacaoCreateDto(cliente.Id, emprestimo.Id, 10000m, 850, null);

        var resultado = await controller.Criar(dto);

        var created = Assert.IsType<CreatedAtActionResult>(resultado);
        var response = Assert.IsType<ContratacaoResponseDto>(created.Value);
        Assert.Equal(0.015m, response.TaxaMensalCalculada);
        Assert.Equal("Aprovada", response.Status);
    }

    [Fact]
    public async Task SolicitarContratacao_ClienteInexistente_Retorna404()
    {
        using var ctx = CriarContexto(nameof(SolicitarContratacao_ClienteInexistente_Retorna404));

        var emprestimo = new Emprestimo
        {
            Nome = "Empréstimo Pessoal", ValorMaximo = 50000m, PrazoMaximoMeses = 60
        };
        ctx.Produtos.Add(emprestimo);
        await ctx.SaveChangesAsync();

        var controller = new ContratacoesController(ctx);
        var dto = new ContratacaoCreateDto(9999, emprestimo.Id, 5000m, 700, null);

        var resultado = await controller.Criar(dto);

        Assert.IsType<NotFoundObjectResult>(resultado);
    }

    [Fact]
    public async Task SolicitarContratacaoReceberSalario_EmpresaInvalida_Retorna400()
    {
        using var ctx = CriarContexto(nameof(SolicitarContratacaoReceberSalario_EmpresaInvalida_Retorna400));
        var agencia = await SeedAgencia(ctx);

        var cliente = new PessoaFisica
        {
            Nome = "Ana", Email = "ana@email.com", Telefone = "11966666666",
            AgenciaId = agencia.Id, Cpf = "444.444.444-44", DataNascimento = new DateTime(1992, 8, 10)
        };
        ctx.Clientes.Add(cliente);

        var receberSalario = new ReceberSalario
        {
            Nome = "Receber Salário", EmpresaConveniada = "FIAP", TaxaPortabilidade = 0m
        };
        ctx.Produtos.Add(receberSalario);
        await ctx.SaveChangesAsync();

        var controller = new ContratacoesController(ctx);
        var dto = new ContratacaoCreateDto(cliente.Id, receberSalario.Id, null, null, "EMPRESA_INVALIDA");

        var resultado = await controller.Criar(dto);

        Assert.IsType<BadRequestObjectResult>(resultado);
    }

    [Fact]
    public async Task ConsultarContratacao_RetornaStatusAprovada()
    {
        using var ctx = CriarContexto(nameof(ConsultarContratacao_RetornaStatusAprovada));
        var agencia = await SeedAgencia(ctx);

        var cliente = new PessoaFisica
        {
            Nome = "Pedro", Email = "pedro@email.com", Telefone = "11955555555",
            AgenciaId = agencia.Id, Cpf = "555.555.555-55", DataNascimento = new DateTime(1980, 12, 25)
        };
        ctx.Clientes.Add(cliente);

        var emprestimo = new Emprestimo
        {
            Nome = "Empréstimo Pessoal", ValorMaximo = 50000m, PrazoMaximoMeses = 60
        };
        ctx.Produtos.Add(emprestimo);
        await ctx.SaveChangesAsync();

        var contratacoesController = new ContratacoesController(ctx);
        var createDto = new ContratacaoCreateDto(cliente.Id, emprestimo.Id, 20000m, 750, null);
        var createResult = await contratacoesController.Criar(createDto);

        var created = Assert.IsType<CreatedAtActionResult>(createResult);
        var contratacaoId = ((ContratacaoResponseDto)created.Value!).Id;

        var getResult = await contratacoesController.Obter(contratacaoId);

        var ok = Assert.IsType<OkObjectResult>(getResult);
        var response = Assert.IsType<ContratacaoResponseDto>(ok.Value);
        Assert.Equal("Aprovada", response.Status);
    }
}