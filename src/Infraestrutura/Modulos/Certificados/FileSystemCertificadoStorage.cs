using System.IO.Compression;
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using Microsoft.Extensions.Options;

namespace GeradorDeCertificados.Infraestrutura.Modulos.Certificados;

public sealed class FileSystemCertificadoStorage : ICertificadoStorage
{
    private readonly string diretorioRaiz;

    public FileSystemCertificadoStorage(IOptions<CertificadosArquivosOptions> options)
    {
        diretorioRaiz = Path.GetFullPath(options.Value.DiretorioArquivos);
        Directory.CreateDirectory(diretorioRaiz);
    }

    public async Task<string> CompactarAsync(
        Guid cursoId,
        Guid processamentoId,
        IReadOnlyList<string> caminhosPdf,
        CancellationToken cancellationToken
    )
    {
        string pastaCurso = ObterPastaCurso(cursoId);
        Directory.CreateDirectory(pastaCurso);

        using MemoryStream memoryStream = new();
        using (ZipArchive zip = new(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (string caminhoPdf in caminhosPdf)
            {
                string nomeEntrada = Path.GetFileName(caminhoPdf);
                zip.CreateEntryFromFile(caminhoPdf, nomeEntrada);
            }
        }

        string caminhoZip = Path.Combine(pastaCurso, $"certificados-{processamentoId}.zip");
        await SalvarBytesAsync(caminhoZip, memoryStream.ToArray(), cancellationToken);

        return Path.GetFullPath(caminhoZip);
    }

    public Stream AbrirLeitura(string caminho)
    {
        return File.OpenRead(caminho);
    }

    private string ObterPastaCurso(Guid cursoId)
    {
        return Path.Combine(diretorioRaiz, cursoId.ToString());
    }

    private static Task SalvarBytesAsync(
        string caminho,
        byte[] bytes,
        CancellationToken cancellationToken
    )
    {
        return File.WriteAllBytesAsync(caminho, bytes, cancellationToken);
    }
}
