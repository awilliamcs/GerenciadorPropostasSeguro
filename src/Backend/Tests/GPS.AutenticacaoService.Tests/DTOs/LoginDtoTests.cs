using GPS.AutenticacaoService.Application.DTOs;
using FluentAssertions;

namespace GPS.AutenticacaoService.Tests.DTOs
{
    public class LoginDtoTests
    {
        [Fact]
        public void LoginDto_CriadoComParametros_DeveArmazenarCorretamente()
        {
            var email = "joao@email.com";
            var senha = "Senha123!";

            var dto = new LoginDto(email, senha);

            dto.Email.Should().Be(email);
            dto.Senha.Should().Be(senha);
        }

        [Fact]
        public void LoginDto_CriadoComValoresNulos_DevePermitirValoresNulos()
        {
            
            var dto = new LoginDto(null!, null!);

            dto.Email.Should().BeNull();
            dto.Senha.Should().BeNull();
        }

        [Fact]
        public void LoginDto_CriadoComStringsVazias_DeveArmazenarCorretamente()
        {
            
            var dto = new LoginDto("", "");

            dto.Email.Should().Be("");
            dto.Senha.Should().Be("");
        }

        [Fact]
        public void LoginDto_DoisObjetosIguais_DevemSerIguais()
        {
            var email = "joao@email.com";
            var senha = "Senha123!";
            var dto1 = new LoginDto(email, senha);
            var dto2 = new LoginDto(email, senha);

            dto1.Should().Be(dto2);
            dto1.GetHashCode().Should().Be(dto2.GetHashCode());
        }

        [Fact]
        public void LoginDto_DoisObjetosDiferentes_NaoDevemSerIguais()
        {
            var dto1 = new LoginDto("joao@email.com", "Senha123!");
            var dto2 = new LoginDto("maria@email.com", "OutraSenha!");

            dto1.Should().NotBe(dto2);
        }
    }
}
