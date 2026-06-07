using FleetControl.Domain.Shared;

namespace FleetControl.Domain.Veiculos;

public sealed class Veiculo : AggregateRoot
{
    public Guid Id { get; private set; }
    public string Modelo { get; private set; }
    public string Marca { get; private set; }
    public int Ano { get; private set; }
    public Placa Placa { get; private set; }
    public StatusVeiculo Status { get; private set; }
    public int KilometragemAtual { get; private set; }
    public DateTimeOffset CriadoEm { get; private set; }
    public DateTimeOffset AtualizadoEm { get; private set; }

    private Veiculo() { }

    public static Veiculo Criar(string modelo, string marca, int ano, string placa)
    {
        var veiculo = new Veiculo
        {
            Id = Guid.NewGuid(),
            Modelo = modelo.Trim(),
            Marca = marca.Trim(),
            Ano = ano,
            Placa = new Placa(placa),
            Status = StatusVeiculo.Disponivel,
            KilometragemAtual = 0,
            CriadoEm = DateTimeOffset.UtcNow,
            AtualizadoEm = DateTimeOffset.UtcNow
        };

        veiculo.AddDomainEvent(new VeiculoRegistradoEvent(veiculo.Id, placa));
        return veiculo;
    }

    public void AtualizarKilometragem(int quilometragem)
    {
        if (quilometragem < KilometragemAtual)
            throw new VeiculoDomainException("Quilometragem não pode ser menor que a atual.");

        KilometragemAtual = quilometragem;
        AtualizadoEm = DateTimeOffset.UtcNow;
    }

    public void AtualizarDados(string modelo, string marca, int ano, string placa)
    {
        Modelo = modelo.Trim();
        Marca = marca.Trim();
        Ano = ano;
        Placa = new Placa(placa);
        AtualizadoEm = DateTimeOffset.UtcNow;
    }

    public void IniciarUso()
    {
        if (Status != StatusVeiculo.Disponivel)
            throw new VeiculoDomainException($"Veículo não está disponível. Status atual: {Status}.");

        Status = StatusVeiculo.EmUso;
        AtualizadoEm = DateTimeOffset.UtcNow;
    }

    public void FinalizarUso()
    {
        if (Status != StatusVeiculo.EmUso)
            throw new VeiculoDomainException("Veículo não está em uso.");

        Status = StatusVeiculo.Disponivel;
        AtualizadoEm = DateTimeOffset.UtcNow;
    }

    public void EnviarParaManutencao()
    {
        Status = StatusVeiculo.EmManutencao;
        AtualizadoEm = DateTimeOffset.UtcNow;
    }

    public void RetornarDaManutencao()
    {
        if (Status != StatusVeiculo.EmManutencao)
            throw new VeiculoDomainException("Veículo não está em manutenção.");

        Status = StatusVeiculo.Disponivel;
        AtualizadoEm = DateTimeOffset.UtcNow;
    }

    public void Desativar()
    {
        Status = StatusVeiculo.Inativo;
        AtualizadoEm = DateTimeOffset.UtcNow;
    }
}
