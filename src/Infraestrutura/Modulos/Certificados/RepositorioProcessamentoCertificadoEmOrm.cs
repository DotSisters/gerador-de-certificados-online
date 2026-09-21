using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Infraestrutura.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeCertificados.Infraestrutura.Modulos.Certificados;

public sealed class RepositorioProcessamentoCertificadoEmOrm
    : RepositorioBaseEmOrm<ProcessamentoCertificado>, IRepositorioProcessamentoCertificado
{
    private readonly GeradorDeCertificadosDbContext dbContext;

    public RepositorioProcessamentoCertificadoEmOrm(GeradorDeCertificadosDbContext dbContext)
        : base(dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task SalvarAsync(
        ProcessamentoCertificado processamento,
        CancellationToken cancellationToken = default
    )
    {
        dbContext.ChangeTracker.Clear();
        dbContext.Attach(processamento);
        dbContext.Entry(processamento).State = EntityState.Modified;

        foreach (Certificado certificado in processamento.Certificados)
        {
            dbContext.Entry(certificado).State = EntityState.Modified;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        dbContext.ChangeTracker.Clear();
    }

    public override async Task<ProcessamentoCertificado?> SelecionarPorIdAsync(
        Guid idSelecionado,
        CancellationToken cancellationToken = default
    )
    {
        return await registros
            .AsNoTracking()
            .Include(p => p.Certificados)
            .SingleOrDefaultAsync(p => p.Id == idSelecionado, cancellationToken);
    }

    public async Task<ProcessamentoCertificado?> SelecionarPorCursoAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    )
    {
        return await registros
            .AsNoTracking()
            .Include(p => p.Certificados)
            .Where(p => p.CursoId == cursoId)
            .OrderByDescending(p => p.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<ProcessamentoCertificado?> SelecionarPorCursoIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    )
    {
        return SelecionarPorCursoAsync(cursoId, cancellationToken);
    }

    public async Task<bool> ExisteEmAndamentoPorCursoIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    )
    {
        return await registros.AnyAsync(
            p => p.CursoId == cursoId
                && (p.Status == StatusProcessamento.Pendente
                    || p.Status == StatusProcessamento.GerandoCertificados
                    || p.Status == StatusProcessamento.GerandoZip),
            cancellationToken
        );
    }
}
