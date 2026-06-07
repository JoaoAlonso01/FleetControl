namespace FleetControl.Domain.Veiculos;

public sealed record Placa
{
    public string Valor { get; }

    public Placa(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new PlacaInvalidaException("Placa não pode ser vazia.");

        var normalizado = valor.Trim().ToUpperInvariant().Replace("-", "");

        if (normalizado.Length != 7)
            throw new PlacaInvalidaException($"Placa '{valor}' inválida. Formato esperado: ABC1234 ou ABC1D23.");

        Valor = normalizado;
    }

    public override string ToString() => $"{Valor[..3]}-{Valor[3..]}";
}
