namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados.DTOs;

public record ArquivoCertificadosDto(
    Stream Conteudo,
    string Name,
    string ContentType
);
