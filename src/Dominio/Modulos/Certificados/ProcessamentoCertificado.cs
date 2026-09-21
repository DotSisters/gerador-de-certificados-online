using GeradorDeCertificados.Dominio.Compartilhado;

namespace GeradorDeCertificados.Dominio.Modulos.Certificados;

public sealed class ProcessamentoCertificado : EntidadeBase<ProcessamentoCertificado>
{
    private readonly List<Certificado> certificados = [];

    public Guid CursoId { get; private set; }
    public StatusProcessamento Status { get; private set; }
    public string? CaminhoZip { get; private set; }
    public DateTime DataSolicitacao { get; private set; }
    public IReadOnlyCollection<Certificado> Certificados => certificados;

    public bool EstaEmAndamento =>
        Status is StatusProcessamento.Pendente
            or StatusProcessamento.GerandoCertificados
            or StatusProcessamento.GerandoZip;

    private ProcessamentoCertificado() { }

    public ProcessamentoCertificado(Guid id, Guid cursoId, IEnumerable<string> nomesAlunos)
    {
        Id = id;
        CursoId = cursoId;
        Status = StatusProcessamento.Pendente;
        DataSolicitacao = DateTime.UtcNow;

        foreach (string nome in nomesAlunos ?? [])
        {
            certificados.Add(new Certificado(Guid.CreateVersion7(), id, nome));
        }
    }

    public override IReadOnlyList<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (certificados.Count == 0)
        {
            erros.Add(new ErroValidacao(
                "Alunos",
                "A solicitação deve possuir pelo menos um aluno."
            ));
        }

        foreach (Certificado certificado in certificados)
        {
            erros.AddRange(certificado.Validar());
        }

        return erros;
    }

    public override void Atualizar(ProcessamentoCertificado entidadeAtualizada)
    {
        Status = entidadeAtualizada.Status;
        CaminhoZip = entidadeAtualizada.CaminhoZip;
    }

    public void MarcarComoGerandoCertificados()
    {
        Status = StatusProcessamento.GerandoCertificados;
    }

    public void MarcarComoGerandoZip()
    {
        Status = StatusProcessamento.GerandoZip;
    }

    public void MarcarComoConcluido(string caminhoZip)
    {
        CaminhoZip = caminhoZip;
        Status = StatusProcessamento.Concluido;
    }

    public void MarcarComoFalha()
    {
        Status = StatusProcessamento.Falha;
    }
}
