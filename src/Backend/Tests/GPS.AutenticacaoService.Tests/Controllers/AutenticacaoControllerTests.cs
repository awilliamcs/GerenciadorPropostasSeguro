using GPS.AutenticacaoService.Api.Controllers;
using GPS.AutenticacaoService.Application.DTOs;
using GPS.AutenticacaoService.Application.Interfaces;
using GPS.CrossCutting;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using FluentAssertions;

namespace GPS.AutenticacaoService.Tests.Controllers
{
    public class AutenticacaoControllerTests
    {
        private readonly IAutenticacaoService _autenticacaoService;
        private readonly AutenticacaoController _controller;

        public AutenticacaoControllerTests()
        {
            _autenticacaoService = Substitute.For<IAutenticacaoService>();
            _controller = new AutenticacaoController(_autenticacaoService);
        }

        [Fact]
        public async Task CriarUsuarioComPessoa_DadosValidos_DeveRetornarOk()
        {
            
            var registerDto = new RegisterDto
            {
                Nome = "João Silva",
                Email = "joao@email.com",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Senha = "Senha123!"
            };

            var expectedResult = new { Message = "Usuário criado com sucesso" };
            _autenticacaoService.CriarUsuarioComPessoaAsync(registerDto, Arg.Any<CancellationToken>())
                .Returns((true, (object?)expectedResult, Enumerable.Empty<string>()));

            
            var result = await _controller.CriarUsuarioComPessoa(registerDto, CancellationToken.None);

            
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult?.Value.Should().Be(expectedResult);
            await _autenticacaoService.Received(1).CriarUsuarioComPessoaAsync(registerDto, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CriarUsuarioComPessoa_DadosInvalidos_DeveRetornarBadRequest()
        {
            
            var registerDto = new RegisterDto
            {
                Nome = "",
                Email = "email_invalido",
                Telefone = "",
                DataNascimento = DateTime.MinValue,
                Senha = ""
            };

            
            var result = await _controller.CriarUsuarioComPessoa(registerDto, CancellationToken.None);

            
            result.Should().BeOfType<BadRequestObjectResult>();
            await _autenticacaoService.DidNotReceive().CriarUsuarioComPessoaAsync(Arg.Any<RegisterDto>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CriarUsuarioComPessoa_FalhaNoServico_DeveRetornarBadRequest()
        {
            
            var registerDto = new RegisterDto
            {
                Nome = "João Silva",
                Email = "joao@email.com",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Senha = "Senha123!"
            };

            var errors = new[] { "Email já existe" };
            _autenticacaoService.CriarUsuarioComPessoaAsync(registerDto, Arg.Any<CancellationToken>())
                .Returns((false, (object?)null, errors));

            
            var result = await _controller.CriarUsuarioComPessoa(registerDto, CancellationToken.None);

            
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult?.Value.Should().Be(errors);
        }

        [Fact]
        public async Task CriarUsuarioComPessoa_ExcecaoLancada_DeveRetornarBadRequest()
        {
            
            var registerDto = new RegisterDto
            {
                Nome = "João Silva",
                Email = "joao@email.com",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Senha = "Senha123!"
            };

            _autenticacaoService.When(x => x.CriarUsuarioComPessoaAsync(registerDto, Arg.Any<CancellationToken>()))
                .Do(x => throw new Exception("Erro interno"));

            
            var result = await _controller.CriarUsuarioComPessoa(registerDto, CancellationToken.None);

            
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task EfetuarLogin_DadosValidos_DeveRetornarOk()
        {
            
            var loginDto = new LoginDto("joao@email.com", "Senha123!");
            var expectedResponse = new AutenticacaoResponseDto 
            ( 
                Token: "token_jwt",
                Expiracao: DateTime.UtcNow.AddHours(1)
            );
            var result = Result<AutenticacaoResponseDto>.Ok(expectedResponse);

            _autenticacaoService.EfetuarLoginAsync(loginDto, Arg.Any<CancellationToken>())
                .Returns(result);

            
            var actionResult = await _controller.EfetuarLoginAsync(loginDto, CancellationToken.None);

            
            actionResult.Should().BeOfType<OkObjectResult>();
            var okResult = actionResult as OkObjectResult;
            okResult?.Value.Should().Be(result);
        }

        [Fact]
        public async Task EfetuarLogin_DadosInvalidos_DeveRetornarBadRequest()
        {
            
            var loginDto = new LoginDto("", "");

            
            var result = await _controller.EfetuarLoginAsync(loginDto, CancellationToken.None);

            
            result.Should().BeOfType<BadRequestObjectResult>();
            await _autenticacaoService.DidNotReceive().EfetuarLoginAsync(Arg.Any<LoginDto>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task EfetuarLogin_FalhaNoServico_DeveRetornarBadRequest()
        {
            
            var loginDto = new LoginDto("joao@email.com", "SenhaErrada");
            var errors = new[] { "Credenciais inválidas" };
            var result = Result<AutenticacaoResponseDto>.Fail(errors);

            _autenticacaoService.EfetuarLoginAsync(loginDto, Arg.Any<CancellationToken>())
                .Returns(result);

            
            var actionResult = await _controller.EfetuarLoginAsync(loginDto, CancellationToken.None);

            
            actionResult.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = actionResult as BadRequestObjectResult;
            badRequestResult?.Value.Should().Be(result.Errors);
        }

        [Fact]
        public async Task EfetuarLogin_ExcecaoLancada_DeveRetornarBadRequest()
        {
            
            var loginDto = new LoginDto("joao@email.com", "Senha123!");

            _autenticacaoService.When(x => x.EfetuarLoginAsync(loginDto, Arg.Any<CancellationToken>()))
                .Do(x => throw new Exception("Erro interno"));

            
            var result = await _controller.EfetuarLoginAsync(loginDto, CancellationToken.None);

            
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
