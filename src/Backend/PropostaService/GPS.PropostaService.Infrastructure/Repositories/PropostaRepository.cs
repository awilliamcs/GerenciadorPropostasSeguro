using GPS.PropostaService.Domain.Entidades;
using GPS.PropostaService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GPS.PropostaService.Infrastructure.Repositories
{
    public class PropostaRepository(PropostaDbContext db) : IPropostaRepository
    {
        private readonly PropostaDbContext _db = db;

        public async Task<Proposta> SalvarAsync(Proposta proposta, CancellationToken ct = default)
        {
            var exists = await _db.Propostas.AsNoTracking().AnyAsync(p => p.IdProposta == proposta.IdProposta, ct);

            if (exists)
                _db.Propostas.Update(proposta);
            else
                await _db.Propostas.AddAsync(proposta, ct);

            await _db.SaveChangesAsync(ct);
            return proposta;
        }

        public Task<Proposta?> ObterPorIdAsync(Guid idProposta, CancellationToken ct = default) => _db.Propostas.AsNoTracking().FirstOrDefaultAsync(p => p.IdProposta == idProposta, ct);

        public async Task<(List<Proposta> Items, int Total)> ObterAsync(int pagina, int quantidadeItens, CancellationToken ct = default)
        {
            if (pagina < 1) pagina = 1;
            if (quantidadeItens < 1) quantidadeItens = 10;

            var query = _db.Propostas.AsNoTracking();

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(p => p.DataSolicitacao)
                .Skip((pagina - 1) * quantidadeItens)
                .Take(quantidadeItens)
                .ToListAsync(ct);

            return (items, total);
        }
    }
}

