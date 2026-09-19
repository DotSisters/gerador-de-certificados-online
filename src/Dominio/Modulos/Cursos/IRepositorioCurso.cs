using GeradorDeCertificados.Dominio.Compartilhado;

namespace GeradorDeCertificados.Dominio.Modulos.Cursos;

public interface IRepositorioCurso : IRepositorio<Curso>
{
    Task<bool> ExistePorIdAsync(
        Guid cursoId,
        CancellationToken cancellationToken
    );
}