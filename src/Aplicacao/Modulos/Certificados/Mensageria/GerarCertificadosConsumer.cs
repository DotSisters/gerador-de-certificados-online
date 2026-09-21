using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados.Mensageria;

public sealed class GerarCertificadosConsumer(
    IRepositorioProcessamentoCertificado repositorioProcessamento,
    IRepositorioCurso repositorioCurso,
    ICertificadoPdfGenerator pdfGenerator,
    ICertificadoStorage certificadoStorage,
    ILogger<GerarCertificadosConsumer> logger
) : IConsumer<CertificadosSolicitadosMessage>
{
    public async Task Consume(ConsumeContext<CertificadosSolicitadosMessage> context)
    {
        ProcessamentoCertificado? processamento = await repositorioProcessamento.SelecionarPorIdAsync(
            context.Message.ProcessamentoId,
            context.CancellationToken
        );

        if (processamento is null)
        {
            logger.LogWarning(
                "Processamento {ProcessamentoId} não encontrado.",
                context.Message.ProcessamentoId
            );
            return;
        }

        Curso? curso = await repositorioCurso.SelecionarPorIdAsync(
            context.Message.CursoId,
            context.CancellationToken
        );

        if (curso is null)
        {
            logger.LogWarning(
                "Curso {CursoId} não encontrado para o processamento {ProcessamentoId}.",
                context.Message.CursoId,
                context.Message.ProcessamentoId
            );
            return;
        }

        foreach (Certificado certificado in processamento.Certificados
            .Where(c => c.StatusGeracao == StatusGeracao.Pendente)
            .ToList())
        {
            try
            {
                string caminho = await pdfGenerator.GerarAsync(
                    certificado,
                    curso,
                    context.CancellationToken
                );

                processamento.RegistrarSucesso(certificado.Id, caminho);
                await repositorioProcessamento.SalvarAsync(processamento, context.CancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Falha ao gerar certificado {CertificadoId} do processamento {ProcessamentoId}.",
                    certificado.Id,
                    processamento.Id
                );

                processamento.RegistrarFalha(certificado.Id);
                await repositorioProcessamento.SalvarAsync(processamento, context.CancellationToken);
            }
        }

        if (!processamento.TodosCertificadosProcessados || processamento.CaminhoZip is not null)
        {
            return;
        }

        IReadOnlyList<string> caminhosPdf = [.. processamento.Certificados
            .Where(c => c.StatusGeracao == StatusGeracao.Gerado && !string.IsNullOrWhiteSpace(c.CaminhoArquivo))
            .Select(c => c.CaminhoArquivo!)];

        string caminhoZip = await certificadoStorage.CompactarAsync(
            processamento.CursoId,
            processamento.Id,
            caminhosPdf,
            context.CancellationToken
        );

        processamento.RegistrarZip(caminhoZip);
        await repositorioProcessamento.SalvarAsync(processamento, context.CancellationToken);

        logger.LogInformation(
            "Processamento {ProcessamentoId} concluído. Gerados: {Gerados}. Falhas: {Falhas}.",
            processamento.Id,
            processamento.Gerados,
            processamento.Falhas
        );
    }
}
