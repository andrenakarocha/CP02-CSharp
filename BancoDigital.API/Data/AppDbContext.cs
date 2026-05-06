using BancoDigital.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BancoDigital.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Agencia> Agencias => Set<Agencia>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Contratacao> Contratacoes => Set<Contratacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TB_AGENCIAS
        modelBuilder.Entity<Agencia>(e =>
        {
            e.ToTable("TB_AGENCIAS");
            e.HasKey(a => a.Id);
            e.Property(a => a.Nome).IsRequired().HasMaxLength(100);
            e.Property(a => a.Endereco).IsRequired().HasMaxLength(200);
            e.Property(a => a.Numero).IsRequired().HasMaxLength(20);
        });

        // TB_CLIENTES - TPH
        modelBuilder.Entity<Cliente>(e =>
        {
            e.ToTable("TB_CLIENTES");
            e.HasKey(c => c.Id);
            e.HasDiscriminator<string>("Tipo")
                .HasValue<PessoaFisica>("PF")
                .HasValue<PessoaJuridica>("PJ");
            e.Property(c => c.Nome).IsRequired().HasMaxLength(150);
            e.Property(c => c.Email).IsRequired().HasMaxLength(150);
            e.Property(c => c.Telefone).HasMaxLength(20);
            e.HasOne(c => c.Agencia)
                .WithMany(a => a.Clientes)
                .HasForeignKey(c => c.AgenciaId);
        });

        modelBuilder.Entity<PessoaFisica>(e =>
        {
            e.Property(p => p.Cpf).HasMaxLength(14);
            e.HasIndex(p => p.Cpf).IsUnique();
        });

        modelBuilder.Entity<PessoaJuridica>(e =>
        {
            e.Property(p => p.Cnpj).HasMaxLength(18);
            e.HasIndex(p => p.Cnpj).IsUnique();
            e.Property(p => p.RazaoSocial).HasMaxLength(200);
        });

        // TB_PRODUTOS - TPH
        modelBuilder.Entity<Produto>(e =>
        {
            e.ToTable("TB_PRODUTOS");
            e.HasKey(p => p.Id);
            e.HasDiscriminator<string>("TipoProduto")
                .HasValue<Emprestimo>("EMPRESTIMO")
                .HasValue<ReceberSalario>("RECEBER_SALARIO")
                .HasValue<MaquinaDeCartao>("MAQUINA_CARTAO");
            e.Property(p => p.Nome).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Emprestimo>(e =>
        {
            e.Property(p => p.ValorMaximo).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<ReceberSalario>(e =>
        {
            e.Property(p => p.EmpresaConveniada).HasMaxLength(100);
            e.Property(p => p.TaxaPortabilidade).HasColumnType("decimal(5,4)");
        });

        // TB_CONTRATACOES
        modelBuilder.Entity<Contratacao>(e =>
        {
            e.ToTable("TB_CONTRATACOES");
            e.HasKey(c => c.Id);
            e.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);
            e.Property(c => c.ValorSolicitado).HasColumnType("decimal(18,2)");
            e.Property(c => c.TaxaMensalCalculada).HasColumnType("decimal(5,4)");
            e.Property(c => c.Observacao).HasMaxLength(500);
            e.Property(c => c.EmpresaEmpregadora).HasMaxLength(100);
            e.HasOne(c => c.Cliente)
                .WithMany(cl => cl.Contratacoes)
                .HasForeignKey(c => c.ClienteId);
            e.HasOne(c => c.Produto)
                .WithMany(p => p.Contratacoes)
                .HasForeignKey(c => c.ProdutoId);
        });

        // SEED
        modelBuilder.Entity<Emprestimo>().HasData(new Emprestimo
        {
            Id = 1,
            Nome = "Empréstimo Pessoal",
            ValorMaximo = 50000m,
            PrazoMaximoMeses = 60
        });

        modelBuilder.Entity<ReceberSalario>().HasData(new ReceberSalario
        {
            Id = 2,
            Nome = "Receber Salário",
            EmpresaConveniada = "FIAP",
            TaxaPortabilidade = 0m
        });

        modelBuilder.Entity<MaquinaDeCartao>().HasData(new MaquinaDeCartao
        {
            Id = 3,
            Nome = "Maquininha de Cartão",
            Bandeira = ""
        });
    }
}