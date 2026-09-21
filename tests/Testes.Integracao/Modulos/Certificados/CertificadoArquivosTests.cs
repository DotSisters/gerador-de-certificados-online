using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using GeradorDeCertificados.Infraestrutura.Modulos.Certificados;
using Microsoft.Extensions.Options;

namespace Testes.Integracao.Modulos.Certificados;

[TestClass]
public sealed class CertificadoArquivosTests
{
    [TestMethod]
    public async Task GerarPdfECompactar_DeveCriarZipLegivel()
    {
        string diretorio = Path.Combine(Path.GetTempPath(), $"certificados-teste-{Guid.NewGuid()}");
        Directory.CreateDirectory(diretorio);

        try
        {
            IOptions<CertificadosArquivosOptions> options = Options.Create(
                new CertificadosArquivosOptions { DiretorioArquivos = diretorio }
            );

            CertificadoPdfGenerator gerador = new(options);
            FileSystemCertificadoStorage storage = new(options);

            Guid cursoId = Guid.CreateVersion7();
            Guid processamentoId = Guid.CreateVersion7();
            Curso curso = new(
                cursoId,
                "Curso de C#",
                "Curso de desenvolvimento com C# e .NET",
                40,
                new DateOnly(2026, 12, 20)
            );
            Certificado certificado = new(Guid.CreateVersion7(), processamentoId, "Ana Silva");

            string caminhoPdf = await gerador.GerarAsync(certificado, curso, CancellationToken.None);

            Assert.IsTrue(File.Exists(caminhoPdf));
            Assert.IsTrue(new FileInfo(caminhoPdf).Length > 0);

            string caminhoZip = await storage.CompactarAsync(
                cursoId,
                processamentoId,
                [caminhoPdf],
                CancellationToken.None
            );

            Assert.IsTrue(File.Exists(caminhoZip));
            Assert.AreEqual($"certificados-{processamentoId}.zip", Path.GetFileName(caminhoZip));

            await using Stream leitura = storage.AbrirLeitura(caminhoZip);
            Assert.IsTrue(leitura.Length > 0);
        }
        finally
        {
            if (Directory.Exists(diretorio))
            {
                Directory.Delete(diretorio, recursive: true);
            }
        }
    }
}
