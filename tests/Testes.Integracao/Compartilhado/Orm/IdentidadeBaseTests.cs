using GeradorDeCertificados.Infraestrutura.Compartilhado.Auth;
using GeradorDeCertificados.Infraestrutura.Compartilhado.Orm;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Testes.Integracao.Compartilhado.Orm;

public abstract class IdentidadeBaseTests
{
    protected GeradorDeCertificadosDbContext dbContext = null!;
    protected GerenciadorDeIdentidade gerenciadorDeIdentidade = null!;

    private ServiceProvider provedorDeServicos = null!;
    private IServiceScope escopo = null!;

    [TestInitialize]
    public void InicializarContexto()
    {
        ServiceCollection servicos = new();

        servicos.AddLogging();

        servicos.AddDbContext<GeradorDeCertificadosDbContext>(options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString())
        );

        servicos.AddIdentityCore<IdentityUser<Guid>>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<GeradorDeCertificadosDbContext>();

        servicos.AddScoped<GerenciadorDeIdentidade>();

        provedorDeServicos = servicos.BuildServiceProvider();
        escopo = provedorDeServicos.CreateScope();

        dbContext = escopo.ServiceProvider.GetRequiredService<GeradorDeCertificadosDbContext>();
        gerenciadorDeIdentidade = escopo.ServiceProvider.GetRequiredService<GerenciadorDeIdentidade>();
    }

    [TestCleanup]
    public void DescartarContexto()
    {
        escopo.Dispose();
        provedorDeServicos.Dispose();
    }
}
