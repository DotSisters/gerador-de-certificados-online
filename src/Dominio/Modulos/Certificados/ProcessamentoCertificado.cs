using GeradorDeCertificados.Dominio.Compartilhado;

namespace GeradorDeCertificados.Dominio.Modulos.Certificados;

public sealed class ProcessamentoCertificado : EntidadeBase<ProcessamentoCertificado>
{
    private readonly List<Certificado> certificados = [];

    public Guid CursoId { get; private set; }
    public StatusProcessamento Status { get; private set; }
    public string? CaminhoZip { get; private set; }
    public DateTime DataSolicitacao { get; private set; }
    public DateTime? ConcluidoEm { get; private set; }
    public IReadOnlyCollection<Certificado> Certificados => certificados;

    public int Gerados => certificados.Count(c => c.StatusGeracao == StatusGeracao.Gerado);
    public int Falhas => certificados.Count(c => c.StatusGeracao == StatusGeracao.Falha);
    public bool TodosCertificadosProcessados =>
        certificados.Count > 0 && certificados.All(c => c.StatusGeracao != StatusGeracao.Pendente);
    public bool EstaFinalizado =>
        Status is StatusProcessamento.Concluido or StatusProcessamento.Falha;
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
        ConcluidoEm = entidadeAtualizada.ConcluidoEm;
    }

    public void RegistrarSucesso(Guid certificadoId, string caminhoArquivo)
    {
        Certificado certificado = EncontrarPendente(certificadoId);
        certificado.RegistrarGeracao(caminhoArquivo);
    }

    public void RegistrarFalha(Guid certificadoId)
    {
        Certificado certificado = EncontrarPendente(certificadoId);
        certificado.RegistrarFalha();
    }

    public void IniciarGeracaoCertificados()
    {
        if (Status != StatusProcessamento.Pendente)
        {
            throw new InvalidOperationException(
                "Só é possível iniciar a geração de certificados a partir do status Pendente."
            );
        }

        Status = StatusProcessamento.GerandoCertificados;
    }

    public void IniciarGeracaoZip()
    {
        if (Status != StatusProcessamento.GerandoCertificados)
        {
            throw new InvalidOperationException(
                "Só é possível iniciar a geração do ZIP a partir do status GerandoCertificados."
            );
        }

        if (!TodosCertificadosProcessados)
        {
            throw new InvalidOperationException(
                "Não é possível iniciar a geração do ZIP enquanto houver certificados pendentes."
            );
        }

        if (Gerados == 0)
        {
            throw new InvalidOperationException(
                "Não é possível iniciar a geração do ZIP sem pelo menos um certificado gerado."
            );
        }

        Status = StatusProcessamento.GerandoZip;
    }

    public void RegistrarZip(string caminhoZip)
    {
        if (Status != StatusProcessamento.GerandoZip)
        {
            throw new InvalidOperationException(
                "Só é possível registrar o ZIP a partir do status GerandoZip."
            );
        }

        if (!TodosCertificadosProcessados)
        {
            throw new InvalidOperationException(
                "Não é possível registrar o ZIP enquanto houver certificados pendentes."
            );
        }

        if (string.IsNullOrWhiteSpace(caminhoZip))
        {
            throw new ArgumentException(
                "O caminho do arquivo ZIP é obrigatório.",
                nameof(caminhoZip)
            );
        }

        CaminhoZip = caminhoZip;
        Status = StatusProcessamento.Concluido;
        ConcluidoEm = DateTime.UtcNow;
    }

    public void RegistrarFalhaProcessamento()
    {
        if (EstaFinalizado)
        {
            throw new InvalidOperationException(
                "Não é possível registrar falha em um processamento já finalizado."
            );
        }

        Status = StatusProcessamento.Falha;
        ConcluidoEm = DateTime.UtcNow;
    }

    private Certificado EncontrarPendente(Guid certificadoId)
    {
        return certificados.Single(c =>
            c.Id == certificadoId && c.StatusGeracao == StatusGeracao.Pendente
        );
    }
}
