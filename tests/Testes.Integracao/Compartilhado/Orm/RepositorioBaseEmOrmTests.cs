// using FizzWare.NBuilder;
// using GeradorDeCertificados.Dominio.Modulos.Certificados;
// using GeradorDeCertificados.Dominio.Modulos.Cursos;
using GeradorDeCertificados.Infraestrutura.Compartilhado.Orm;
// using GeradorDeCertificados.Infraestrutura.Modulos.Certificados;
// using GeradorDeCertificados.Infraestrutura.Modulos.Cursos;
using Microsoft.EntityFrameworkCore;
using Testes.Integracao.Compartilhado.Auth;

namespace Testes.Integracao.Compartilhado.Orm;

public abstract class RepositorioBaseEmOrmTests
{
    protected GeradorDeCertificadosDbContext dbContext = null!;
    protected Guid usuarioId;
    protected ProvedorDeUsuarioFake provedorDeUsuario = null!;

    // protected RepositorioCursoEmOrm repositorioCurso = null!;
    // protected RepositorioCertificadoEmOrm repositorioCertificado = null!;

    [TestInitialize]
    public void InicializarContexto()
    {
        usuarioId = Guid.NewGuid();
        provedorDeUsuario = new ProvedorDeUsuarioFake(usuarioId);
        dbContext = CriarDbContext();

        // Curso
        // repositorioCurso = new RepositorioCursoEmOrm(dbContext);
        //
        // BuilderSetup.SetCreatePersistenceMethod<Curso>(curso =>
        //     repositorioCurso.CadastrarAsync(curso).GetAwaiter().GetResult());
        // BuilderSetup.SetCreatePersistenceMethod<IList<Curso>>((cursos) =>
        // {
        //     foreach (Curso c in cursos)
        //         repositorioCurso.CadastrarAsync(c).GetAwaiter().GetResult();
        // });

        // Certificado
        // repositorioCertificado = new RepositorioCertificadoEmOrm(dbContext);
        //
        // BuilderSetup.SetCreatePersistenceMethod<Certificado>(certificado =>
        //     repositorioCertificado.CadastrarAsync(certificado).GetAwaiter().GetResult());
        // BuilderSetup.SetCreatePersistenceMethod<IList<Certificado>>((certificados) =>
        // {
        //     foreach (Certificado c in certificados)
        //         repositorioCertificado.CadastrarAsync(c).GetAwaiter().GetResult();
        // });
    }

    [TestCleanup]
    public void DescartarContexto()
    {
        dbContext.Dispose();
    }

    private GeradorDeCertificadosDbContext CriarDbContext()
    {
        DbContextOptions<GeradorDeCertificadosDbContext> options =
            new DbContextOptionsBuilder<GeradorDeCertificadosDbContext>()
                .UseInMemoryDatabase($"integracao-{Guid.NewGuid():N}")
                .Options;

        return new GeradorDeCertificadosDbContext(options);
    }
}
