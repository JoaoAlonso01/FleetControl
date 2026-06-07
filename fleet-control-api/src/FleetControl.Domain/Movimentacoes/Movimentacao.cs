using FleetControl.Domain.Shared;

namespace FleetControl.Domain.Movimentacoes;

public sealed class Movimentacao : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid VeiculoId { get; private set; }
    public Guid UsuarioId { get; private set; }
    public TipoMovimento Tipo { get; private set; }
    public int KilometragemSaida { get; private set; }
    public int? KilometragemRetorno { get; private set; }
    public string? Destino { get; private set; }
    public string? Observacao { get; private set; }
    public DateTimeOffset DataHora { get; private set; }
    public DateTimeOffset? DataHoraRetorno { get; private set; }
    public DateTimeOffset CriadoEm { get; private set; }

    private Movimentacao() { }

    public static Movimentacao RegistrarCheckOut(Guid veiculoId, Guid usuarioId, int quilometragemSaida, string? destino = null, string? observacao = null)
    {
        if (quilometragemSaida < 0)
            throw new MovimentacaoDomainException("Quilometragem de saída não pode ser negativa.");

        return new Movimentacao
        {
            Id = Guid.NewGuid(),
            VeiculoId = veiculoId,
            UsuarioId = usuarioId,
            Tipo = TipoMovimento.CheckOut,
            KilometragemSaida = quilometragemSaida,
            Destino = destino?.Trim(),
            Observacao = observacao?.Trim(),
            DataHora = DateTimeOffset.UtcNow,
            CriadoEm = DateTimeOffset.UtcNow
        };
    }

    public static Movimentacao RegistrarCheckIn(Guid veiculoId, Guid usuarioId, int quilometragemRetorno, string? observacao = null)
    {
        if (quilometragemRetorno < 0)
            throw new MovimentacaoDomainException("Quilometragem de retorno não pode ser negativa.");

        return new Movimentacao
        {
            Id = Guid.NewGuid(),
            VeiculoId = veiculoId,
            UsuarioId = usuarioId,
            Tipo = TipoMovimento.CheckIn,
            KilometragemSaida = 0,
            KilometragemRetorno = quilometragemRetorno,
            Observacao = observacao?.Trim(),
            DataHora = DateTimeOffset.UtcNow,
            DataHoraRetorno = DateTimeOffset.UtcNow,
            CriadoEm = DateTimeOffset.UtcNow
        };
    }

    public void RegistrarRetorno(int quilometragemRetorno, string? observacao = null)
    {
        if (Tipo != TipoMovimento.CheckOut || DataHoraRetorno is not null)
            throw new MovimentacaoDomainException("Movimentacao nao possui check-out aberto.");

        if (quilometragemRetorno < KilometragemSaida)
            throw new MovimentacaoDomainException("Quilometragem de retorno nao pode ser menor que a de saida.");

        KilometragemRetorno = quilometragemRetorno;
        DataHoraRetorno = DateTimeOffset.UtcNow;

        if (!string.IsNullOrWhiteSpace(observacao))
            Observacao = observacao.Trim();
    }
}
