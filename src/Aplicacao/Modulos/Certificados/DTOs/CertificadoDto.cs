using GeradorDeCertificados.Dominio.Modulos.Certificados;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados.DTOs;

public record CertificadoDto(
    Guid Id,
    string NomeAluno,
    StatusGeracao Status,
    DateTime? GeradoEm,
    string? CaminhoArquivo
);
