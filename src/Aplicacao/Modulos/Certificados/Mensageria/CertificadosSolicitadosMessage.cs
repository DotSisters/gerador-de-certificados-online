namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados.Mensageria;

public sealed record CertificadosSolicitadosMessage(
    Guid ProcessamentoId,
    Guid CursoId
);
