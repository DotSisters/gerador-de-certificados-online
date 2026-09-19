namespace GeradorDeCertificados.WebApp.Modulos.Cursos;

public sealed record CadastrarCursoRequest(
    string Nome,
    string DescricaoCurso,
    int CargaHoraria,
    DateOnly DataConclusao
);

public sealed record CadastrarCursoResponse(
    Guid Id,
    string Nome
);

public sealed record CursoResponse(
    Guid Id,
    string Nome,
    string DescricaoCurso,
    int CargaHoraria,
    DateOnly DataConclusao
);