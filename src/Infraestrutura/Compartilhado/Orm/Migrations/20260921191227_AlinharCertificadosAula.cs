using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeradorDeCertificados.Infraestrutura.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class AlinharCertificadosAula : Migration
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

            // 0 Pendente, 1 GerandoCertificados, 2 GerandoZip, 3 Concluido, 4 Falha
            // → 0 Pendente, 1 Concluido, 2 ConcluidoComFalhas, 3 Falha
            migrationBuilder.Sql("""
                UPDATE "TBProcessamentosCertificado"
                SET "Status" = CASE "Status"
                    WHEN 0 THEN 0
                    WHEN 1 THEN 0
                    WHEN 2 THEN 0
                    WHEN 3 THEN 1
                    WHEN 4 THEN 3
                    ELSE "Status"
                END;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE "TBProcessamentosCertificado"
                SET "Status" = CASE "Status"
                    WHEN 0 THEN 0
                    WHEN 1 THEN 3
                    WHEN 2 THEN 4
                    WHEN 3 THEN 4
                    ELSE "Status"
                END;
                """);

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
