using GeradorDeCertificados.Dominio.Modulos.Certificados;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeradorDeCertificados.Infraestrutura.Compartilhado.Orm.Config;

public sealed class CertificadoConfiguration : IEntityTypeConfiguration<Certificado>
{
    public void Configure(EntityTypeBuilder<Certificado> builder)
    {
        builder.ToTable("TBCertificados");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.ProcessamentoId)
            .IsRequired();

        builder.Property(c => c.NomeAluno)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.CaminhoArquivo)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(c => c.DataGeracao)
            .IsRequired(false);

        builder.Property(c => c.Status)
            .IsRequired();
    }
}
