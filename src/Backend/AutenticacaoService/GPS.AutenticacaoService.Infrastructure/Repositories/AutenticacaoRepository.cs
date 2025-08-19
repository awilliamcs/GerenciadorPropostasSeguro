using GPS.AutenticacaoService.Domain.Entidades;
using GPS.AutenticacaoService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GPS.AutenticacaoService.Infrastructure.Repositories
{
    public class AutenticacaoRepository(AutenticacaoDbContext db) : IAutenticacaoRepository
    {
        private readonly AutenticacaoDbContext _db = db;

        public async Task<ApplicationUser> SalvarAsync(ApplicationUser usuario, CancellationToken ct = default)
        {
            var exists = await _db.Users.AsNoTracking().AnyAsync(p => p.Id == usuario.Id, ct);

            if (exists)
                _db.Users.Update(usuario);
            else
                await _db.Users.AddAsync(usuario, ct);

            await _db.SaveChangesAsync(ct);
            return usuario;
        }

        public Task<ApplicationUser?> ObterPorIdAsync(Guid id, CancellationToken ct = default) => _db.Users.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);
    }
}

