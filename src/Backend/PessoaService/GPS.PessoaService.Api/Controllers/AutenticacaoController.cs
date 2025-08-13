using GPS.PessoaService.Application.DTOs;
using GPS.PessoaService.Domain.Entidades;
using GPS.PessoaService.Infrastructure;
using GPS.PessoaService.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GPS.PessoaService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacaoController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        PessoaDbContext db,
        IConfiguration configuration) : ControllerBase
    {
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken ct)
        {
            var result = await CriarPessoaComUsuarioAsync(dto, ct);
            
            if (!result.Success)
            {
                return BadRequest(result.Errors);
            }
            
            return Ok(result.Result);
        }

        private async Task<(bool Success, object Result, IEnumerable<string> Errors)> CriarPessoaComUsuarioAsync(RegisterDto dto, CancellationToken ct)
        {
            var exists = await userManager.FindByEmailAsync(dto.Email);
            if (exists != null) 
                return (false, null!, new[] { "E-mail já cadastrado." });

            using var tx = await db.Database.BeginTransactionAsync(ct);

            try
            {
                var pessoa = new Pessoa(dto.Nome, dto.Email, dto.Telefone, dto.DataNascimento);
                await db.Pessoas.AddAsync(pessoa, ct);
                await db.SaveChangesAsync(ct);

                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = dto.Email,
                    Email = dto.Email,
                    EmailConfirmed = true,
                    PessoaId = pessoa.IdPessoa
                };

                var result = await userManager.CreateAsync(user, dto.Senha);
                if (!result.Succeeded)
                {
                    await tx.RollbackAsync(ct);
                    return (false, null!, result.Errors.Select(e => e.Description));
                }

                await tx.CommitAsync(ct);
                return (true, new { pessoa.IdPessoa }, Enumerable.Empty<string>());
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null) return Unauthorized("Credenciais inválidas.");

            var pwdOk = await signInManager.CheckPasswordSignInAsync(user, dto.Senha, lockoutOnFailure: false);
            if (!pwdOk.Succeeded) return Unauthorized("Credenciais inválidas.");

            var token = GenerateJwt(user);
            return Ok(token);
        }

        private AutenticacaoResponseDto GenerateJwt(ApplicationUser user)
        {
            var section = configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(section["SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiresMinutes = Convert.ToInt32(section["ExpiresInMinutes"] ?? "60");
            var expires = DateTime.UtcNow.AddMinutes(expiresMinutes);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim("PessoaId", user.PessoaId.ToString())
            };

            var jwt = new JwtSecurityToken(
                issuer: section["Issuer"],
                audience: section["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds);

            return new AutenticacaoResponseDto(new JwtSecurityTokenHandler().WriteToken(jwt), expires);
        }
    }
}
