using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GPS.ContratacaoService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoBD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contratacoes",
                columns: table => new
                {
                    IdContratacao = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdProposta = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataContratacao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contratacoes", x => x.IdContratacao);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contratacoes_IdProposta",
                table: "Contratacoes",
                column: "IdProposta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contratacoes");
        }
    }
}
