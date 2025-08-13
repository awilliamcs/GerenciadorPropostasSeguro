namespace GPS.ContratacaoService.Domain.Entidades
{
    public class Contratacao(Guid idProposta)
    {
        public Guid IdContratacao { get; private set; } = Guid.NewGuid();
        public Guid IdProposta { get; private set; } = idProposta;
        public DateTime DataContratacao { get; private set; } = DateTime.UtcNow;
    }
}
