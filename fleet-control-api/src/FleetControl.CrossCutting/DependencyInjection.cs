using FleetControl.Application.Manutencoes;
using FleetControl.Application.Movimentacoes;
using FleetControl.Application.Services.Manutencoes;
using FleetControl.Application.Services.Movimentacoes;
using FleetControl.Application.Services.Usuarios;
using FleetControl.Application.Services.Veiculos;
using FleetControl.Application.Usuarios;
using FleetControl.Application.Veiculos;
using FleetControl.Domain.Manutencoes;
using FleetControl.Domain.Movimentacoes;
using FleetControl.Domain.Usuarios;
using FleetControl.Domain.Veiculos;
using FleetControl.Infra.Data;
using FleetControl.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FleetControl.CrossCutting;

public static class DependencyInjection
{
    public static IServiceCollection AddFleetControlServices(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<FleetControlDbContext>(options =>
            options.UseSqlite(connectionString));

        // Repositories — Scoped (por request)
        services.AddScoped<IVeiculoRepository, VeiculoRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IManutencaoRepository, ManutencaoRepository>();
        services.AddScoped<IMovimentacaoRepository, MovimentacaoRepository>();

        // Application Services — Scoped (dependem dos repositories)
        services.AddScoped<IVeiculoService, VeiculoService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IManutencaoService, ManutencaoService>();
        services.AddScoped<IMovimentacaoService, MovimentacaoService>();

        return services;
    }

    public static void ApplyMigrations(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FleetControlDbContext>();
        db.Database.Migrate();
    }
}
