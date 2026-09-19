using FluentResults;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos.DTOs;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using Moq;

namespace Testes.Unidade.Modulos.Cursos;

[TestClass]
public sealed class CadastrarCursoCommandHandlerTests
{
    [TestMethod]
    public async Task Cadastrar_ComTodosCampos_PersisteCurso()
    {
        Mock<IRepositorioCurso> repositorioCurso = new();

        Curso? cursoCadastrado = null;

        repositorioCurso
            .Setup(r => r.CadastrarAsync(
                It.IsAny<Curso>(),
                It.IsAny<CancellationToken>()))
            .Callback<Curso, CancellationToken>(
                (curso, _) => cursoCadastrado = curso
            )
            .Returns(Task.CompletedTask);

        CadastrarCursoCommandHandler handler = new(
            repositorioCurso.Object
        );

        Result<CursoDto> resultado = await handler.Handle(
            new CadastrarCursoCommand(
                "Curso de C#",
                "Curso de desenvolvimento com C# e .NET",
                40,
                new DateOnly(2026, 12, 20)
            ),
            CancellationToken.None
        );

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(cursoCadastrado);

        Assert.AreEqual(
            "Curso de C#",
            cursoCadastrado.Nome
        );

        Assert.AreEqual(
            "Curso de desenvolvimento com C# e .NET",
            cursoCadastrado.DescricaoCurso
        );

        Assert.AreEqual(
            40,
            cursoCadastrado.CargaHoraria
        );

        Assert.AreEqual(
            new DateOnly(2026, 12, 20),
            cursoCadastrado.DataConclusao
        );

        Assert.AreEqual(
            "Curso de C#",
            resultado.Value.Nome
        );

        repositorioCurso.Verify(
            r => r.CadastrarAsync(
                It.IsAny<Curso>(),
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [TestMethod]
    public async Task Cadastrar_ComDadosInvalidos_NaoPersisteCurso()
    {
        Mock<IRepositorioCurso> repositorioCurso = new();

        CadastrarCursoCommandHandler handler = new(
            repositorioCurso.Object
        );

        Result<CursoDto> resultado = await handler.Handle(
            new CadastrarCursoCommand(
                string.Empty,
                "Curso de desenvolvimento com C# e .NET",
                40,
                new DateOnly(2026, 12, 20)
            ),
            CancellationToken.None
        );

        Assert.IsTrue(resultado.IsFailed);

        repositorioCurso.Verify(
            r => r.CadastrarAsync(
                It.IsAny<Curso>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}