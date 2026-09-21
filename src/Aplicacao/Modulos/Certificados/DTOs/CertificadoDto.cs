using GeradorDeCertificados.Dominio.Modulos.Certificados;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados.DTOs;

public record CertificadoDto(
    Guid Id,
    string NomeAluno,
    StatusCertificado Status,
    DateTime? DataGeracao,
    string? CaminhoArquivo
);
