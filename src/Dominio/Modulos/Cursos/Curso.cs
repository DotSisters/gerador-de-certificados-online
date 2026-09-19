using GeradorDeCertificados.Dominio.Compartilhado;

namespace GeradorDeCertificados.Dominio.Modulos.Cursos;

public sealed class Curso : EntidadeBase<Curso>
{
    public string Nome { get; private set; } = string.Empty;
    public string DescricaoCurso { get; private set; } = string.Empty;
    public int CargaHoraria { get; private set; }
    public DateOnly? DataConclusao { get; private set; }

    private Curso() { }

    public Curso(Guid id, string nome, string descricaoCurso, int cargaHoraria, DateOnly dataConclusao)
    {
        Id = id;
        Nome = nome.Trim();
        DescricaoCurso = descricaoCurso?.Trim() ?? string.Empty;
        CargaHoraria = cargaHoraria;
        DataConclusao = dataConclusao;
    }

    public override IReadOnlyList<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (Nome.Length is < 2 or > 200)
        {
            erros.Add(new ErroValidacao(nameof(Nome),
            "O nome deve possuir entre 2 e 200 caracteres."
            ));
        }

        if (DescricaoCurso.Length > 500)
        {
            erros.Add(new ErroValidacao(nameof(DescricaoCurso),
            "A descrição deve possuir no máximo 500 caracteres."
            ));
        }

        if (CargaHoraria <= 0)
        {
            erros.Add(new ErroValidacao(nameof(CargaHoraria),
            "A carga horária deve ser maior que zero."
            ));
        }

        if (DataConclusao is null)
        {
            erros.Add(new ErroValidacao(nameof(DataConclusao),
            "A data de conclusão do curso é obrigatória."
            ));
        }

        return erros;
    }
    public override void Atualizar(Curso entidadeAtualizada)
    {
        Nome = entidadeAtualizada.Nome.Trim();
        DescricaoCurso = entidadeAtualizada.DescricaoCurso;
        CargaHoraria = entidadeAtualizada.CargaHoraria;
        DataConclusao = entidadeAtualizada.DataConclusao;
    }
}