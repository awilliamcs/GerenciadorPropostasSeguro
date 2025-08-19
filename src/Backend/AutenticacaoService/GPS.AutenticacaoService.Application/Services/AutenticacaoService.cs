using GPS.AutenticacaoService.Application.DTOs;
using GPS.AutenticacaoService.Application.Interfaces;
using GPS.AutenticacaoService.Application.Sagas;
using GPS.AutenticacaoService.Domain.Entidades;
using GPS.CrossCutting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GPS.AutenticacaoService.Application.Services
{
    public class AutenticacaoService(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IRegistroUsuarioSagaOrchestrator usuarioSagaOrchestrator,
        IConfiguration configuration,
        ILogger<AutenticacaoService> logger) : IAutenticacaoService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly IRegistroUsuarioSagaOrchestrator _usuarioSagaOrchestrator = usuarioSagaOrchestrator;
        private readonly ILogger<AutenticacaoService> _logger = logger;

        public async Task<Result<AutenticacaoResponseDto>> EfetuarLoginAsync(LoginDto dto, CancellationToken ct = default)
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

        public async Task<(bool Success, object Result, IEnumerable<string> Errors)> CriarUsuarioComPessoaAsync(RegisterDto dto, CancellationToken ct = default)
        {
            var exists = await _userManager.FindByEmailAsync(dto.Email);
            if (exists != null)
                return (false, null!, new[] { "E-mail já cadastrado." });

            try
            {
                var sagaData = new RegistroUsuarioSagaData { DadosRegistro = dto };

                var sagaId = await _usuarioSagaOrchestrator.IniciarSagaAsync(sagaData, ct);
                return (true, new { SagaId = sagaId, Status = "SAGA usuário iniciada" }, Enumerable.Empty<string>());
            }
            catch (Exception ex)
            {
                return (false, null!, new[] { $"Erro ao iniciar SAGA usuário: {ex.Message}" });
            }
        }

        public async Task<(bool Success, object Result, IEnumerable<string> Errors)> CriarUsuarioAsync(RegisterDto dto, CancellationToken ct = default)
        {
            var exists = await _userManager.FindByEmailAsync(dto.Email);
            if (exists != null)
                return (false, null!, new[] { "E-mail já cadastrado." });

            try
            {
                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = dto.Email,
                    Email = dto.Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, dto.Senha);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Usuário criado com sucesso. UserId: {UserId}, Email: {Email}", user.Id, user.Email);
                    
                    var response = new
                    {
                        UserId = user.Id,
                        user.Email,
                        Message = "Usuário criado com sucesso."
                    };
                    
                    return (true, response, Enumerable.Empty<string>());
                }
                else
                {
                    var errors = result.Errors.Select(e => e.Description);
                    _logger.LogWarning("Falha ao criar usuário. Email: {Email}, Erros: {Errors}", dto.Email, string.Join(", ", errors));
                    return (false, null!, errors);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar usuário. Email: {Email}", dto.Email);
                return (false, null!, new[] { "Erro interno ao criar usuário." });
            }
        }

        public async Task<(bool Success, object Result, IEnumerable<string> Errors)> ConsultarStatusSagaAsync(Guid sagaId, CancellationToken ct = default)
        {
            try
            {
                var saga = await _usuarioSagaOrchestrator.ObterSagaAsync(sagaId, ct);
                if (saga == null)
                    return (false, null!, new[] { "SAGA não encontrada." });

                var result = new
                {
                    saga.SagaId,
                    Status = saga.Status.ToString(),
                    saga.Erros,
                    saga.DataInicio,
                    saga.DataFim,
                    saga.UserId,
                    PessoaId = saga.PessoaCriada != null ? ((PessoaDto)saga.PessoaCriada).IdPessoa : (Guid?)null
                };

                return (true, result, Enumerable.Empty<string>());
            }
            catch (Exception ex)
            {
                return (false, null!, new[] { $"Erro ao consultar SAGA: {ex.Message}" });
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
                new("PessoaId", user.PessoaId.ToString() ?? string.Empty),
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
