namespace GPS.AutenticacaoService.Application.Events
{
    public class DeletarPessoaCompensacaoEvent
    {
        public Guid SagaId { get; set; }
        public Guid PessoaId { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
