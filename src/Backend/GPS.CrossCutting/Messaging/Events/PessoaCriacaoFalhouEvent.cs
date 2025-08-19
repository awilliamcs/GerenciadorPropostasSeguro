namespace GPS.CrossCutting.Messaging.Events
{
    public class PessoaCriacaoFalhouEvent
    {
        public Guid CorrelationId { get; set; }
        public string Erro { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
