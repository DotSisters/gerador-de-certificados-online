using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Infraestrutura.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeCertificados.Infraestrutura.Modulos.Certificados;

public sealed class RepositorioProcessamentoCertificadoEmOrm(
    GeradorDeCertificadosDbContext dbContext
) : RepositorioBaseEmOrm<ProcessamentoCertificado>(dbContext), IRepositorioProcessamentoCertificado
{
    public override async Task<ProcessamentoCertificado?> SelecionarPorIdAsync(
        Guid idSelecionado,
        CancellationToken cancellationToken = default
    )
    {
        return await registros
            .Include(p => p.Certificados)
            .SingleOrDefaultAsync(p => p.Id == idSelecionado, cancellationToken);
    }

    public async Task<ProcessamentoCertificado?> SelecionarPorCursoIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    )
    {
        return await SelecionarMaisRecentePorCursoIdAsync(cursoId, cancellationToken);
    }

    public async Task<bool> ExisteEmAndamentoPorCursoIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    )
    {
        return await registros.AnyAsync(
            p => p.CursoId == cursoId &&
                 (p.Status == StatusProcessamento.Pendente ||
                  p.Status == StatusProcessamento.GerandoCertificados ||
                  p.Status == StatusProcessamento.GerandoZip),
            cancellationToken
        );
    }

    public async Task<ProcessamentoCertificado?> SelecionarMaisRecentePorCursoIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    )
    {
        return await registros
            .Include(p => p.Certificados)
            .Where(p => p.CursoId == cursoId)
            .OrderByDescending(p => p.DataSolicitacao)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
