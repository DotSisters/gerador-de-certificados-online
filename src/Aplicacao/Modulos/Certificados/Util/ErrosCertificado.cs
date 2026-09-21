using FluentResults;
using GeradorDeCertificados.Aplicacao.Compartilhado;
using GeradorDeCertificados.Dominio.Compartilhado;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados.Util;

public static class ErrosCertificado
{
    public static IEnumerable<Error> Validacao(IEnumerable<ErroValidacao> erros)
    {
        return erros.Select(erro => new Error(erro.Mensagem)
            .WithMetadata(nameof(TipoErro), TipoErro.Validacao)
            .WithMetadata("Campo", erro.Campo));
    }

    public static Error ProcessamentoEmAndamento(Guid cursoId)
    {
        return new Error("Já existe um processamento de certificados em andamento para este curso.")
            .WithMetadata(nameof(TipoErro), TipoErro.Conflito)
            .WithMetadata("IdCurso", cursoId);
    }

    public static Error ProcessamentoNaoEncontrado(Guid cursoId)
    {
        return new Error("Não há processamento de certificados para este curso.")
            .WithMetadata(nameof(TipoErro), TipoErro.NaoEncontrado)
            .WithMetadata("IdCurso", cursoId);
    }

    public static Error ZipIndisponivel(Guid cursoId)
    {
        return new Error("O arquivo ZIP dos certificados ainda não está disponível.")
            .WithMetadata(nameof(TipoErro), TipoErro.Conflito)
            .WithMetadata("IdCurso", cursoId);
    }
}
