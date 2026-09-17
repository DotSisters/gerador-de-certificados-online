using FluentResults;
using GeradorDeCertificados.Aplicacao.Modulos.Usuarios.Util;
using GeradorDeCertificados.Dominio.Compartilhado.Auth;
using MediatR;

namespace GeradorDeCertificados.Aplicacao.Modulos.Usuarios;

public sealed record CadastrarUsuarioCommand(
    string Email,
    string Senha
) : IRequest<Result<Guid>>;

public sealed class CadastrarUsuarioCommandHandler(
    IGerenciadorDeIdentidade gerenciadorDeIdentidade
) : IRequestHandler<CadastrarUsuarioCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
    CadastrarUsuarioCommand command,
    CancellationToken cancellationToken = default
)
    {
        var usuarioId = Guid.CreateVersion7();
        try
        {
            UsuarioDto usuario = await gerenciadorDeIdentidade.CadastrarAsync(
                usuarioId,
                command.Email,
                command.Senha
            );
            return Result.Ok(usuario.Id);
        }
        catch (ValidacaoDeIdentidadeException ex)
        {
            return Result.Fail(ErrosDeUsuario.ValidacaoDeIdentidade(ex.Campo, ex.Message));
        }
        catch (ConflitoDeIdentidadeException ex)
        {
            return Result.Fail(ErrosDeUsuario.ConflitoDeIdentidade(ex.Message));
        }
    }
}
