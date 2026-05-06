namespace BancoDigital.API.Models;

public class ReceberSalario : Produto
{
    public string EmpresaConveniada { get; set; } = string.Empty;
    public decimal TaxaPortabilidade { get; set; }

    private static readonly HashSet<string> _empresasValidas = new(StringComparer.OrdinalIgnoreCase)
    {
        "FIAP", "ACCENTURE", "ITAU", "BRADESCO", "AMBEV", "PETROBRAS"
    };

    public bool ValidarConvenio(string empresa) => _empresasValidas.Contains(empresa);
}