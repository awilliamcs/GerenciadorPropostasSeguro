using GPS.ContratacaoService.Domain.Entidades;
using GPS.ContratacaoService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GPS.ContratacaoService.Infrastructure.Repositories
{
    public class ContratacaoRepository(ContratacaoDbContext db) : IContratacaoRepository
    {
        private readonly ContratacaoDbContext _db = db;

        public async Task<Contratacao> SalvarAsync(Contratacao Contratacao, CancellationToken ct = default)
        {
            var exists = await _db.Contratacoes.AsNoTracking().AnyAsync(p => p.IdContratacao == Contratacao.IdContratacao, ct);

            if (exists)
                _db.Contratacoes.Update(Contratacao);
            else
                await _db.Contratacoes.AddAsync(Contratacao, ct);

            await _db.SaveChangesAsync(ct);
            return Contratacao;
        }

        public Task<Contratacao?> ObterPorIdAsync(Guid idContratacao, CancellationToken ct = default) => _db.Contratacoes.AsNoTracking().FirstOrDefaultAsync(p => p.IdContratacao == idContratacao, ct);

        public async Task<(List<Contratacao> Items, int Total)> ObterAsync(int pagina, int quantidadeItens, CancellationToken ct = default)
        {
            if (pagina < 1) pagina = 1;
            if (quantidadeItens < 1) quantidadeItens = 10;

            var query = _db.Contratacoes.AsNoTracking();

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(p => p.DataContratacao)
                .Skip((pagina - 1) * quantidadeItens)
                .Take(quantidadeItens)
                .ToListAsync(ct);

            return (items, total);
        }
    }
}
