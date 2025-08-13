using Microsoft.AspNetCore.Identity;

namespace GPS.PessoaService.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public Guid PessoaId { get; set; }
        public bool EhAdministrador { get; set; }
    }
}
