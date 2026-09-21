using MassTransit;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados.Mensageria;

public sealed class GerarZipConsumer : IConsumer<ZipSolicitadoMessage>
{
    public Task Consume(ConsumeContext<ZipSolicitadoMessage> context)
    {
        // Próxima aula: chamar ICompactadorDeArquivosZip e MarcarComoConcluido.
        return Task.CompletedTask;
    }
}
