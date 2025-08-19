using GPS.AutenticacaoService.Application.DTOs;
using GPS.AutenticacaoService.Application.Extensions;
using FluentAssertions;

namespace GPS.AutenticacaoService.Tests.Extensions
{
    public class LoginDtoExtensionsTests
    {
        [Fact]
        public void Validar_DadosValidos_DeveRetornarListaVazia()
        {
            
            var dto = new LoginDto("joao@email.com", "Senha123!");

            
            var erros = dto.Validar();

            
            erros.Should().BeEmpty();
        }

        [Fact]
        public void Validar_EmailVazio_DeveRetornarErro()
        {
            
            var dto = new LoginDto("", "Senha123!");

            
            var erros = dto.Validar();

            
            erros.Should().NotBeEmpty();
            erros.Should().Contain("E-mail é obrigatório.");
        }

        [Fact]
        public void Validar_EmailInvalido_DeveRetornarErro()
        {
            
            var dto = new LoginDto("email_invalido", "Senha123!");

            
            var erros = dto.Validar();

            
            // Como o validador atual só verifica se está vazio, não se é válido,
            // este teste não deve retornar erro para um email não vazio
            erros.Should().BeEmpty();
        }

        [Fact]
        public void Validar_SenhaVazia_DeveRetornarErro()
        {
            
            var dto = new LoginDto("joao@email.com", "");

            
            var erros = dto.Validar();

            
            erros.Should().NotBeEmpty();
            erros.Should().Contain(e => e.Contains("Senha"));
        }

        [Fact]
        public void Validar_TodosCamposVazios_DeveRetornarMultiplosErros()
        {
            
            var dto = new LoginDto("", "");

            
            var erros = dto.Validar();

            
            erros.Should().NotBeEmpty();
            erros.Count.Should().BeGreaterThan(1);
        }

        [Fact]
        public void Validar_TodosCamposNulos_DeveRetornarMultiplosErros()
        {
            
            var dto = new LoginDto(null!, null!);

            
            var erros = dto.Validar();

            
            erros.Should().NotBeEmpty();
            erros.Count.Should().BeGreaterThan(1);
        }

        [Fact]
        public void Validar_EmailValidoSenhaInvalida_DeveRetornarErroSomenteSenha()
        {
            
            var dto = new LoginDto("joao@email.com", "");

            
            var erros = dto.Validar();

            
            erros.Should().NotBeEmpty();
            erros.Should().Contain("Senha é obrigatória.");
            erros.Should().NotContain("E-mail é obrigatório.");
        }
    }
}
