namespace GPS.PessoaService.Application.Events
{
    // Eventos específicos do domínio Pessoa
    public class PessoaCriadaEvent
    {
        public Guid SagaId { get; set; }
        public Guid PessoaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class PessoaCriacaoFalhouEvent
    {
        public Guid SagaId { get; set; }
        public string Erro { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class DeletarPessoaCompensacaoEvent
    {
        public Guid SagaId { get; set; }
        public Guid PessoaId { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
