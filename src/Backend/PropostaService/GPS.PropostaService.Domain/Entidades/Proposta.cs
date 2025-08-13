using GPS.CrossCutting.Enums;

namespace GPS.PropostaService.Domain.Entidades
{
    public class Proposta(Guid idPessoa, TipoProposta tipo, decimal valor)
    {
        public Guid IdProposta { get; private set; } = Guid.NewGuid();
        public Guid IdPessoa { get; private set; } = idPessoa;
        public TipoProposta Tipo { get; private set; } = tipo;
        public decimal Valor { get; private set; } = valor;
        public StatusProposta Status { get; private set; } = StatusProposta.EmAnalise;
        public DateTime DataSolicitacao { get; private set; } = DateTime.UtcNow;
        public Guid? IdUsuarioResponsavel { get; private set; } = null;

        public void AlterarStatus(StatusProposta novoStatus, Guid idUsuarioResponsavel)
        {
            Status = novoStatus;
            IdUsuarioResponsavel = idUsuarioResponsavel;
        }
    }
}
