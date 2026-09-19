using GeradorDeCertificados.Infraestrutura.Compartilhado.Orm;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeCertificados.Infraestrutura.Modulos.Cursos;

public sealed class RepositorioCursoEmOrm(
    GeradorDeCertificadosDbContext dbContext
) : RepositorioBaseEmOrm<Curso>(dbContext), IRepositorioCurso
{
    public async Task<bool> ExistePorIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken
    )
    {
        return await registros.AnyAsync(
            r => r.Id == cursoId,
            cancellationToken
        );
    }
}