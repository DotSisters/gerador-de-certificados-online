using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeradorDeCertificados.Infraestrutura.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarGeradoEmEConcluidoEm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataGeracao",
                table: "TBCertificados",
                newName: "GeradoEm");

            migrationBuilder.AddColumn<DateTime>(
                name: "ConcluidoEm",
                table: "TBProcessamentosCertificado",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConcluidoEm",
                table: "TBProcessamentosCertificado");

            migrationBuilder.RenameColumn(
                name: "GeradoEm",
                table: "TBCertificados",
                newName: "DataGeracao");
        }
    }
}
