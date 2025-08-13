using FluentAssertions;
using GPS.PessoaService.Api.Controllers;
using GPS.PessoaService.Application.DTOs;
using GPS.PessoaService.Infrastructure;
using GPS.PessoaService.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace GPS.PessoaService.Tests.Controllers
{
    public class AutenticacaoControllerTests : IDisposable
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly PessoaDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly AutenticacaoController _controller;

        public AutenticacaoControllerTests()
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

            _controller = new AutenticacaoController(_userManager, _signInManager, _dbContext, _configuration);
        }

        [Fact]
        public async Task Register_ComEmailJaExistente_DeveRetornarBadRequest()
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

            
            var result = await _controller.Register(dto, CancellationToken.None);

            
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.Value.Should().BeAssignableTo<IEnumerable<string>>();
        }

        [Fact]
        public async Task Register_ComDadosValidos_DeveRetornarOk()
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

            
            var result = await _controller.Register(dto, CancellationToken.None);

            
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task Register_ComFalhaNoUserManager_DeveRetornarBadRequest()
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

            
            var result = await _controller.Register(dto, CancellationToken.None);

            
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            var errors = badRequestResult!.Value as IEnumerable<string>;
            errors.Should().Contain("Senha muito fraca");
        }

        [Fact]
        public async Task Login_ComEmailInexistente_DeveRetornarUnauthorized()
        {
            
            var dto = new LoginDto("email@inexistente.com", "senha123");
            _userManager.FindByEmailAsync(dto.Email).Returns((ApplicationUser?)null);

            
            var result = await _controller.Login(dto);

            
            result.Should().BeOfType<UnauthorizedObjectResult>();
            var unauthorizedResult = result as UnauthorizedObjectResult;
            unauthorizedResult!.Value.Should().Be("Credenciais inválidas.");
        }

        [Fact]
        public async Task Login_ComSenhaIncorreta_DeveRetornarUnauthorized()
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

            
            var result = await _controller.Login(dto);

            
            result.Should().BeOfType<UnauthorizedObjectResult>();
            var unauthorizedResult = result as UnauthorizedObjectResult;
            unauthorizedResult!.Value.Should().Be("Credenciais inválidas.");
        }

        [Fact]
        public async Task Login_ComCredenciaisValidas_DeveRetornarOkComToken()
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

            
            var result = await _controller.Login(dto);

            
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as AutenticacaoResponseDto;
            response.Should().NotBeNull();
            response!.Token.Should().NotBeNullOrEmpty();
            response.Expiracao.Should().BeAfter(DateTime.UtcNow);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task Login_ComEmailInvalido_DeveRetornarUnauthorized(string email)
        {
            
            var dto = new LoginDto(email, "senha123");
            _userManager.FindByEmailAsync(email).Returns((ApplicationUser?)null);

            
            var result = await _controller.Login(dto);

            
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task Register_ComNomeInvalido_DeveGerarExcecao(string nome)
        {
            
            var dto = new RegisterDto
            {
                Nome = nome,
                Email = "joao@teste.com",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Senha = "MinhaSenh@123"
            };

            _userManager.FindByEmailAsync(dto.Email).Returns((ApplicationUser?)null);

             var exception = await Assert.ThrowsAnyAsync<Exception>(
                () => _controller.Register(dto, CancellationToken.None));
            
            // Verifica se a exceção é uma das esperadas para nome inválido
            exception.Should().Match<Exception>(e => 
                e is DbUpdateException || e is NullReferenceException);
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
