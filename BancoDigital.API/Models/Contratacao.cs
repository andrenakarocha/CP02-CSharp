namespace BancoDigital.API.Models;

public enum StatusContratacao
{
    Pendente,
    Aprovada,
    Reprovada
}

public class Contratacao
{
    public int Id { get; set; }

    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public int ProdutoId { get; set; }
    public Produto Produto { get; set; } = null!;

    public StatusContratacao Status { get; set; }
    public DateTime DataSolicitacao { get; set; }
    public DateTime? DataProcessamento { get; set; }
    public string? Observacao { get; set; }

    public decimal? ValorSolicitado { get; set; }
    public int? ScoreCredito { get; set; }
    public decimal? TaxaMensalCalculada { get; set; }
    public string? EmpresaEmpregadora { get; set; }
}