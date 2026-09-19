using FluentResults;
using GeradorDeCertificados.Aplicacao.Compartilhado;
using GeradorDeCertificados.Aplicacao.Modulos.Usuarios;
using GeradorDeCertificados.Dominio.Compartilhado.Auth;
using Moq;

namespace Testes.Unidade.Modulos.Usuarios;

[TestClass]
public sealed class CadastrarUsuarioCommandHandlerTests
{
    [TestMethod]
    public async Task Cadastrar_QuandoIdentidadeCriaUsuario_RetornaSucessoComId()
    {
        Mock<IGerenciadorDeIdentidade> gerenciadorDeIdentidade = new();

        UsuarioDto usuarioCadastrado = new(Guid.NewGuid(), "usuario@teste.com");

        gerenciadorDeIdentidade
            .Setup(g => g.CadastrarAsync(
                It.IsAny<Guid>(),
                usuarioCadastrado.Email,
                "Senha@123"
            ))
            .ReturnsAsync(usuarioCadastrado);

        CadastrarUsuarioCommandHandler handler = new(gerenciadorDeIdentidade.Object);

        Result<Guid> resultado = await handler.Handle(
            new CadastrarUsuarioCommand(usuarioCadastrado.Email, "Senha@123")
        );

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(usuarioCadastrado.Id, resultado.Value);

        gerenciadorDeIdentidade.Verify(
            g => g.CadastrarAsync(
                It.IsAny<Guid>(),
                usuarioCadastrado.Email,
                "Senha@123"
            ),
            Times.Once
        );
    }

    [TestMethod]
    public async Task Cadastrar_QuandoIdentidadeLancaValidacao_RetornaErroDeValidacao()
    {
        Mock<IGerenciadorDeIdentidade> gerenciadorDeIdentidade = new();

        gerenciadorDeIdentidade
            .Setup(g => g.CadastrarAsync(
                It.IsAny<Guid>(),
                "usuario@teste.com",
                "Senha@123"
            ))
            .ThrowsAsync(new ValidacaoDeIdentidadeException(
                "Email",
                "E-mail em formato inválido."
            ));

        CadastrarUsuarioCommandHandler handler = new(gerenciadorDeIdentidade.Object);

        Result<Guid> resultado = await handler.Handle(
            new CadastrarUsuarioCommand("usuario@teste.com", "Senha@123")
        );

        Assert.IsTrue(resultado.IsFailed);

        IError erro = resultado.Errors.Single();

        Assert.AreEqual(TipoErro.Validacao, erro.Metadata[nameof(TipoErro)]);
        Assert.AreNotEqual(TipoErro.Conflito, erro.Metadata[nameof(TipoErro)]);
        Assert.AreEqual("Email", erro.Metadata["Campo"]);
        Assert.AreEqual("E-mail em formato inválido.", erro.Message);
    }

    [TestMethod]
    public async Task Cadastrar_QuandoIdentidadeLancaConflito_RetornaErroDeConflito()
    {
        Mock<IGerenciadorDeIdentidade> gerenciadorDeIdentidade = new();

        gerenciadorDeIdentidade
            .Setup(g => g.CadastrarAsync(
                It.IsAny<Guid>(),
                "usuario@teste.com",
                "Senha@123"
            ))
            .ThrowsAsync(new ConflitoDeIdentidadeException(
                "Já existe um usuário cadastrado com este email."
            ));

        CadastrarUsuarioCommandHandler handler = new(gerenciadorDeIdentidade.Object);

        Result<Guid> resultado = await handler.Handle(
            new CadastrarUsuarioCommand("usuario@teste.com", "Senha@123")
        );

        Assert.IsTrue(resultado.IsFailed);

        IError erro = resultado.Errors.Single();

        Assert.AreEqual(TipoErro.Conflito, erro.Metadata[nameof(TipoErro)]);
        Assert.AreEqual("Já existe um usuário cadastrado com este email.", erro.Message);
    }
}
