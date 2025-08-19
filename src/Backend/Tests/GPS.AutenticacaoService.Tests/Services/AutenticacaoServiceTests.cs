using GPS.AutenticacaoService.Application.DTOs;
using GPS.AutenticacaoService.Application.Interfaces;
using GPS.AutenticacaoService.Application.Sagas;
using GPS.AutenticacaoService.Domain.Entidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using FluentAssertions;
using AutenticacaoSvc = GPS.AutenticacaoService.Application.Services.AutenticacaoService;

namespace GPS.AutenticacaoService.Tests.Services
{
    public class AutenticacaoServiceTests
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRegistroUsuarioSagaOrchestrator _sagaOrchestrator;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AutenticacaoSvc> _logger;
        private readonly AutenticacaoSvc _service;

        public AutenticacaoServiceTests()
        {
            _userManager = Substitute.For<UserManager<ApplicationUser>>(
                Substitute.For<IUserStore<ApplicationUser>>(),
                null, null, null, null, null, null, null, null);

            _signInManager = Substitute.For<SignInManager<ApplicationUser>>(
                _userManager,
                Substitute.For<IHttpContextAccessor>(),
                Substitute.For<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null, null, null, null);

            _sagaOrchestrator = Substitute.For<IRegistroUsuarioSagaOrchestrator>();
            _configuration = Substitute.For<IConfiguration>();
            _logger = Substitute.For<ILogger<AutenticacaoSvc>>();

            // Configurar o JWT settings
            var configSection = Substitute.For<IConfigurationSection>();
            configSection["SecretKey"].Returns("minha-chave-secreta-super-segura-com-mais-de-32-caracteres");
            configSection["ExpiresInMinutes"].Returns("60");
            configSection["Issuer"].Returns("GPS.AutenticacaoService");
            configSection["Audience"].Returns("GPS.Application");
            _configuration.GetSection("JwtSettings").Returns(configSection);

            _service = new AutenticacaoSvc(_userManager, _signInManager, _sagaOrchestrator, _configuration, _logger);
        }

        [Fact]
        public async Task EfetuarLoginAsync_CredenciaisValidas_DeveRetornarSucesso()
        {
            
            var loginDto = new LoginDto("usuario@teste.com", "senha123");
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "usuario@teste.com",
                UserName = "usuario@teste.com",
                PessoaId = Guid.NewGuid(),
                EhAdministrador = false
            };

            _userManager.FindByEmailAsync(loginDto.Email).Returns(user);
            _signInManager.CheckPasswordSignInAsync(user, loginDto.Senha, false)
                .Returns(SignInResult.Success);

            
            var result = await _service.EfetuarLoginAsync(loginDto);

            
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Token.Should().NotBeNullOrEmpty();
            result.Data.Expiracao.Should().BeAfter(DateTime.UtcNow);
        }

        [Fact]
        public async Task EfetuarLoginAsync_UsuarioNaoExiste_DeveRetornarFalha()
        {
            
            var loginDto = new LoginDto("usuario@inexistente.com", "senha123");
            _userManager.FindByEmailAsync(loginDto.Email).Returns((ApplicationUser?)null);

            
            var result = await _service.EfetuarLoginAsync(loginDto);

            
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Credenciais inválidas.");
        }

        [Fact]
        public async Task EfetuarLoginAsync_SenhaIncorreta_DeveRetornarFalha()
        {
            
            var loginDto = new LoginDto("usuario@teste.com", "senha_errada");
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "usuario@teste.com",
                UserName = "usuario@teste.com"
            };

            _userManager.FindByEmailAsync(loginDto.Email).Returns(user);
            _signInManager.CheckPasswordSignInAsync(user, loginDto.Senha, false)
                .Returns(SignInResult.Failed);

            
            var result = await _service.EfetuarLoginAsync(loginDto);

            
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Credenciais inválidas.");
        }

        [Fact]
        public async Task CriarUsuarioComPessoaAsync_EmailJaExiste_DeveRetornarFalha()
        {
            
            var registerDto = new RegisterDto
            {
                Nome = "João Silva",
                Email = "joao@existente.com",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Senha = "Senha123!"
            };

            var userExistente = new ApplicationUser { Email = "joao@existente.com" };
            _userManager.FindByEmailAsync(registerDto.Email).Returns(userExistente);

            
            var result = await _service.CriarUsuarioComPessoaAsync(registerDto);

            
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("E-mail já cadastrado.");
        }

        [Fact]
        public async Task CriarUsuarioComPessoaAsync_EmailNovo_DeveIniciarSaga()
        {
            
            var registerDto = new RegisterDto
            {
                Nome = "João Silva",
                Email = "joao@novo.com",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Senha = "Senha123!"
            };

            var sagaId = Guid.NewGuid();
            _userManager.FindByEmailAsync(registerDto.Email).Returns((ApplicationUser?)null);
            _sagaOrchestrator.IniciarSagaAsync(Arg.Any<RegistroUsuarioSagaData>(), Arg.Any<CancellationToken>())
                .Returns(sagaId);

            
            var result = await _service.CriarUsuarioComPessoaAsync(registerDto);

            
            result.Success.Should().BeTrue();
            result.Result.Should().NotBeNull();
            await _sagaOrchestrator.Received(1).IniciarSagaAsync(Arg.Any<RegistroUsuarioSagaData>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CriarUsuarioAsync_EmailJaExiste_DeveRetornarFalha()
        {
            
            var registerDto = new RegisterDto
            {
                Nome = "João Silva",
                Email = "joao@existente.com",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Senha = "Senha123!"
            };

            var userExistente = new ApplicationUser { Email = "joao@existente.com" };
            _userManager.FindByEmailAsync(registerDto.Email).Returns(userExistente);

            
            var result = await _service.CriarUsuarioAsync(registerDto);

            
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("E-mail já cadastrado.");
        }

        [Fact]
        public async Task CriarUsuarioAsync_EmailNovo_DeveCriarUsuario()
        {
            
            var registerDto = new RegisterDto
            {
                Nome = "João Silva",
                Email = "joao@novo.com",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Senha = "Senha123!"
            };

            _userManager.FindByEmailAsync(registerDto.Email).Returns((ApplicationUser?)null);
            _userManager.CreateAsync(Arg.Any<ApplicationUser>(), registerDto.Senha)
                .Returns(IdentityResult.Success);

            
            var result = await _service.CriarUsuarioAsync(registerDto);

            
            result.Success.Should().BeTrue();
            result.Result.Should().NotBeNull();
            await _userManager.Received(1).CreateAsync(Arg.Any<ApplicationUser>(), registerDto.Senha);
        }

        [Fact]
        public async Task CriarUsuarioAsync_FalhaAoCriar_DeveRetornarErros()
        {
            
            var registerDto = new RegisterDto
            {
                Nome = "João Silva",
                Email = "joao@novo.com",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Senha = "123" // Senha fraca
            };

            var identityErrors = new[]
            {
                new IdentityError { Description = "Senha muito fraca" },
                new IdentityError { Description = "Senha deve ter pelo menos 6 caracteres" }
            };

            _userManager.FindByEmailAsync(registerDto.Email).Returns((ApplicationUser?)null);
            _userManager.CreateAsync(Arg.Any<ApplicationUser>(), registerDto.Senha)
                .Returns(IdentityResult.Failed(identityErrors));

            
            var result = await _service.CriarUsuarioAsync(registerDto);

            
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Senha muito fraca");
            result.Errors.Should().Contain("Senha deve ter pelo menos 6 caracteres");
        }

        [Fact]
        public async Task ConsultarStatusSagaAsync_SagaExiste_DeveRetornarStatus()
        {
            
            var sagaId = Guid.NewGuid();
            var saga = new RegistroUsuarioSagaData
            {
                SagaId = sagaId,
                Status = RegistroUsuarioSagaStatus.Finalizado,
                DataInicio = DateTime.UtcNow.AddMinutes(-5),
                DataFim = DateTime.UtcNow,
                UserId = Guid.NewGuid(),
                PessoaCriada = new PessoaDto { IdPessoa = Guid.NewGuid() },
                Erros = new List<string>()
            };

            _sagaOrchestrator.ObterSagaAsync(sagaId, Arg.Any<CancellationToken>()).Returns(saga);

            
            var result = await _service.ConsultarStatusSagaAsync(sagaId);

            
            result.Success.Should().BeTrue();
            result.Result.Should().NotBeNull();
        }

        [Fact]
        public async Task ConsultarStatusSagaAsync_SagaNaoExiste_DeveRetornarFalha()
        {
            
            var sagaId = Guid.NewGuid();
            _sagaOrchestrator.ObterSagaAsync(sagaId, Arg.Any<CancellationToken>()).Returns((RegistroUsuarioSagaData?)null);

            
            var result = await _service.ConsultarStatusSagaAsync(sagaId);

            
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("SAGA não encontrada.");
        }
    }
}
