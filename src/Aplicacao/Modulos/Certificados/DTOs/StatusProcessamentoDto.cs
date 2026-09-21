using GeradorDeCertificados.Dominio.Modulos.Certificados;

namespace GeradorDeCertificados.Aplicacao.Modulos.Certificados.DTOs;

public record StatusProcessamentoDto(
    Guid ProcessamentoId,
    Guid CursoId,
    StatusProcessamento Status
);
