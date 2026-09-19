using GeradorDeCertificados.Aplicacao.Modulos.Cursos;
using GeradorDeCertificados.WebApi.Compartilhado.Http;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GeradorDeCertificados.WebApp.Modulos.Cursos;

[ApiController]
[Route("api/cursos")]
public sealed class CursosController(
    IMediator mediator
) : ControllerBase
{
    [HttpGet("{cursoId:guid}")]
    [ProducesResponseType<CursoResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CursoResponse>> ObterPorId(
        Guid cursoId,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(
            new ObterCursoPorIdQuery(cursoId),
            cancellationToken
        );

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return Ok(new CursoResponse(
            resultado.Value.Id,
            resultado.Value.Nome,
            resultado.Value.DescricaoCurso,
            resultado.Value.CargaHoraria,
            resultado.Value.DataConclusao
        ));
    }

    [HttpPost]
    [ProducesResponseType<CadastrarCursoResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<CadastrarCursoResponse>> Cadastrar(
        CadastrarCursoRequest request,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(
            new CadastrarCursoCommand(
                request.Nome,
                request.DescricaoCurso,
                request.CargaHoraria,
                request.DataConclusao
            ),
            cancellationToken
        );

        if (!resultado.IsSuccess)
            return this.ProblemDetails(resultado);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { cursoId = resultado.Value.Id },
            new CadastrarCursoResponse(
                resultado.Value.Id,
                resultado.Value.Nome
            )
        );
    }
}