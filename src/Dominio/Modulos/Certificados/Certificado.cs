using GeradorDeCertificados.Dominio.Compartilhado;

namespace GeradorDeCertificados.Dominio.Modulos.Certificados;

public sealed class Certificado : EntidadeBase<Certificado>
{
    public Guid ProcessamentoId { get; private set; }
    public string NomeAluno { get; private set; } = string.Empty;
    public string? CaminhoArquivo { get; private set; }
    public DateTime? DataGeracao { get; private set; }
    public StatusCertificado Status { get; private set; }

    private Certificado() { }

    public Certificado(Guid id, Guid processamentoId, string nomeAluno)
    {
        Id = id;
        ProcessamentoId = processamentoId;
        NomeAluno = nomeAluno?.Trim() ?? string.Empty;
        Status = StatusCertificado.Pendente;
    }

    public override IReadOnlyList<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (string.IsNullOrWhiteSpace(NomeAluno))
        {
            erros.Add(new ErroValidacao(
                nameof(NomeAluno),
                "O nome do aluno é obrigatório."
            ));
        }
        else if (NomeAluno.Length > 200)
        {
            erros.Add(new ErroValidacao(
                nameof(NomeAluno),
                "O nome do aluno deve possuir no máximo 200 caracteres."
            ));
        }

        return erros;
    }

    public override void Atualizar(Certificado entidadeAtualizada)
    {
        NomeAluno = entidadeAtualizada.NomeAluno.Trim();
        CaminhoArquivo = entidadeAtualizada.CaminhoArquivo;
        DataGeracao = entidadeAtualizada.DataGeracao;
        Status = entidadeAtualizada.Status;
    }

    public void MarcarComoGerado(string caminho, DateTime dataGeracao)
    {
        CaminhoArquivo = caminho;
        DataGeracao = dataGeracao;
        Status = StatusCertificado.Gerado;
    }

    public void MarcarComoFalha()
    {
        Status = StatusCertificado.Falha;
    }
}
