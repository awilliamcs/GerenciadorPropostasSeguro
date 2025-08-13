using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GPS.PropostaService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoBD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Propostas",
                columns: table => new
                {
                    IdProposta = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdPessoa = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DataSolicitacao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IdUsuarioResponsavel = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Propostas", x => x.IdProposta);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Propostas_IdPessoa",
                table: "Propostas",
                column: "IdPessoa");

            migrationBuilder.CreateIndex(
                name: "IX_Propostas_Status",
                table: "Propostas",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Propostas_Tipo_Status",
                table: "Propostas",
                columns: new[] { "Tipo", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Propostas");
        }
    }
}
