using GPS.PropostaService.Domain.Entidades;

namespace GPS.PropostaService.Domain.Interfaces
{
    public interface IPropostaRepository
    {
        Task<Proposta> SalvarAsync(Proposta proposta, CancellationToken ct = default);
        Task<Proposta?> ObterPorIdAsync(Guid idProposta, CancellationToken ct = default);
        
        /// <summary>
        /// Retorna as propostas paginado
        /// </summary>
        /// <param name="pagina">página atual</param>
        /// <param name="quantidadeItens">quantidade de itens</param>
        Task<(List<Proposta> Items, int Total)> ObterAsync(int pagina, int quantidadeItens, CancellationToken ct = default);
    }
}
