namespace BancoDigital.API.DTOs;

public record PessoaFisicaCreateDto(
    string Nome,
    string Email,
    string Telefone,
    int AgenciaId,
    string Cpf,
    DateTime DataNascimento
);

public record PessoaJuridicaCreateDto(
    string Nome,
    string Email,
    string Telefone,
    int AgenciaId,
    string Cnpj,
    string RazaoSocial
);

public record ClienteResponseDto(
    int Id,
    string Nome,
    string Email,
    string Telefone,
    string Tipo,
    int AgenciaId,
    string AgenciaNome,
    string? Cpf,
    DateTime? DataNascimento,
    string? Cnpj,
    string? RazaoSocial
);