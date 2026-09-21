using GeradorDeCertificados.Dominio.Modulos.Certificados;

namespace Testes.Unidade.Modulos.Certificados;

[TestClass]
public sealed class ProcessamentoCertificadoTests
{
    [TestMethod]
    public void RegistrarZip_QuandoTodosGerados_DefineConcluido()
    {
        ProcessamentoCertificado processamento = CriarProcessamento("Ana", "Bia");
        processamento.IniciarGeracaoCertificados();

        foreach (Certificado certificado in processamento.Certificados)
        {
            processamento.RegistrarSucesso(certificado.Id, $"/tmp/{certificado.Id}.pdf");
        }

        processamento.IniciarGeracaoZip();
        processamento.RegistrarZip("/tmp/certificados.zip");

        Assert.AreEqual(StatusProcessamento.Concluido, processamento.Status);
        Assert.AreEqual("/tmp/certificados.zip", processamento.CaminhoZip);
        Assert.IsNotNull(processamento.ConcluidoEm);
        Assert.IsTrue(processamento.EstaFinalizado);
        Assert.IsFalse(processamento.EstaEmAndamento);
        Assert.AreEqual(2, processamento.Gerados);
        Assert.AreEqual(0, processamento.Falhas);
    }

    [TestMethod]
    public void RegistrarZip_ComCertificadosPendentes_DeveFalhar()
    {
        ProcessamentoCertificado processamento = CriarProcessamento("Ana");

        Assert.Throws<InvalidOperationException>(
            () => processamento.RegistrarZip("/tmp/certificados.zip")
        );
    }

    private static ProcessamentoCertificado CriarProcessamento(params string[] alunos)
    {
        return new ProcessamentoCertificado(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            alunos
        );
    }
}
