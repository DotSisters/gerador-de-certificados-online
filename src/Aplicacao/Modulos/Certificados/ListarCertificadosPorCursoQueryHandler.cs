using FluentResults;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.DTOs;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos.Util;
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using MediatR;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados;

public record ListarCertificadosPorCursoQuery(Guid CursoId)
    : IRequest<Result<IReadOnlyList<CertificadoDto>>>;

public sealed class ListarCertificadosPorCursoQueryHandler(
    IRepositorioCurso repositorioCurso,
    IRepositorioProcessamentoCertificado repositorioProcessamento
) : IRequestHandler<ListarCertificadosPorCursoQuery, Result<IReadOnlyList<CertificadoDto>>>
{
    public async Task<Result<IReadOnlyList<CertificadoDto>>> Handle(
        ListarCertificadosPorCursoQuery query,
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
            await repositorioProcessamento.SelecionarPorCursoIdAsync(
                query.CursoId,
                cancellationToken
            );

        if (processamento is null)
        {
            return Result.Ok<IReadOnlyList<CertificadoDto>>([]);
        }

        IReadOnlyList<CertificadoDto> certificados = [.. processamento.Certificados.Select(
            certificado => new CertificadoDto(
                certificado.Id,
                certificado.NomeAluno,
                certificado.Status,
                certificado.DataGeracao,
                certificado.CaminhoArquivo
            )
        )];

        return Result.Ok(certificados);
    }
}
