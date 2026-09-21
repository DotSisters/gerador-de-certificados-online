using FluentResults;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.DTOs;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.Util;
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using MediatR;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados;

public record ObterArquivoCertificadosQuery(Guid CursoId)
    : IRequest<Result<ArquivoCertificadosDto>>;

public sealed class ObterArquivoCertificadosQueryHandler(
    IRepositorioProcessamentoCertificado repositorioProcessamento,
    ICertificadoStorage certificadoStorage
) : IRequestHandler<ObterArquivoCertificadosQuery, Result<ArquivoCertificadosDto>>
{
    public async Task<Result<ArquivoCertificadosDto>> Handle(
        ObterArquivoCertificadosQuery query,
        CancellationToken cancellationToken)
    {
        ProcessamentoCertificado? processamento =
            await repositorioProcessamento.SelecionarPorCursoAsync(
                query.CursoId,
                cancellationToken
            );

        if (processamento is null)
        {
            return Result.Fail(ErrosCertificado.ProcessamentoNaoEncontrado(query.CursoId));
        }

        if (!processamento.EstaFinalizado || processamento.CaminhoZip is null)
        {
            return Result.Fail(ErrosCertificado.ZipIndisponivel(query.CursoId));
        }

        return Result.Ok(new ArquivoCertificadosDto(
            certificadoStorage.AbrirLeitura(processamento.CaminhoZip),
            $"certificados-{query.CursoId}.zip",
            "application/zip"
        ));
    }
}
