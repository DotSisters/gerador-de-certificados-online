using GeradorDeCertificados.Dominio.Compartilhado.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Testes.Integracao.Compartilhado.Orm;

namespace Testes.Integracao.Modulos.Usuarios;

[TestClass]
public class GerenciadorDeIdentidadeTests : IdentidadeBaseTests
{
    private const string EmailValido = "usuario@teste.com";
    private const string SenhaValida = "Senha@123";

    [TestMethod]
    public async Task Cadastrar_ComEmailESenhaValidos_PersisteUsuario()
    {
        Guid usuarioId = Guid.CreateVersion7();

        UsuarioDto cadastrado = await gerenciadorDeIdentidade.CadastrarAsync(
            usuarioId,
            EmailValido,
            SenhaValida
        );

        dbContext.ChangeTracker.Clear();

        IdentityUser<Guid>? persistido = await dbContext.Users.SingleOrDefaultAsync(
            u => u.Id == usuarioId
        );

        Assert.IsNotNull(persistido);
        Assert.AreEqual(cadastrado.Id, persistido.Id);
        Assert.AreEqual(EmailValido, persistido.Email);
        Assert.AreEqual(EmailValido, persistido.UserName);
    }

    [TestMethod]
    public async Task Cadastrar_ComSenhaDeExatamente8CaracteresComDigitoENaoAlfanumerico_PersisteUsuario()
    {
        Guid usuarioId = Guid.CreateVersion7();
        const string senhaNoLimiteMinimo = "Senha1@a";

        UsuarioDto cadastrado = await gerenciadorDeIdentidade.CadastrarAsync(
            usuarioId,
            EmailValido,
            senhaNoLimiteMinimo
        );

        dbContext.ChangeTracker.Clear();

        IdentityUser<Guid>? persistido = await dbContext.Users.SingleOrDefaultAsync(
            u => u.Id == usuarioId
        );

        Assert.IsNotNull(persistido);
        Assert.AreEqual(cadastrado.Id, persistido.Id);
        Assert.AreEqual(EmailValido, persistido.Email);
    }

    [TestMethod]
    public async Task Cadastrar_NaoArmazenaSenhaEmTextoPuro()
    {
        Guid usuarioId = Guid.CreateVersion7();

        await gerenciadorDeIdentidade.CadastrarAsync(usuarioId, EmailValido, SenhaValida);

        dbContext.ChangeTracker.Clear();

        IdentityUser<Guid> persistido = await dbContext.Users.SingleAsync(u => u.Id == usuarioId);

        Assert.IsFalse(string.IsNullOrWhiteSpace(persistido.PasswordHash));
        Assert.AreNotEqual(SenhaValida, persistido.PasswordHash);
        Assert.AreNotEqual(SenhaValida, persistido.Email);
        Assert.AreNotEqual(SenhaValida, persistido.UserName);
    }

    [TestMethod]
    public async Task Cadastrar_ComEmailInvalido_LancaValidacaoDeIdentidade()
    {
        ValidacaoDeIdentidadeException excecao = await Assert.ThrowsExactlyAsync<ValidacaoDeIdentidadeException>(
            () => gerenciadorDeIdentidade.CadastrarAsync(
                Guid.CreateVersion7(),
                "email-invalido",
                SenhaValida
            )
        );

        Assert.AreEqual("Email", excecao.Campo);

        dbContext.ChangeTracker.Clear();

        Assert.IsFalse(await dbContext.Users.AnyAsync());
    }

    [TestMethod]
    public async Task Cadastrar_ComEmailDuplicado_LancaConflitoDeIdentidade()
    {
        await gerenciadorDeIdentidade.CadastrarAsync(
            Guid.CreateVersion7(),
            EmailValido,
            SenhaValida
        );

        await Assert.ThrowsExactlyAsync<ConflitoDeIdentidadeException>(
            () => gerenciadorDeIdentidade.CadastrarAsync(
                Guid.CreateVersion7(),
                EmailValido,
                SenhaValida
            )
        );

        dbContext.ChangeTracker.Clear();

        Assert.AreEqual(1, await dbContext.Users.CountAsync());
    }

    [TestMethod]
    public async Task Cadastrar_ComSenhaComMenosDe8Caracteres_LancaValidacaoDeIdentidade()
    {
        ValidacaoDeIdentidadeException excecao = await Assert.ThrowsExactlyAsync<ValidacaoDeIdentidadeException>(
            () => gerenciadorDeIdentidade.CadastrarAsync(
                Guid.CreateVersion7(),
                EmailValido,
                "Senha@1"
            )
        );

        Assert.AreEqual("Senha", excecao.Campo);

        dbContext.ChangeTracker.Clear();

        Assert.IsFalse(await dbContext.Users.AnyAsync());
    }

    [TestMethod]
    public async Task Cadastrar_ComSenhaSemDigito_LancaValidacaoDeIdentidade()
    {
        ValidacaoDeIdentidadeException excecao = await Assert.ThrowsExactlyAsync<ValidacaoDeIdentidadeException>(
            () => gerenciadorDeIdentidade.CadastrarAsync(
                Guid.CreateVersion7(),
                EmailValido,
                "Senha@abc"
            )
        );

        Assert.AreEqual("Senha", excecao.Campo);

        dbContext.ChangeTracker.Clear();

        Assert.IsFalse(await dbContext.Users.AnyAsync());
    }

    [TestMethod]
    public async Task Cadastrar_ComSenhaSemCaractereNaoAlfanumerico_LancaValidacaoDeIdentidade()
    {
        ValidacaoDeIdentidadeException excecao = await Assert.ThrowsExactlyAsync<ValidacaoDeIdentidadeException>(
            () => gerenciadorDeIdentidade.CadastrarAsync(
                Guid.CreateVersion7(),
                EmailValido,
                "Senha1234"
            )
        );

        Assert.AreEqual("Senha", excecao.Campo);

        dbContext.ChangeTracker.Clear();

        Assert.IsFalse(await dbContext.Users.AnyAsync());
    }

    [TestMethod]
    public async Task ChecarValidadeDeSenha_ComCredenciaisValidas_RetornaUsuario()
    {
        Guid usuarioId = Guid.CreateVersion7();

        await gerenciadorDeIdentidade.CadastrarAsync(usuarioId, EmailValido, SenhaValida);

        UsuarioDto? usuario = await gerenciadorDeIdentidade.ChecarValidadeDeSenhaAsync(
            EmailValido,
            SenhaValida
        );

        Assert.IsNotNull(usuario);
        Assert.AreEqual(usuarioId, usuario.Id);
        Assert.AreEqual(EmailValido, usuario.Email);
    }

    [TestMethod]
    public async Task ChecarValidadeDeSenha_ComEmailInexistente_RetornaNulo()
    {
        UsuarioDto? usuario = await gerenciadorDeIdentidade.ChecarValidadeDeSenhaAsync(
            EmailValido,
            SenhaValida
        );

        Assert.IsNull(usuario);
    }

    [TestMethod]
    public async Task ChecarValidadeDeSenha_ComSenhaIncorreta_RetornaNulo()
    {
        await gerenciadorDeIdentidade.CadastrarAsync(
            Guid.CreateVersion7(),
            EmailValido,
            SenhaValida
        );

        UsuarioDto? usuario = await gerenciadorDeIdentidade.ChecarValidadeDeSenhaAsync(
            EmailValido,
            "Senha@errada!"
        );

        Assert.IsNull(usuario);
    }
}
