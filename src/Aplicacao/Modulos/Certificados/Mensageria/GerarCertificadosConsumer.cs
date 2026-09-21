using MassTransit;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados.Mensageria;

public sealed class GerarCertificadosConsumer : IConsumer<CertificadosSolicitadosMessage>
{
    public Task Consume(ConsumeContext<CertificadosSolicitadosMessage> context)
    {
        // Próxima aula: carregar o processamento e o curso, chamar IGeradorPdfCertificado
        // por aluno, MarcarComoGerado/MarcarComoFalha e publicar ZipSolicitadoMessage.
        return Task.CompletedTask;
    }
}
