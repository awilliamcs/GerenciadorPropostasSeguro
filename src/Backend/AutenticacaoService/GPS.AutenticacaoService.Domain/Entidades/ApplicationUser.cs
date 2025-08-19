using Microsoft.AspNetCore.Identity;

namespace GPS.AutenticacaoService.Domain.Entidades
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public Guid? PessoaId { get; set; }
        public bool EhAdministrador { get; set; }
    }
}
