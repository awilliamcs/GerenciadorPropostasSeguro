using GPS.AutenticacaoService.Application.DTOs;
using FluentAssertions;

namespace GPS.AutenticacaoService.Tests.DTOs
{
    public class RegisterDtoTests
    {
        [Fact]
        public void RegisterDto_PropriedadesDefinidas_DeveArmazenarCorretamente()
        {
            
            var nome = "João Silva";
            var email = "joao@email.com";
            var telefone = "11999999999";
            var dataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var senha = "Senha123!";

            
            var dto = new RegisterDto
            {
                Nome = nome,
                Email = email,
                Telefone = telefone,
                DataNascimento = dataNascimento,
                Senha = senha
            };

            
            dto.Nome.Should().Be(nome);
            dto.Email.Should().Be(email);
            dto.Telefone.Should().Be(telefone);
            dto.DataNascimento.Should().Be(dataNascimento);
            dto.Senha.Should().Be(senha);
        }

        [Fact]
        public void RegisterDto_InstanciaPadrao_DeveTerValoresPadrao()
        {
            
            var dto = new RegisterDto();

            
            dto.Nome.Should().BeNull();
            dto.Email.Should().BeNull();
            dto.Telefone.Should().BeNull();
            dto.DataNascimento.Should().Be(default(DateTime));
            dto.Senha.Should().BeNull();
        }
    }
}
