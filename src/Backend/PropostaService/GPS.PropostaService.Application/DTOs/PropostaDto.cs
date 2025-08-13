using GPS.CrossCutting.Enums;

namespace GPS.PropostaService.Application.DTOs
{
    public class PropostaDto
    {
        public Guid Id { get; set; }
        public Guid IdPessoa { get; set; }
        public TipoProposta Tipo { get; set; }
        public decimal Valor { get; set; }
        public StatusProposta Status { get; set; }
        public DateTime DataSolicitacao { get; set; }
        public Guid? IdUsuarioResponsavel { get; set; }
    }
}
