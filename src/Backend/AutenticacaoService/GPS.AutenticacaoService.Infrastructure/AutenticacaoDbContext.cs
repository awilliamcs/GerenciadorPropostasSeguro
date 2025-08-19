using GPS.AutenticacaoService.Domain.Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GPS.AutenticacaoService.Infrastructure
{
    public class AutenticacaoDbContext(DbContextOptions<AutenticacaoDbContext> options) : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);            
            builder.ApplyConfigurationsFromAssembly(typeof(AutenticacaoDbContext).Assembly);
        }
    }
}
