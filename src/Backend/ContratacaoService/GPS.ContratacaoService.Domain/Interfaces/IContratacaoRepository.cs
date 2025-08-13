using GPS.ContratacaoService.Domain.Entidades;

namespace GPS.ContratacaoService.Domain.Interfaces
{
    public interface IContratacaoRepository
    {
        Task<Contratacao> SalvarAsync(Contratacao Contratacao, CancellationToken ct = default);
        Task<Contratacao?> ObterPorIdAsync(Guid idContratacao, CancellationToken ct = default);

        /// <summary>
        /// Retorna as Contratacaos paginado
        /// </summary>
        /// <param name="pagina">página atual</param>
        /// <param name="quantidadeItens">quantidade de itens</param>
        Task<(List<Contratacao> Items, int Total)> ObterAsync(int pagina, int quantidadeItens, CancellationToken ct = default);
    }
}
