namespace GeradorDeCertificados.Infraestrutura.Modulos.Certificados;

public sealed class CertificadosArquivosOptions
{
    public const string SectionName = "Certificados";

    public string DiretorioArquivos { get; set; } = "certificados-arquivos";
}
