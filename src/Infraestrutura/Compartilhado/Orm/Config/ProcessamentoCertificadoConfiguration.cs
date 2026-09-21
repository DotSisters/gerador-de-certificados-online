using GeradorDeCertificados.Dominio.Modulos.Certificados;
using GeradorDeCertificados.Dominio.Modulos.Cursos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeradorDeCertificados.Infraestrutura.Compartilhado.Orm.Config;

public sealed class ProcessamentoCertificadoConfiguration
    : IEntityTypeConfiguration<ProcessamentoCertificado>
{
    public void Configure(EntityTypeBuilder<ProcessamentoCertificado> builder)
    {
        builder.ToTable("TBProcessamentosCertificado");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.CursoId)
            .IsRequired();

        builder.HasOne<Curso>()
            .WithMany()
            .HasForeignKey(p => p.CursoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(p => p.Status)
            .IsRequired();

        builder.Property(p => p.CaminhoZip)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(p => p.DataSolicitacao)
            .IsRequired();

        builder.Property(p => p.ConcluidoEm)
            .IsRequired(false);

        builder.HasMany(p => p.Certificados)
            .WithOne()
            .HasForeignKey(c => c.ProcessamentoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(p => p.Certificados)
            .HasField("certificados")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(p => p.Gerados);
        builder.Ignore(p => p.Falhas);
        builder.Ignore(p => p.TodosCertificadosProcessados);
        builder.Ignore(p => p.EstaFinalizado);
        builder.Ignore(p => p.EstaEmAndamento);
    }
}
