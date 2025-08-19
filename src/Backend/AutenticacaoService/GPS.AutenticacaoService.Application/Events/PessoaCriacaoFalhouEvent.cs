namespace GPS.AutenticacaoService.Application.Events
{
    public class PessoaCriacaoFalhouEvent
    {
        public Guid SagaId { get; set; }
        public string Erro { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
