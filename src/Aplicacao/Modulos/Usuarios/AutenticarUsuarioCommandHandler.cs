using FluentResults;
using GeradorDeCertificados.Aplicacao.Modulos.Usuarios.Util;
using GeradorDeCertificados.Dominio.Compartilhado.Auth;
using MediatR;

namespace GeradorDeCertificados.Aplicacao.Modulos.Usuarios;

public sealed record AutenticarUsuarioCommand(
    string Email,
    string Senha
) : IRequest<Result<AccessTokenDoUsuarioDto>>;

public sealed record AccessTokenDoUsuarioDto(
    Guid UsuarioId,
    string Token,
    DateTime DataExpiracaoEmUtc
);

public sealed class AutenticarUsuarioCommandHandler(
    IGerenciadorDeIdentidade gerenciadorDeIdentidade,
    IEmissorDeTokens emissorDeTokens
) : IRequestHandler<AutenticarUsuarioCommand, Result<AccessTokenDoUsuarioDto>>
{
    public async Task<Result<AccessTokenDoUsuarioDto>> Handle(
        AutenticarUsuarioCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var usuario = await gerenciadorDeIdentidade.ChecarValidadeDeSenhaAsync(
            request.Email,
            request.Senha
        );

        if (usuario is null)
            return Result.Fail(ErrosDeUsuario.CredenciaisInvalidas());

        var accessToken = emissorDeTokens.CriarToken(
            usuario.Id,
            usuario.Email
        );

        return Result.Ok(new AccessTokenDoUsuarioDto(
            usuario.Id,
            accessToken.Token,
            accessToken.DataExpiracaoEmUtc
        ));
    }
}

