namespace GPS.PropostaService.Application.DTOs
{
    public class PropostaListResponseDto
    {
        public List<PropostaDto> Items { get; set; } = [];
        public int Total { get; set; }
        public int Pagina { get; set; }
        public int QuantidadeItens { get; set; }
        public int TotalPaginas => (int)Math.Ceiling((double)Total / QuantidadeItens);
    }
}
