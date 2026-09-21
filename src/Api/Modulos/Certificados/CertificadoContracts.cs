using GeradorDeCertificados.Dominio.Modulos.Certificados;

namespace GeradorDeCertificados.WebApp.Modulos.Certificados;

public sealed record SolicitarGeracaoCertificadosRequest(
    IReadOnlyList<AlunoRequest> Alunos
);

public sealed record AlunoRequest(string Nome);

public sealed record SolicitarGeracaoCertificadosResponse(
    Guid ProcessamentoId,
    StatusProcessamento Status
);

public sealed record StatusProcessamentoResponse(
    Guid ProcessamentoId,
    Guid CursoId,
    StatusProcessamento Status
);

public sealed record CertificadoResponse(
    Guid Id,
    string NomeAluno,
    StatusGeracao Status,
    DateTime? GeradoEm
);
