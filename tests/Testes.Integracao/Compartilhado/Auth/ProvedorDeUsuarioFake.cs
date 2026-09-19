using GeradorDeCertificados.Dominio.Compartilhado.Auth;

namespace Testes.Integracao.Compartilhado.Auth;

public sealed class ProvedorDeUsuarioFake(
    Guid userId,
    string? email = "usuario@teste.com"
) : IProvedorDeUsuario
{
    public Guid? Id => userId;
    public string? Email => email;
    public bool EstaAutenticado => true;
}
