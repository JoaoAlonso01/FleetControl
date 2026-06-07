using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetControl.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Manutencoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    VeiculoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Oficina = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CustoEstimado = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CustoReal = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    DataAgendada = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    DataConclusao = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    CriadoEm = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manutencoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movimentacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    VeiculoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    KilometragemSaida = table.Column<int>(type: "INTEGER", nullable: false),
                    KilometragemRetorno = table.Column<int>(type: "INTEGER", nullable: true),
                    Destino = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    Observacao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DataHora = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    DataHoraRetorno = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    CriadoEm = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movimentacoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Perfil = table.Column<int>(type: "INTEGER", nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    NumeroCnh = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    CriadoEm = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Veiculos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Modelo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Marca = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Ano = table.Column<int>(type: "INTEGER", nullable: false),
                    PlacaValor = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    KilometragemAtual = table.Column<int>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    AtualizadoEm = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veiculos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Manutencoes_VeiculoId",
                table: "Manutencoes",
                column: "VeiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_Movimentacoes_UsuarioId",
                table: "Movimentacoes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Movimentacoes_VeiculoId",
                table: "Movimentacoes",
                column: "VeiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Veiculos_PlacaValor",
                table: "Veiculos",
                column: "PlacaValor",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Manutencoes");

            migrationBuilder.DropTable(
                name: "Movimentacoes");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Veiculos");
        }
    }
}
