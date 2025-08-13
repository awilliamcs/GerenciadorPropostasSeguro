using GPS.CrossCutting;
using GPS.PessoaService.Application.DTOs;
using GPS.PessoaService.Application.Interfaces;
using GPS.PessoaService.Domain.Entidades;
using GPS.PessoaService.Domain.Interfaces;
using GPS.PessoaService.Infrastructure;
using GPS.PessoaService.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GPS.PessoaService.Application.Services
{
    public class AutenticacaoService(UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager, 
        IConfiguration configuration,
        IPessoaRepository pessoaRepository,
        PessoaDbContext db) : IAutenticacaoService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly PessoaDbContext _db = db;

        public async Task<Result<AutenticacaoResponseDto>> LoginAsync(LoginDto dto, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Result<AutenticacaoResponseDto>.Fail("Credenciais inválidas.");

            var pwdOk = await _signInManager.CheckPasswordSignInAsync(user, dto.Senha, lockoutOnFailure: false);
            if (!pwdOk.Succeeded)
                return Result<AutenticacaoResponseDto>.Fail("Credenciais inválidas.");

            var token = GenerateJwt(user);
            return Result<AutenticacaoResponseDto>.Ok(token);
        }

        public async Task<(bool Success, object Result, IEnumerable<string> Errors)> RegistrarUsuarioAsync(RegisterDto dto, CancellationToken ct = default)
        {
            var exists = await _userManager.FindByEmailAsync(dto.Email);
            if (exists != null)
                return (false, null!, new[] { "E-mail já cadastrado." });

            using var transaction = await db.Database.BeginTransactionAsync(ct);

            try
            {
                var pessoa = new Pessoa(dto.Nome, dto.Email, dto.Telefone, dto.DataNascimento);
                await pessoaRepository.SalvarAsync(pessoa, ct);
                
                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = dto.Email,
                    Email = dto.Email,
                    EmailConfirmed = true,
                    PessoaId = pessoa.IdPessoa
                };

                var result = await _userManager.CreateAsync(user, dto.Senha);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync(ct);
                    return (false, null!, result.Errors.Select(e => e.Description));
                }

                await transaction.CommitAsync(ct);
                return (true, new { pessoa.IdPessoa }, Enumerable.Empty<string>());
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        private AutenticacaoResponseDto GenerateJwt(ApplicationUser user)
        {
            var section = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(section["SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiresMinutes = Convert.ToInt32(section["ExpiresInMinutes"] ?? "60");
            var expires = DateTime.UtcNow.AddMinutes(expiresMinutes);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new("PessoaId", user.PessoaId.ToString()),
                new("EhAdministrador", (user.EhAdministrador ? 1 : 0).ToString()),
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
