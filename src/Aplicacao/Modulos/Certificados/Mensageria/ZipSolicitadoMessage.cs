namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados.Mensageria;

public sealed record ZipSolicitadoMessage(
    Guid ProcessamentoId,
    Guid CursoId
);
