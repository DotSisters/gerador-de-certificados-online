using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using GeradorDeCertificados.Infraestrutura.Modulos.Certificados;
using Testes.Integracao.Compartilhado.Orm;

namespace Testes.Integracao.Modulos.Certificados;

[TestClass]
public sealed class RepositorioProcessamentoCertificadoTests : RepositorioBaseTests
{
    [TestMethod]
    public async Task SelecionarPorCursoAsync_DeveRetornarProcessamentoDeMaiorId()
    {
        Guid cursoId = Guid.CreateVersion7();
        await repositorioCurso.CadastrarAsync(CriarCurso(cursoId));

        RepositorioProcessamentoCertificadoEmOrm repositorio = new(dbContext);

        Guid idMenor = Guid.CreateVersion7();
        await Task.Delay(10);
        Guid idMaior = Guid.CreateVersion7();

        ProcessamentoCertificado processamentoMaiorId = new(idMaior, cursoId, ["Ana"]);
        await repositorio.CadastrarAsync(processamentoMaiorId);

        await Task.Delay(20);

        ProcessamentoCertificado processamentoMaisRecentePorData = new(idMenor, cursoId, ["Bia"]);
        await repositorio.CadastrarAsync(processamentoMaisRecentePorData);

        dbContext.ChangeTracker.Clear();

        ProcessamentoCertificado? selecionado = await repositorio.SelecionarPorCursoAsync(cursoId);

        Assert.IsNotNull(selecionado);
        Assert.AreEqual(idMaior, selecionado.Id);
        Assert.AreEqual("Ana", selecionado.Certificados.Single().NomeAluno);
    }

    private static Curso CriarCurso(Guid cursoId)
    {
        return new Curso(
            cursoId,
            "Curso de C#",
            "Curso de desenvolvimento com C# e .NET",
            40,
            new DateOnly(2026, 12, 20)
        );
    }
}
