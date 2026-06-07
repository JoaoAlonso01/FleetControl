using FleetControl.Domain.Shared;

namespace FleetControl.Domain.Manutencoes;

public sealed class Manutencao : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid VeiculoId { get; private set; }
    public TipoManutencao Tipo { get; private set; }
    public StatusManutencao Status { get; private set; }
    public string Descricao { get; private set; }
    public string? Oficina { get; private set; }
    public decimal? CustoEstimado { get; private set; }
    public decimal? CustoReal { get; private set; }
    public DateTimeOffset DataAgendada { get; private set; }
    public DateTimeOffset? DataConclusao { get; private set; }
    public DateTimeOffset CriadoEm { get; private set; }
    public DateTimeOffset AtualizadoEm { get; private set; }

    private Manutencao() { }

    public static Manutencao Agendar(Guid veiculoId, TipoManutencao tipo, string descricao, DateTimeOffset dataAgendada, string? oficina = null, decimal? custoEstimado = null)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ManutencaoDomainException("Descrição é obrigatória.");

        if (dataAgendada <= DateTimeOffset.UtcNow)
            throw new ManutencaoDomainException("Data agendada deve ser no futuro.");

        var manutencao = new Manutencao
        {
            Id = Guid.NewGuid(),
            VeiculoId = veiculoId,
            Tipo = tipo,
            Status = StatusManutencao.Agendada,
            Descricao = descricao.Trim(),
            Oficina = oficina?.Trim(),
            CustoEstimado = custoEstimado,
            DataAgendada = dataAgendada,
            CriadoEm = DateTimeOffset.UtcNow,
            AtualizadoEm = DateTimeOffset.UtcNow
        };

        manutencao.AddDomainEvent(new ManutencaoAgendadaEvent(manutencao.Id, veiculoId, dataAgendada));
        return manutencao;
    }

    public void Iniciar()
    {
        if (Status != StatusManutencao.Agendada)
            throw new ManutencaoDomainException("Manutenção não está agendada.");

        Status = StatusManutencao.EmAndamento;
        AtualizadoEm = DateTimeOffset.UtcNow;
    }

    public void Concluir(decimal custoReal)
    {
        if (Status != StatusManutencao.EmAndamento)
            throw new ManutencaoDomainException("Manutenção não está em andamento.");

        Status = StatusManutencao.Concluida;
        CustoReal = custoReal;
        DataConclusao = DateTimeOffset.UtcNow;
        AtualizadoEm = DateTimeOffset.UtcNow;
    }

    public void Cancelar()
    {
        if (Status == StatusManutencao.Concluida)
            throw new ManutencaoDomainException("Manutenção já concluída não pode ser cancelada.");

        Status = StatusManutencao.Cancelada;
        AtualizadoEm = DateTimeOffset.UtcNow;
    }
}
