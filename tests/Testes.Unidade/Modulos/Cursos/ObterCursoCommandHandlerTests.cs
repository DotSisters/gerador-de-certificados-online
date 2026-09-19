using FluentResults;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos.DTOs;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using Moq;

namespace Testes.Unidade.Modulos.Cursos;

[TestClass]
public sealed class ObterCursoPorIdQueryHandlerTests
{
    [TestMethod]
    public async Task ObterPorId_CursoExistente_RetornaCurso()
    {
        Mock<IRepositorioCurso> repositorioCurso = new();

        Curso curso = new(
            Guid.CreateVersion7(),
            "Curso de C#",
            "Curso de desenvolvimento com C# e .NET",
            40,
            new DateOnly(2026, 12, 20)
        );

        repositorioCurso
            .Setup(r => r.SelecionarPorIdAsync(
                curso.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(curso);

        ObterCursoPorIdQueryHandler handler = new(
            repositorioCurso.Object
        );

        Result<CursoDto> resultado = await handler.Handle(
            new ObterCursoPorIdQuery(curso.Id),
            CancellationToken.None
        );

        Assert.IsTrue(resultado.IsSuccess);

        Assert.AreEqual(curso.Id, resultado.Value.Id);
        Assert.AreEqual(curso.Nome, resultado.Value.Nome);
        Assert.AreEqual(
            curso.DescricaoCurso,
            resultado.Value.DescricaoCurso
        );
        Assert.AreEqual(
            curso.CargaHoraria,
            resultado.Value.CargaHoraria
        );
        Assert.AreEqual(
            curso.DataConclusao,
            resultado.Value.DataConclusao
        );

        repositorioCurso.Verify(
            r => r.SelecionarPorIdAsync(
                curso.Id,
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [TestMethod]
    public async Task ObterPorId_CursoNaoEncontrado_RetornaErro()
    {
        Mock<IRepositorioCurso> repositorioCurso = new();

        Guid cursoId = Guid.CreateVersion7();

        repositorioCurso
            .Setup(r => r.SelecionarPorIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Curso?)null);

        ObterCursoPorIdQueryHandler handler = new(
            repositorioCurso.Object
        );

        Result<CursoDto> resultado = await handler.Handle(
            new ObterCursoPorIdQuery(cursoId),
            CancellationToken.None
        );

        Assert.IsTrue(resultado.IsFailed);

        repositorioCurso.Verify(
            r => r.SelecionarPorIdAsync(
                cursoId,
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}