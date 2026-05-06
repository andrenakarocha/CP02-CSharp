namespace BancoDigital.API.Models;

public class Emprestimo : Produto
{
    public decimal ValorMaximo { get; set; }
    public int PrazoMaximoMeses { get; set; }

    public decimal CalcularTaxaMensal(int scoreCredito) => scoreCredito switch
    {
        >= 800 => 0.015m,
        >= 600 => 0.025m,
        >= 400 => 0.040m,
        _      => 0.060m
    };
}