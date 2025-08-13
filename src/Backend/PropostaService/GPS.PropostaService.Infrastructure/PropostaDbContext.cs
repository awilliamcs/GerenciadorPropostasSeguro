using GPS.PropostaService.Domain.Entidades;
using Microsoft.EntityFrameworkCore;

namespace GPS.PropostaService.Infrastructure
{
    public class PropostaDbContext(DbContextOptions<PropostaDbContext> opts) : DbContext(opts)
    {
        public DbSet<Proposta> Propostas { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PropostaDbContext).Assembly);
        }
    }
}
