namespace GPS.ContratacaoService.Domain.Entidades
{
    public class ContratacaoListResponseDto
    {
        public List<Contratacao> Items { get; set; } = [];
        public int Total { get; set; }
        public int Pagina { get; set; }
        public int QuantidadeItens { get; set; }
        public int TotalPaginas => (int)Math.Ceiling((double)Total / QuantidadeItens);
    }
}
