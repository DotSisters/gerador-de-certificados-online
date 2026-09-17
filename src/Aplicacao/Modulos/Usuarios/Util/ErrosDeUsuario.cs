using FluentResults;
using GeradorDeCertificados.Aplicacao.Compartilhado;

namespace GeradorDeCertificados.Aplicacao.Modulos.Usuarios.Util;

public static class ErrosDeUsuario
{
    public static Error CredenciaisInvalidas()
    {
        return new Error("O endereço de email ou senha informados são inválidos.")
            .WithMetadata(nameof(TipoErro), TipoErro.Validacao)
            .WithMetadata("Campo", "Credenciais");
    }

    public static Error ConflitoDeIdentidade(string mensagem)
    {
        return new Error(mensagem)
            .WithMetadata(nameof(TipoErro), TipoErro.Conflito);
    }

    public static Error ValidacaoDeIdentidade(string campo, string mensagem)
    {
        return new Error(mensagem)
            .WithMetadata(nameof(TipoErro), TipoErro.Validacao)
            .WithMetadata("Campo", campo);
    }
}
