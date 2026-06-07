namespace FleetControl.Domain.Manutencoes;

public enum TipoManutencao
{
    Preventiva = 1,
    Corretiva = 2,
    Revisao = 3
}

public enum StatusManutencao
{
    Agendada = 1,
    EmAndamento = 2,
    Concluida = 3,
    Cancelada = 4
}
