namespace BancoDigital.API.DTOs;

public record AgenciaCreateDto(string Nome, string Endereco, string Numero);

public record AgenciaResponseDto(int Id, string Nome, string Endereco, string Numero);