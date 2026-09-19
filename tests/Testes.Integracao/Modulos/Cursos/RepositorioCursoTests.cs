using GeradorDeCertificados.Dominio.Modulos.Cursos;
using Testes.Integracao.Compartilhado.Orm;

namespace Testes.Integracao.Modulos.Cursos;

[TestClass]
public sealed class RepositorioCursoTests : RepositorioBaseTests
{
    [TestMethod]
    public async Task Cadastrar_ComTodosOsCampos_DevePersistirCurso()
    {
        Curso curso = new(
            Guid.CreateVersion7(),
            "Curso de C#",
            "Curso de desenvolvimento com C# e .NET",
            40,
            new DateOnly(2026, 12, 20)
        );

        await repositorioCurso.CadastrarAsync(curso);

        dbContext.ChangeTracker.Clear();

        Curso? cursoSelecionado =
            await repositorioCurso.SelecionarPorIdAsync(curso.Id);

        Assert.IsNotNull(cursoSelecionado);
        Assert.AreEqual("Curso de C#", cursoSelecionado.Nome);
        Assert.AreEqual(
            "Curso de desenvolvimento com C# e .NET",
            cursoSelecionado.DescricaoCurso
        );
        Assert.AreEqual(40, cursoSelecionado.CargaHoraria);
        Assert.AreEqual(
            new DateOnly(2026, 12, 20),
            cursoSelecionado.DataConclusao
        );
    }

    [TestMethod]
    public async Task Editar_ComDadosValidos_AtualizaCurso()
    {
        Curso curso = new(
            Guid.CreateVersion7(),
            "Curso de C#",
            "Curso de desenvolvimento com C# e .NET",
            40,
            new DateOnly(2026, 12, 20)
        );

        await repositorioCurso.CadastrarAsync(curso);

        dbContext.ChangeTracker.Clear();

        Curso cursoAtualizado = new(
            Guid.CreateVersion7(),
            "Curso de .NET",
            "Curso avançado de desenvolvimento com .NET",
            80,
            new DateOnly(2027, 01, 30)
        );

        bool conseguiuEditar = await repositorioCurso.EditarAsync(
            curso.Id,
            cursoAtualizado
        );

        dbContext.ChangeTracker.Clear();

        Curso? cursoSelecionado =
            await repositorioCurso.SelecionarPorIdAsync(curso.Id);

        Assert.IsTrue(conseguiuEditar);
        Assert.IsNotNull(cursoSelecionado);
        Assert.AreEqual("Curso de .NET", cursoSelecionado.Nome);
        Assert.AreEqual(
            "Curso avançado de desenvolvimento com .NET",
            cursoSelecionado.DescricaoCurso
        );
        Assert.AreEqual(80, cursoSelecionado.CargaHoraria);
        Assert.AreEqual(
            new DateOnly(2027, 01, 30),
            cursoSelecionado.DataConclusao
        );
    }

    [TestMethod]
    public async Task SelecionarPorId_CursoCadastrado_RetornaCurso()
    {
        Curso curso = new(
            Guid.CreateVersion7(),
            "Curso de C#",
            "Curso de desenvolvimento com C# e .NET",
            40,
            new DateOnly(2026, 12, 20)
        );

        await repositorioCurso.CadastrarAsync(curso);

        dbContext.ChangeTracker.Clear();

        Curso? cursoSelecionado =
            await repositorioCurso.SelecionarPorIdAsync(curso.Id);

        Assert.IsNotNull(cursoSelecionado);
        Assert.AreEqual(curso.Id, cursoSelecionado.Id);
        Assert.AreEqual(curso.Nome, cursoSelecionado.Nome);
    }

    [TestMethod]
    public async Task SelecionarPorId_CursoInexistente_RetornaNull()
    {
        Guid cursoId = Guid.CreateVersion7();

        Curso? cursoSelecionado =
            await repositorioCurso.SelecionarPorIdAsync(cursoId);

        Assert.IsNull(cursoSelecionado);
    }

    [TestMethod]
    public async Task SelecionarTodos_ComTresCursos_RetornaTodos()
    {
        Curso curso1 = new(
            Guid.CreateVersion7(),
            "Curso 1",
            "Descrição 1",
            40,
            new DateOnly(2026, 12, 20)
        );

        Curso curso2 = new(
            Guid.CreateVersion7(),
            "Curso 2",
            "Descrição 2",
            60,
            new DateOnly(2026, 12, 21)
        );

        Curso curso3 = new(
            Guid.CreateVersion7(),
            "Curso 3",
            "Descrição 3",
            80,
            new DateOnly(2026, 12, 22)
        );

        await repositorioCurso.CadastrarAsync(curso1);
        await repositorioCurso.CadastrarAsync(curso2);
        await repositorioCurso.CadastrarAsync(curso3);

        dbContext.ChangeTracker.Clear();

        List<Curso> cursosSelecionados =
            await repositorioCurso.SelecionarTodosAsync();

        Assert.HasCount(3, cursosSelecionados);
    }

    [TestMethod]
    public async Task Excluir_CursoExistente_RemoveCurso()
    {
        Curso curso = new(
            Guid.CreateVersion7(),
            "Curso de C#",
            "Curso de desenvolvimento com C# e .NET",
            40,
            new DateOnly(2026, 12, 20)
        );

        await repositorioCurso.CadastrarAsync(curso);

        dbContext.ChangeTracker.Clear();

        bool conseguiuExcluir =
            await repositorioCurso.ExcluirAsync(curso.Id);

        dbContext.ChangeTracker.Clear();

        Curso? cursoSelecionado =
            await repositorioCurso.SelecionarPorIdAsync(curso.Id);

        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(cursoSelecionado);
    }

    [TestMethod]
    public async Task ExistePorId_CursoExistente_RetornaTrue()
    {
        Curso curso = new(
            Guid.CreateVersion7(),
            "Curso de C#",
            "Curso de desenvolvimento com C# e .NET",
            40,
            new DateOnly(2026, 12, 20)
        );

        await repositorioCurso.CadastrarAsync(curso);

        dbContext.ChangeTracker.Clear();

        bool existe = await repositorioCurso.ExistePorIdAsync(
            curso.Id,
            CancellationToken.None
        );

        Assert.IsTrue(existe);
    }

    [TestMethod]
    public async Task ExistePorId_CursoInexistente_RetornaFalse()
    {
        Guid cursoId = Guid.CreateVersion7();

        bool existe = await repositorioCurso.ExistePorIdAsync(
            cursoId,
            CancellationToken.None
        );

        Assert.IsFalse(existe);
    }
}