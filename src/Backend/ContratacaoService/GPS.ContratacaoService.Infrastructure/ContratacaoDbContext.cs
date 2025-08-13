using GPS.ContratacaoService.Domain.Entidades;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace GPS.ContratacaoService.Infrastructure
{
    public class ContratacaoDbContext(DbContextOptions<ContratacaoDbContext> opts) : DbContext(opts)
    {
        public DbSet<Contratacao> Contratacoes { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configurar tabelas do Outbox do MassTransit
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContratacaoDbContext).Assembly);
        }
    }
}
