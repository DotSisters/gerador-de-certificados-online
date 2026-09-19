using FluentResults;
using MediatR;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos.DTOs;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos.Util;
using GeradorDeCertificados.Dominio.Modulos.Cursos;

namespace GeradorDeCertificados.Aplicacao.Modulos.Cursos;

public record ObterCursoPorIdQuery(Guid CursoId)
    : IRequest<Result<CursoDto>>;

public sealed class ObterCursoPorIdQueryHandler(
    IRepositorioCurso repositorioCurso
) : IRequestHandler<ObterCursoPorIdQuery, Result<CursoDto>>
{
    public async Task<Result<CursoDto>> Handle(
        ObterCursoPorIdQuery query,
        CancellationToken cancellationToken)
    {
        Curso? curso = await repositorioCurso.SelecionarPorIdAsync(
            query.CursoId,
            cancellationToken
        );

        if (curso is null)
        {
            return Result.Fail(
                ErrosCurso.NaoEncontrado(query.CursoId)
            );
        }

        CursoDto dto = new(
            curso.Id,
            curso.Nome,
            curso.DescricaoCurso,
            curso.CargaHoraria,
            curso.DataConclusao!.Value
        );

        return Result.Ok(dto);
    }
}