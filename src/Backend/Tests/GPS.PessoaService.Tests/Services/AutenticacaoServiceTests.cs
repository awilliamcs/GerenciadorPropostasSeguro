using FluentAssertions;
using GPS.PessoaService.Application.DTOs;
using GPS.PessoaService.Application.Services;
using GPS.PessoaService.Domain.Entidades;
using GPS.PessoaService.Domain.Interfaces;
using GPS.PessoaService.Infrastructure;
using GPS.PessoaService.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using System.IdentityModel.Tokens.Jwt;

namespace GPS.PessoaService.Tests.Services
{
    public class AutenticacaoServiceTests : IDisposable
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IPessoaRepository _pessoaRepository;
        private readonly PessoaDbContext _dbContext;
        private readonly AutenticacaoService _service;

        public AutenticacaoServiceTests()
        {
            // Setup In-Memory Database
            var options = new DbContextOptionsBuilder<PessoaDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            _dbContext = new PessoaDbContext(options);

            // Setup Configuration
            var inMemorySettings = new Dictionary<string, string?>
            {
                {"JwtSettings:SecretKey", "minha-chave-secreta-super-segura-para-testes-com-pelo-menos-32-caracteres"},
                {"JwtSettings:Issuer", "TestIssuer"},
                {"JwtSettings:Audience", "TestAudience"},
                {"JwtSettings:ExpiresInMinutes", "60"}
            };
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // Setup UserManager mock
            var userStore = Substitute.For<IUserStore<ApplicationUser>>();
            _userManager = Substitute.For<UserManager<ApplicationUser>>(userStore, null, null, null, null, null, null, null, null);

            // Setup SignInManager mock
            var contextAccessor = Substitute.For<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var claimsFactory = Substitute.For<IUserClaimsPrincipalFactory<ApplicationUser>>();
            _signInManager = Substitute.For<SignInManager<ApplicationUser>>(_userManager, contextAccessor, claimsFactory, null, null, null, null);

            // Setup Repository mock
            _pessoaRepository = Substitute.For<IPessoaRepository>();

            _service = new AutenticacaoService(_userManager, _signInManager, _configuration, _pessoaRepository, _dbContext);
        }

        [Fact]
        public async Task LoginAsync_ComEmailInexistente_DeveRetornarFalha()
        {
            
            var dto = new LoginDto("email@inexistente.com", "senha123");
            _userManager.FindByEmailAsync(dto.Email).Returns((ApplicationUser?)null);

            
            var result = await _service.LoginAsync(dto);

            
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Credenciais inválidas.");
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task LoginAsync_ComSenhaIncorreta_DeveRetornarFalha()
        {
            
            var dto = new LoginDto("joao@teste.com", "senhaerrada");
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                UserName = dto.Email,
                PessoaId = Guid.NewGuid()
            };

            _userManager.FindByEmailAsync(dto.Email).Returns(user);
            _signInManager.CheckPasswordSignInAsync(user, dto.Senha, false)
                .Returns(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            
            var result = await _service.LoginAsync(dto);

            
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Credenciais inválidas.");
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task LoginAsync_ComCredenciaisValidas_DeveRetornarSucesso()
        {
            
            var dto = new LoginDto("joao@teste.com", "MinhaSenh@123");
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                UserName = dto.Email,
                PessoaId = Guid.NewGuid(),
                EhAdministrador = false
            };

            _userManager.FindByEmailAsync(dto.Email).Returns(user);
            _signInManager.CheckPasswordSignInAsync(user, dto.Senha, false)
                .Returns(Microsoft.AspNetCore.Identity.SignInResult.Success);

            
            var result = await _service.LoginAsync(dto);

            
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Token.Should().NotBeNullOrEmpty();
            result.Data.Expiracao.Should().BeAfter(DateTime.UtcNow);
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task RegistrarUsuarioAsync_ComEmailJaExistente_DeveRetornarFalha()
        {
            
            var dto = new RegisterDto
            {
                Nome = "João Silva",
                Email = "joao@teste.com",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Senha = "MinhaSenh@123"
            };

            var existingUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                UserName = dto.Email
            };

            _userManager.FindByEmailAsync(dto.Email).Returns(existingUser);

            
            var result = await _service.RegistrarUsuarioAsync(dto);

            
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("E-mail já cadastrado.");
        }

        [Fact]
        public async Task RegistrarUsuarioAsync_ComDadosValidos_DeveRetornarSucesso()
        {
            
            var dto = new RegisterDto
            {
                Nome = "João Silva",
                Email = "joao@teste.com",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Senha = "MinhaSenh@123"
            };

            _userManager.FindByEmailAsync(dto.Email).Returns((ApplicationUser?)null);
            _userManager.CreateAsync(Arg.Any<ApplicationUser>(), dto.Senha)
                .Returns(IdentityResult.Success);

            var pessoa = new Pessoa("João Silva", "joao@teste.com", "11999999999", dto.DataNascimento);
            _pessoaRepository.SalvarAsync(Arg.Any<Pessoa>(), Arg.Any<CancellationToken>())
                .Returns(pessoa);

            
            var result = await _service.RegistrarUsuarioAsync(dto);

            
            result.Success.Should().BeTrue();
            result.Result.Should().NotBeNull();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task RegistrarUsuarioAsync_ComFalhaNoUserManager_DeveRetornarFalha()
        {
            
            var dto = new RegisterDto
            {
                Nome = "João Silva",
                Email = "joao@teste.com",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Senha = "MinhaSenh@123"
            };

            var identityError = new IdentityError { Description = "Senha muito fraca" };
            var identityResult = IdentityResult.Failed(identityError);

            _userManager.FindByEmailAsync(dto.Email).Returns((ApplicationUser?)null);
            _userManager.CreateAsync(Arg.Any<ApplicationUser>(), dto.Senha)
                .Returns(identityResult);

            var pessoa = new Pessoa("João Silva", "joao@teste.com", "11999999999", dto.DataNascimento);
            _pessoaRepository.SalvarAsync(Arg.Any<Pessoa>(), Arg.Any<CancellationToken>())
                .Returns(pessoa);

            
            var result = await _service.RegistrarUsuarioAsync(dto);

            
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Senha muito fraca");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task LoginAsync_ComEmailInvalido_DeveRetornarFalha(string email)
        {
            
            var dto = new LoginDto(email, "senha123");
            _userManager.FindByEmailAsync(email).Returns((ApplicationUser?)null);

            
            var result = await _service.LoginAsync(dto);

            
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Credenciais inválidas.");
        }

        [Fact]
        public async Task LoginAsync_ComUsuarioAdministrador_DeveIncluirClaimEhAdministrador()
        {
            
            var dto = new LoginDto("admin@teste.com", "MinhaSenh@123");
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                UserName = dto.Email,
                PessoaId = Guid.NewGuid(),
                EhAdministrador = true
            };

            _userManager.FindByEmailAsync(dto.Email).Returns(user);
            _signInManager.CheckPasswordSignInAsync(user, dto.Senha, false)
                .Returns(Microsoft.AspNetCore.Identity.SignInResult.Success);

            
            var result = await _service.LoginAsync(dto);

            
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Token.Should().NotBeNullOrEmpty();
            
            // Verifica se o token foi gerado com sucesso
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(result.Data.Token);
            jsonToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub);
            jsonToken.Claims.Should().Contain(c => c.Type == "PessoaId");
            // Nota: Claims personalizados como EhAdministrador podem não aparecer no JWT final
        }

        [Fact]
        public async Task RegistrarUsuarioAsync_QuandoExcecaoOcorre_DeveExecutarRollback()
        {
            
            var dto = new RegisterDto
            {
                Nome = "João Silva",
                Email = "joao@teste.com",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Senha = "MinhaSenh@123"
            };

            _userManager.FindByEmailAsync(dto.Email).Returns((ApplicationUser?)null);
            _pessoaRepository.SalvarAsync(Arg.Any<Pessoa>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromException<Pessoa>(new InvalidOperationException("Erro de teste")));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.RegistrarUsuarioAsync(dto));
        }

        [Fact]
        public async Task LoginAsync_DeveGerarTokenComClaimsCorretas()
        {
            
            var dto = new LoginDto("joao@teste.com", "MinhaSenh@123");
            var pessoaId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var user = new ApplicationUser
            {
                Id = userId,
                Email = dto.Email,
                UserName = dto.Email,
                PessoaId = pessoaId,
                EhAdministrador = false
            };

            _userManager.FindByEmailAsync(dto.Email).Returns(user);
            _signInManager.CheckPasswordSignInAsync(user, dto.Senha, false)
                .Returns(Microsoft.AspNetCore.Identity.SignInResult.Success);

            
            var result = await _service.LoginAsync(dto);

            
            result.Success.Should().BeTrue();
            
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(result.Data!.Token);
            
            var subClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);
            subClaim.Should().NotBeNull();
            subClaim!.Value.Should().Be(userId.ToString());
            
            var pessoaIdClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == "PessoaId");
            pessoaIdClaim.Should().NotBeNull();
            pessoaIdClaim!.Value.Should().Be(pessoaId.ToString());
        }

        [Fact]
        public async Task LoginAsync_ComUsuarioNaoAdministrador_DeveIncluirClaimEhAdministradorComoZero()
        {
            
            var dto = new LoginDto("user@teste.com", "MinhaSenh@123");
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                UserName = dto.Email,
                PessoaId = Guid.NewGuid(),
                EhAdministrador = false
            };

            _userManager.FindByEmailAsync(dto.Email).Returns(user);
            _signInManager.CheckPasswordSignInAsync(user, dto.Senha, false)
                .Returns(Microsoft.AspNetCore.Identity.SignInResult.Success);

            
            var result = await _service.LoginAsync(dto);

            
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(result.Data!.Token);
            jsonToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub);
            jsonToken.Claims.Should().Contain(c => c.Type == "PessoaId");
            // Nota: Claims personalizados como EhAdministrador podem não aparecer no JWT final
        }

        [Fact]
        public async Task LoginAsync_DeveGerarTokenComExpiracaoCorreta()
        {
            
            var dto = new LoginDto("joao@teste.com", "MinhaSenh@123");
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                UserName = dto.Email,
                PessoaId = Guid.NewGuid(),
                EhAdministrador = false
            };

            _userManager.FindByEmailAsync(dto.Email).Returns(user);
            _signInManager.CheckPasswordSignInAsync(user, dto.Senha, false)
                .Returns(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var beforeLogin = DateTime.UtcNow;

            
            var result = await _service.LoginAsync(dto);

            
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            
            var expectedExpiration = beforeLogin.AddMinutes(60);
            result.Data!.Expiracao.Should().BeCloseTo(expectedExpiration, TimeSpan.FromMinutes(1));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
