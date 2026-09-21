using FluentResults;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.DTOs;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.Mensageria;
using GeradorDeCertificados.Aplicacao.Modulos.Certificados.Util;
using GeradorDeCertificados.Aplicacao.Modulos.Cursos.Util;
using GeradorDeCertificados.Dominio.Compartilhado;
using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using MassTransit;
using MediatR;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados;

public record SolicitarGeracaoCertificadosCommand(
    Guid CursoId,
    IReadOnlyList<string> NomesAlunos
) : IRequest<Result<ProcessamentoDto>>;

public sealed class SolicitarGeracaoCertificadosCommandHandler(
    IRepositorioCurso repositorioCurso,
    IRepositorioProcessamentoCertificado repositorioProcessamento,
    IPublishEndpoint publishEndpoint
) : IRequestHandler<SolicitarGeracaoCertificadosCommand, Result<ProcessamentoDto>>
{
    public async Task<Result<ProcessamentoDto>> Handle(
        SolicitarGeracaoCertificadosCommand request,
        CancellationToken cancellationToken)
    {
        bool cursoExiste = await repositorioCurso.ExistePorIdAsync(
            request.CursoId,
            cancellationToken
        );

        if (!cursoExiste)
        {
            return Result.Fail(ErrosCurso.NaoEncontrado(request.CursoId));
        }

        bool emAndamento = await repositorioProcessamento.ExisteEmAndamentoPorCursoIdAsync(
            request.CursoId,
            cancellationToken
        );

        if (emAndamento)
        {
            return Result.Fail(ErrosCertificado.ProcessamentoEmAndamento(request.CursoId));
        }

        ProcessamentoCertificado processamento = new(
            Guid.CreateVersion7(),
            request.CursoId,
            request.NomesAlunos
        );

        IReadOnlyList<ErroValidacao> erros = processamento.Validar();

        if (erros.Count > 0)
        {
            return Result.Fail(ErrosCertificado.Validacao(erros));
        }

        await repositorioProcessamento.CadastrarAsync(processamento, cancellationToken);

        await publishEndpoint.Publish(
            new CertificadosSolicitadosMessage(processamento.Id, request.CursoId),
            cancellationToken
        );

        return Result.Ok(MapearProcessamento(processamento));
    }

    private static ProcessamentoDto MapearProcessamento(ProcessamentoCertificado processamento)
    {
        return new ProcessamentoDto(
            processamento.Id,
            processamento.CursoId,
            processamento.Status,
            processamento.CaminhoZip,
            [.. processamento.Certificados.Select(certificado => new CertificadoDto(
                certificado.Id,
                certificado.NomeAluno,
                certificado.Status,
                certificado.DataGeracao,
                certificado.CaminhoArquivo
            ))]
        );
    }
}
