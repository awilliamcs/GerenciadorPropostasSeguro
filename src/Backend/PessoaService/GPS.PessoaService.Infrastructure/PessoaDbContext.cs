using GPS.PessoaService.Domain.Entidades;
using GPS.PessoaService.Infrastructure.Identity;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GPS.PessoaService.Infrastructure
{
    public class PessoaDbContext(DbContextOptions<PessoaDbContext> options) : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
    {
        public DbSet<Pessoa> Pessoas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Configurar tabelas do Outbox do MassTransit
            builder.AddInboxStateEntity();
            builder.AddOutboxMessageEntity();
            builder.AddOutboxStateEntity();
            
            builder.ApplyConfigurationsFromAssembly(typeof(PessoaDbContext).Assembly);
        }
    }
}
