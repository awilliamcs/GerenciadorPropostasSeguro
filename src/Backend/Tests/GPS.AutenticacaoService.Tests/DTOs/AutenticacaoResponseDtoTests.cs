using GPS.AutenticacaoService.Application.DTOs;
using FluentAssertions;

namespace GPS.AutenticacaoService.Tests.DTOs
{
    public class AutenticacaoResponseDtoTests
    {
        [Fact]
        public void AutenticacaoResponseDto_CriadoComParametros_DeveArmazenarCorretamente()
        {
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
            var expiracao = DateTime.UtcNow.AddHours(1);

            var dto = new AutenticacaoResponseDto(token, expiracao);

            dto.Token.Should().Be(token);
            dto.Expiracao.Should().Be(expiracao);
        }

        [Fact]
        public void AutenticacaoResponseDto_DoisObjetosIguais_DevemSerIguais()
        {
            var token = "token_jwt";
            var expiracao = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            var dto1 = new AutenticacaoResponseDto(token, expiracao);
            var dto2 = new AutenticacaoResponseDto(token, expiracao);

            dto1.Should().Be(dto2);
            dto1.GetHashCode().Should().Be(dto2.GetHashCode());
        }

        [Fact]
        public void AutenticacaoResponseDto_DoisObjetosDiferentes_NaoDevemSerIguais()
        {
            var dto1 = new AutenticacaoResponseDto("token1", DateTime.UtcNow);
            var dto2 = new AutenticacaoResponseDto("token2", DateTime.UtcNow.AddHours(1));

            dto1.Should().NotBe(dto2);
        }

        [Fact]
        public void AutenticacaoResponseDto_CriadoComTokenVazio_DevePermitir()
        {
            var token = "";
            var expiracao = DateTime.UtcNow;

            var dto = new AutenticacaoResponseDto(token, expiracao);

            dto.Token.Should().Be(token);
            dto.Expiracao.Should().Be(expiracao);
        }

        [Fact]
        public void AutenticacaoResponseDto_ToString_DeveRetornarRepresentacaoCorreta()
        {
            var token = "token_teste";
            var expiracao = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            var dto = new AutenticacaoResponseDto(token, expiracao);

            var stringRepresentation = dto.ToString();

            stringRepresentation.Should().Contain("token_teste");
            stringRepresentation.Should().Contain("2025");
        }
    }
}
