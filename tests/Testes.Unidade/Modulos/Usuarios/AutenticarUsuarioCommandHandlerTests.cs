using FluentResults;
using GeradorDeCertificados.Aplicacao.Compartilhado;
using GeradorDeCertificados.Aplicacao.Modulos.Usuarios;
using GeradorDeCertificados.Dominio.Compartilhado.Auth;
using Moq;

namespace Testes.Unidade.Modulos.Usuarios;

[TestClass]
public sealed class AutenticarUsuarioCommandHandlerTests
{
    [TestMethod]
    public async Task Autenticar_ComCredenciaisValidas_RetornaAccessTokenComExpiracao()
    {
        Mock<IGerenciadorDeIdentidade> gerenciadorDeIdentidade = new();
        Mock<IEmissorDeTokens> emissorDeTokens = new();

        UsuarioDto usuario = new(Guid.NewGuid(), "usuario@teste.com");
        AccessToken accessToken = new("token-de-acesso", DateTime.UtcNow.AddHours(1));

        gerenciadorDeIdentidade
            .Setup(g => g.ChecarValidadeDeSenhaAsync(usuario.Email, "Senha@123"))
            .ReturnsAsync(usuario);

        emissorDeTokens
            .Setup(e => e.CriarToken(usuario.Id, usuario.Email))
            .Returns(accessToken);

        AutenticarUsuarioCommandHandler handler = new(
            gerenciadorDeIdentidade.Object,
            emissorDeTokens.Object
        );

        Result<AccessTokenDoUsuarioDto> resultado = await handler.Handle(
            new AutenticarUsuarioCommand(usuario.Email, "Senha@123")
        );

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(usuario.Id, resultado.Value.UsuarioId);
        Assert.AreEqual(accessToken.Token, resultado.Value.Token);
        Assert.AreEqual(accessToken.DataExpiracaoEmUtc, resultado.Value.DataExpiracaoEmUtc);

        emissorDeTokens.Verify(e => e.CriarToken(usuario.Id, usuario.Email), Times.Once);
    }

    [TestMethod]
    public async Task Autenticar_QuandoCredenciaisInvalidas_RetornaErroDeValidacao()
    {
        Mock<IGerenciadorDeIdentidade> gerenciadorDeIdentidade = new();
        Mock<IEmissorDeTokens> emissorDeTokens = new();

        gerenciadorDeIdentidade
            .Setup(g => g.ChecarValidadeDeSenhaAsync("usuario@teste.com", "Senha@123"))
            .ReturnsAsync((UsuarioDto?)null);

        AutenticarUsuarioCommandHandler handler = new(
            gerenciadorDeIdentidade.Object,
            emissorDeTokens.Object
        );

        Result<AccessTokenDoUsuarioDto> resultado = await handler.Handle(
            new AutenticarUsuarioCommand("usuario@teste.com", "Senha@123")
        );

        Assert.IsTrue(resultado.IsFailed);

        IError erro = resultado.Errors.Single();

        Assert.AreEqual(TipoErro.Validacao, erro.Metadata[nameof(TipoErro)]);
        Assert.AreEqual("Credenciais", erro.Metadata["Campo"]);
        Assert.AreEqual(
            "O endereço de email ou senha informados são inválidos.",
            erro.Message
        );

        emissorDeTokens.Verify(
            e => e.CriarToken(It.IsAny<Guid>(), It.IsAny<string>()),
            Times.Never
        );
    }
}
