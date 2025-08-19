namespace GPS.CrossCutting.Messaging.Events
{
    public class UsuarioCriadoEvent
    {
        public Guid CorrelationId { get; set; }
        public Guid UserId { get; set; }
        public Guid PessoaId { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
