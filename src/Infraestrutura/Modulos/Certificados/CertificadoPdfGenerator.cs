using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GeradorDeCertificados.Infraestrutura.Modulos.Certificados;

public sealed class CertificadoPdfGenerator : ICertificadoPdfGenerator
{
    private readonly string diretorioRaiz;

    static CertificadoPdfGenerator()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public CertificadoPdfGenerator(IOptions<CertificadosArquivosOptions> options)
    {
        diretorioRaiz = Path.GetFullPath(options.Value.DiretorioArquivos);
        Directory.CreateDirectory(diretorioRaiz);
    }

    public async Task<string> GerarAsync(
        Certificado certificado,
        Curso curso,
        CancellationToken cancellationToken
    )
    {
        string pastaCurso = Path.Combine(diretorioRaiz, curso.Id.ToString());
        Directory.CreateDirectory(pastaCurso);

        string nomeArquivo = SanitizarNomeArquivo($"{certificado.NomeAluno}-{certificado.Id}.pdf");
        string caminho = Path.Combine(pastaCurso, nomeArquivo);

        byte[] pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(50);
                page.PageColor(Colors.White);

                page.Content().Column(coluna =>
                {
                    coluna.Item().Text("Certificado de Conclusão")
                        .FontSize(28)
                        .Bold()
                        .FontColor(Colors.Blue.Darken2)
                        .AlignCenter();

                    coluna.Item().PaddingTop(40).Text($"Certificamos que {certificado.NomeAluno}")
                        .FontSize(18)
                        .AlignCenter();

                    coluna.Item().PaddingTop(12).Text($"concluiu o curso {curso.Nome}")
                        .FontSize(16)
                        .AlignCenter();

                    coluna.Item().PaddingTop(12)
                        .Text($"com carga horária de {curso.CargaHoraria} horas")
                        .FontSize(14)
                        .AlignCenter();

                    if (curso.DataConclusao is not null)
                    {
                        coluna.Item().PaddingTop(12)
                            .Text($"em {curso.DataConclusao.Value:dd/MM/yyyy}")
                            .FontSize(14)
                            .AlignCenter();
                    }
                });
            });
        }).GeneratePdf();

        await File.WriteAllBytesAsync(caminho, pdf, cancellationToken);

        return Path.GetFullPath(caminho);
    }

    private static string SanitizarNomeArquivo(string nome)
    {
        foreach (char caractere in Path.GetInvalidFileNameChars())
        {
            nome = nome.Replace(caractere, '_');
        }

        return nome;
    }
}
