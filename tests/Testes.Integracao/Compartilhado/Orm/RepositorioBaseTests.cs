using GeradorDeCertificados.Infraestrutura.Compartilhado.Orm;
using GeradorDeCertificados.Infraestrutura.Modulos.Cursos;
using Microsoft.EntityFrameworkCore;

namespace Testes.Integracao.Compartilhado.Orm;

public abstract class RepositorioBaseTests
{
    protected GeradorDeCertificadosDbContext dbContext = null!;
    protected RepositorioCursoEmOrm repositorioCurso = null!;

    [TestInitialize]
    public void InicializarContexto()
    {
        dbContext = CriarDbContext();

        repositorioCurso = new RepositorioCursoEmOrm(dbContext);
    }

    [TestCleanup]
    public void DescartarContexto()
    {
        dbContext.Dispose();
    }

    private static GeradorDeCertificadosDbContext CriarDbContext()
    {
        DbContextOptions<GeradorDeCertificadosDbContext> options =
            new DbContextOptionsBuilder<GeradorDeCertificadosDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        return new GeradorDeCertificadosDbContext(options);
    }
}