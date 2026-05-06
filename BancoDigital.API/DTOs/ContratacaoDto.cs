namespace BancoDigital.API.DTOs;

public record ContratacaoCreateDto(
    int ClienteId,
    int ProdutoId,
    decimal? ValorSolicitado,
    int? ScoreCredito,
    string? EmpresaEmpregadora
);

public record ContratacaoResponseDto(
    int Id,
    int ClienteId,
    int ProdutoId,
    string Status,
    DateTime DataSolicitacao,
    DateTime? DataProcessamento,
    string? Observacao,
    decimal? ValorSolicitado,
    int? ScoreCredito,
    decimal? TaxaMensalCalculada,
    string? EmpresaEmpregadora
);