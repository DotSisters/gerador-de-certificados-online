using GeradorDeCertificados.Dominio.Compartilhado;

namespace GeradorDeCertificados.Dominio.Modulos.Certificados;

public interface IRepositorioProcessamentoCertificado : IRepositorio<ProcessamentoCertificado>
{
    Task<ProcessamentoCertificado?> SelecionarPorCursoIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    );

    Task<bool> ExisteEmAndamentoPorCursoIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    );

    Task<ProcessamentoCertificado?> SelecionarMaisRecentePorCursoIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken = default
    );
}
