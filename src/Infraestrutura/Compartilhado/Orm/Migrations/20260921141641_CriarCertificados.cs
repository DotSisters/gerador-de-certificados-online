using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeradorDeCertificados.Infraestrutura.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class CriarCertificados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBProcessamentosCertificado",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CursoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CaminhoZip = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DataSolicitacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBProcessamentosCertificado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBProcessamentosCertificado_TBCursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "TBCursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TBCertificados",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProcessamentoId = table.Column<Guid>(type: "uuid", nullable: false),
                    NomeAluno = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CaminhoArquivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DataGeracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBCertificados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBCertificados_TBProcessamentosCertificado_ProcessamentoId",
                        column: x => x.ProcessamentoId,
                        principalTable: "TBProcessamentosCertificado",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBCertificados_ProcessamentoId",
                table: "TBCertificados",
                column: "ProcessamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBProcessamentosCertificado_CursoId",
                table: "TBProcessamentosCertificado",
                column: "CursoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBCertificados");

            migrationBuilder.DropTable(
                name: "TBProcessamentosCertificado");
        }
    }
}
