using GPS.PessoaService.Domain.Entidades;
using GPS.PessoaService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GPS.PessoaService.Infrastructure.Repositories
{
    public class PessoaRepository(PessoaDbContext db) : IPessoaRepository
    {
        private readonly PessoaDbContext _db = db;

        public async Task<Pessoa> SalvarAsync(Pessoa pessoa, CancellationToken ct = default)
        {
            var exists = await _db.Pessoas.AsNoTracking().AnyAsync(p => p.IdPessoa == pessoa.IdPessoa, ct);

            if (exists)
                _db.Pessoas.Update(pessoa);
            else
                await _db.Pessoas.AddAsync(pessoa, ct);

            await _db.SaveChangesAsync(ct);
            return pessoa;
        }

        public Task<Pessoa?> ObterPorIdAsync(Guid idPessoa, CancellationToken ct = default) => _db.Pessoas.AsNoTracking().FirstOrDefaultAsync(p => p.IdPessoa == idPessoa, ct);
    }
}

