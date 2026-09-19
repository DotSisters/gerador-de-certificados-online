namespace GeradorDeCertificados.Aplicacao.Modulos.Cursos.DTOs;

public record CursoDto(
    Guid Id,
    string Nome,
    string DescricaoCurso,
    int CargaHoraria,
    DateOnly DataConclusao
);