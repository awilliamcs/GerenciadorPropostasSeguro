namespace GPS.AutenticacaoService.Application.Events
{
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
}
