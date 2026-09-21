namespace GeradorDeCertificados.Dominio.Modulos.Certificados;

public interface ICompactadorDeArquivosZip
{
    Task<string> CompactarAsync(
        IReadOnlyList<Certificado> certificados,
        Guid processamentoId,
        CancellationToken cancellationToken
    );
}
