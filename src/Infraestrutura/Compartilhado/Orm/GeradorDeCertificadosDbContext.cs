using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GeradorDeCertificados.Infraestrutura.Compartilhado.Orm;

public sealed class GeradorDeCertificadosDbContext(
    DbContextOptions<GeradorDeCertificadosDbContext> options
) : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<ProcessamentoCertificado> ProcessamentosCertificado => Set<ProcessamentoCertificado>();
    public DbSet<Certificado> Certificados => Set<Certificado>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GeradorDeCertificadosDbContext).Assembly);
    }
}
