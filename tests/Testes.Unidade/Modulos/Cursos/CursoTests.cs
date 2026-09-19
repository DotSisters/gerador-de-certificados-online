using GeradorDeCertificados.Dominio.Compartilhado;
using GeradorDeCertificados.Dominio.Modulos.Cursos;

namespace Testes.Unidade.Modulos.Cursos;

[TestClass]
public sealed class CursoTests
{
    [TestMethod]
    public void Validar_Todos_DadosValidos()
    {
        Curso curso = new Curso(
            Guid.CreateVersion7(),
            "Curso de C#",
            "Curso de desenvolvimento com C# e .NET",
            40,
            new DateOnly(2026, 12, 20)
        );

        IReadOnlyList<ErroValidacao> erros = curso.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Validar_NomeVazio_DeveRetornarErro()
    {
        Curso curso = new Curso(
            Guid.CreateVersion7(),
            string.Empty,
            "Curso de desenvolvimento com C# e .NET",
            40,
            new DateOnly(2026, 12, 20)
        );

        IReadOnlyList<ErroValidacao> erros = curso.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O nome deve possuir entre 2 e 200 caracteres.",
            erros.First().Mensagem
        );
    }

    [TestMethod]
    public void Validar_NomeComDoisCaracteres_DeveSerValido()
    {
        Curso curso = new(
            Guid.CreateVersion7(),
            "AB",
            "Curso de desenvolvimento com C# e .NET",
            40,
            new DateOnly(2026, 12, 20)
        );

        IReadOnlyList<ErroValidacao> erros = curso.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Validar_NomeComDuzentosCaracteres_DeveSerValido()
    {
        Curso curso = new(
            Guid.CreateVersion7(),
            new string('A', 200),
            "Curso de desenvolvimento com C# e .NET",
            40,
            new DateOnly(2026, 12, 20)
        );

        IReadOnlyList<ErroValidacao> erros = curso.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Validar_DescricaoComQuinhentosCaracteres_DeveSerValida()
    {
        Curso curso = new(
            Guid.CreateVersion7(),
            "Curso de C#",
            new string('A', 500),
            40,
            new DateOnly(2026, 12, 20)
        );

        IReadOnlyList<ErroValidacao> erros = curso.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Validar_NomeLongo_DeveRetornarErro()
    {
        Curso curso = new Curso(
            Guid.CreateVersion7(),
            new string('A', 201),
            "Curso de desenvolvimento com C# e .NET",
            40,
            new DateOnly(2026, 12, 20));

        IReadOnlyList<ErroValidacao> erros = curso.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O nome deve possuir entre 2 e 200 caracteres.",
            erros.First().Mensagem
        );
    }

    [TestMethod]
    public void Validar_DescricaoLonga_DeveRetornarErro()
    {
        Curso curso = new Curso(
            Guid.CreateVersion7(),
            "Curso de C#",
            new string('A', 501),
            40,
            new DateOnly(2026, 12, 20));

        IReadOnlyList<ErroValidacao> erros = curso.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "A descrição deve possuir no máximo 500 caracteres.",
            erros.First().Mensagem
        );
    }

    [TestMethod]
    public void Validar_CargaHorariaUm_DeveSerValida()
    {
        Curso curso = new(
            Guid.CreateVersion7(),
            "Curso de C#",
            "Curso de desenvolvimento com C# e .NET",
            1,
            new DateOnly(2026, 12, 20)
        );

        IReadOnlyList<ErroValidacao> erros = curso.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Validar_CargaHorariaMenorQueZero_DeveRetornarErro()
    {
        Curso curso = new Curso(
            Guid.CreateVersion7(),
            "Curso de C#",
            "Curso de desenvolvimento com C# e .NET",
            -1,
            new DateOnly(2026, 12, 20));

        IReadOnlyList<ErroValidacao> erros = curso.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "A carga horária deve ser maior que zero.",
            erros.First().Mensagem
        );
    }

    [TestMethod]
    public void Validar_CargaHorariaZero_DeveRetornarErro()
    {
        Curso curso = new Curso(
            Guid.CreateVersion7(),
            "Curso de C#",
            "Curso de desenvolvimento com C# e .NET",
            0,
            new DateOnly(2026, 12, 20));

        IReadOnlyList<ErroValidacao> erros = curso.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "A carga horária deve ser maior que zero.",
            erros.First().Mensagem
        );
    }

    [TestMethod]
    public void Validar_DataConclusaoNaoInformada_DeveRetornarErro()
    {
        Curso curso = new Curso(
            Guid.CreateVersion7(),
            "Curso de C#",
            "Curso de desenvolvimento com C# e .NET",
            40,
            default
        );

        IReadOnlyList<ErroValidacao> erros = curso.Validar();

        Assert.HasCount(1, erros);

        Assert.AreEqual(
            "A data de conclusão do curso é obrigatória.",
            erros.First().Mensagem
        );
    }

    [TestMethod]
    public void Atualizar_DeveAtualizarTodosOsDados()
    {
        Curso curso = new(
            Guid.CreateVersion7(),
            "Curso de C#",
            "Curso de desenvolvimento com C#",
            40,
            new DateOnly(2026, 12, 20)
        );

        Curso cursoAtualizado = new(
            Guid.CreateVersion7(),
            "Curso de .NET",
            "Curso avançado de desenvolvimento com .NET",
            80,
            new DateOnly(2027, 01, 30)
        );

        curso.Atualizar(cursoAtualizado);

        Assert.AreEqual("Curso de .NET", curso.Nome);
        Assert.AreEqual(
            "Curso avançado de desenvolvimento com .NET",
            curso.DescricaoCurso
        );
        Assert.AreEqual(80, curso.CargaHoraria);
        Assert.AreEqual(
            new DateOnly(2027, 01, 30),
            curso.DataConclusao
        );
    }
}