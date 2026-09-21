using FluentResults;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.DTOs;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.Util;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos.Util;
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using MediatR;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados;

public record ObterStatusProcessamentoQuery(Guid CursoId)
    : IRequest<Result<StatusProcessamentoDto>>;

public sealed class ObterStatusProcessamentoQueryHandler(
    IRepositorioCurso repositorioCurso,
    IRepositorioProcessamentoCertificado repositorioProcessamento
) : IRequestHandler<ObterStatusProcessamentoQuery, Result<StatusProcessamentoDto>>
{
    public async Task<Result<StatusProcessamentoDto>> Handle(
        ObterStatusProcessamentoQuery query,
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
            await repositorioProcessamento.SelecionarPorCursoAsync(
                query.CursoId,
                cancellationToken
            );

        if (processamento is null)
        {
            return Result.Fail(ErrosCertificado.ProcessamentoNaoEncontrado(query.CursoId));
        }

        return Result.Ok(new StatusProcessamentoDto(
            processamento.Id,
            processamento.CursoId,
            processamento.Status
        ));
    }
}
