using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Infraestrutura.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeCertificados.Infraestrutura.Modulos.Certificados;

public sealed class RepositorioCertificadoEmOrm : RepositorioBaseEmOrm<Certificado>, IRepositorioCertificado
{
    private readonly GeradorDeCertificadosDbContext contexto;

    public RepositorioCertificadoEmOrm(GeradorDeCertificadosDbContext dbContext) : base(dbContext)
    {
        contexto = dbContext;
    }

    public async Task<List<Certificado>> SelecionarPorCursoIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    )
    {
        Guid? processamentoId = await contexto.Set<ProcessamentoCertificado>()
            .Where(p => p.CursoId == cursoId)
            .OrderByDescending(p => p.DataSolicitacao)
            .Select(p => (Guid?)p.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (processamentoId is null)
        {
            return [];
        }

        return await registros
            .Where(c => c.ProcessamentoId == processamentoId)
            .ToListAsync(cancellationToken);
    }
}
