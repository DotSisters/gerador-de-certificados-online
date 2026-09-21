using GeradorDeCertificados.Aplicacao.Modulos.Certificados;
using GeradorDeCertificados.WebApi.Compartilhado.Http;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GeradorDeCertificados.WebApp.Modulos.Certificados;

[ApiController]
[Route("api/cursos/{cursoId:guid}")]
public sealed class CertificadosController(
    IMediator mediator
) : ControllerBase
{
    [HttpPost("certificados")]
    [ProducesResponseType<SolicitarGeracaoCertificadosResponse>(StatusCodes.Status202Accepted)]
    public async Task<ActionResult<SolicitarGeracaoCertificadosResponse>> SolicitarGeracao(
        Guid cursoId,
        SolicitarGeracaoCertificadosRequest request,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(
            new SolicitarGeracaoCertificadosCommand(
                cursoId,
                request.Alunos?.Select(aluno => aluno.Nome).ToList() ?? []
            ),
            cancellationToken
        );

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return AcceptedAtAction(
            nameof(ObterStatus),
            new { cursoId },
            new SolicitarGeracaoCertificadosResponse(
                resultado.Value.Id,
                resultado.Value.Status
            )
        );
    }

    [HttpGet("status")]
    [ProducesResponseType<StatusProcessamentoResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<StatusProcessamentoResponse>> ObterStatus(
        Guid cursoId,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(
            new ObterStatusProcessamentoQuery(cursoId),
            cancellationToken
        );

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return Ok(new StatusProcessamentoResponse(
            resultado.Value.ProcessamentoId,
            resultado.Value.CursoId,
            resultado.Value.Status
        ));
    }

    [HttpGet("certificados")]
    [ProducesResponseType<IReadOnlyList<CertificadoResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CertificadoResponse>>> Listar(
        Guid cursoId,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(
            new ListarCertificadosPorCursoQuery(cursoId),
            cancellationToken
        );

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        IReadOnlyList<CertificadoResponse> certificados = [.. resultado.Value.Select(
            certificado => new CertificadoResponse(
                certificado.Id,
                certificado.NomeAluno,
                certificado.Status,
                certificado.DataGeracao
            )
        )];

        return Ok(certificados);
    }

    [HttpGet("certificados/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Download(
        Guid cursoId,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(
            new ObterArquivoZipQuery(cursoId),
            cancellationToken
        );

        if (resultado.IsFailed)
            return this.ProblemDetails(resultado);

        return PhysicalFile(
            resultado.Value.Caminho,
            "application/zip",
            resultado.Value.NomeArquivo
        );
    }
}
