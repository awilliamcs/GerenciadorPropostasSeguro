using GPS.PessoaService.Domain.Entidades;
using Microsoft.EntityFrameworkCore;

namespace GPS.PessoaService.Infrastructure
{
    public class PessoaDbContext(DbContextOptions<PessoaDbContext> options) : DbContext(options)
    {
        public DbSet<Pessoa> Pessoas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);            
            builder.ApplyConfigurationsFromAssembly(typeof(PessoaDbContext).Assembly);
        }
    }
}
