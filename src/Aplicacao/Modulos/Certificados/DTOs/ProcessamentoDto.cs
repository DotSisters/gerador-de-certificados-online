using GeradorDeCertificados.Dominio.Modulos.Certificados;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados.DTOs;

public record ProcessamentoDto(
    Guid Id,
    Guid CursoId,
    StatusProcessamento Status,
    string? CaminhoZip,
    IReadOnlyList<CertificadoDto> Certificados
);
