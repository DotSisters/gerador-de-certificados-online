using GeradorDeCertificados.Aplicacao.Modulos.Usuarios;
using GeradorDeCertificados.WebApi.Compartilhado.Http;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeradorDeCertificados.WebApi.Modulos.Usuarios;

[ApiController]
[Route("auth")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("cadastro")]
    [ProducesResponseType<CadastrarUsuarioResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<CadastrarUsuarioResponse>> Cadastrar(
    CadastrarUsuarioRequest request,
    CancellationToken cancellationToken
)
    {
        var resultado = await mediator.Send(new CadastrarUsuarioCommand(
                    request.Email,
                    request.Senha
                ), cancellationToken);

        if (!resultado.IsSuccess)
            return this.ProblemDetails(resultado);

        return Created(
            (string?)null,
            new CadastrarUsuarioResponse(resultado.Value)
        );
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<AutenticarUsuarioResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<AutenticarUsuarioResponse>> Autenticar(
        AutenticarUsuarioRequest request,
        CancellationToken cancellationToken
    )
    {
        var resultado = await mediator.Send(new AutenticarUsuarioCommand(
            request.Email,
            request.Senha
        ), cancellationToken);

        if (!resultado.IsSuccess)
            return this.ProblemDetails(resultado);

        var accessTokenDoUsuario = resultado.Value;

        return Ok(new AutenticarUsuarioResponse(
            accessTokenDoUsuario.UsuarioId,
            accessTokenDoUsuario.Token,
            accessTokenDoUsuario.DataExpiracaoEmUtc
        ));
    }
}
