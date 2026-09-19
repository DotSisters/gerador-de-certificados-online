using FluentResults;
using MediatR;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos.DTOs;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos.Util;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using GeradorDeCertificados.Dominio.Compartilhado;

namespace GeradorDeCertificados.Aplicacao.Modulos.Cursos;

public record CadastrarCursoCommand(
    string Nome,
    string DescricaoCurso,
    int CargaHoraria,
    DateOnly DataConclusao
) : IRequest<Result<CursoDto>>;

public sealed class CadastrarCursoCommandHandler(
    IRepositorioCurso repositorioCurso
) : IRequestHandler<CadastrarCursoCommand, Result<CursoDto>>
{
    public async Task<Result<CursoDto>> Handle(
        CadastrarCursoCommand request,
        CancellationToken cancellationToken)
    {
        Curso curso = new(
            Guid.CreateVersion7(),
            request.Nome,
            request.DescricaoCurso,
            request.CargaHoraria,
            request.DataConclusao
        );

        IReadOnlyList<ErroValidacao> erros = curso.Validar();

        if (erros.Count > 0)
        {
            return Result.Fail(ErrosCurso.Validacao(erros));
        }

        await repositorioCurso.CadastrarAsync(curso, cancellationToken);

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