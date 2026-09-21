using FluentResults;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.DTOs;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.Util;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos.Util;
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using MediatR;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados;

public record ObterArquivoZipQuery(Guid CursoId)
    : IRequest<Result<ArquivoZipDto>>;

public sealed class ObterArquivoZipQueryHandler(
    IRepositorioCurso repositorioCurso,
    IRepositorioProcessamentoCertificado repositorioProcessamento
) : IRequestHandler<ObterArquivoZipQuery, Result<ArquivoZipDto>>
{
    public async Task<Result<ArquivoZipDto>> Handle(
        ObterArquivoZipQuery query,
        CancellationToken cancellationToken)
    {
        bool cursoExiste = await repositorioCurso.ExistePorIdAsync(
            query.CursoId,
            cancellationToken
        );

        if (!cursoExiste)
        {
            return Result.Fail(ErrosCurso.NaoEncontrado(query.CursoId));
        }

        ProcessamentoCertificado? processamento =
            await repositorioProcessamento.SelecionarMaisRecentePorCursoIdAsync(
                query.CursoId,
                cancellationToken
            );

        if (processamento is null)
        {
            return Result.Fail(ErrosCertificado.ProcessamentoNaoEncontrado(query.CursoId));
        }

        if (string.IsNullOrWhiteSpace(processamento.CaminhoZip))
        {
            return Result.Fail(ErrosCertificado.ZipIndisponivel(query.CursoId));
        }

        string nomeArquivo = Path.GetFileName(processamento.CaminhoZip);

        if (string.IsNullOrWhiteSpace(nomeArquivo))
        {
            nomeArquivo = $"certificados-{query.CursoId}.zip";
        }

        return Result.Ok(new ArquivoZipDto(processamento.CaminhoZip, nomeArquivo));
    }
}
