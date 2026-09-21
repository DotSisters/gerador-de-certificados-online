using GeradorDeCertificados.Dominio.Modulos.Cursos;

namespace GeradorDeCertificados.Dominio.Modulos.Certificados;

public interface ICertificadoPdfGenerator
{
    Task<string> GerarAsync(
        Certificado certificado,
        Curso curso,
        CancellationToken cancellationToken
    );
}
