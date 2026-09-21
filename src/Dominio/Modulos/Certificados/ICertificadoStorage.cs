namespace GeradorDeCertificados.Dominio.Modulos.Certificados;

public interface ICertificadoStorage
{
    Task<string> CompactarAsync(
        Guid cursoId,
        Guid processamentoId,
        IReadOnlyList<string> caminhosPdf,
        CancellationToken cancellationToken
    );

    Stream AbrirLeitura(string caminho);
}
