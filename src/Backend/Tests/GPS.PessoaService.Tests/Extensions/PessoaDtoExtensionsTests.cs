using GPS.PessoaService.Application.DTOs;
using GPS.PessoaService.Application.Extensions;
using FluentAssertions;

namespace GPS.PessoaService.Tests.Extensions
{
    public class PessoaDtoExtensionsTests
    {
        [Fact]
        public void Validar_DadosValidos_DeveRetornarListaVazia()
        {
            
            var dto = new PessoaDto
            {
                IdPessoa = Guid.NewGuid(),
                Nome = "João Silva",
                Email = "joao@email.com",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            
            var erros = dto.Validar();

            
            erros.Should().BeEmpty();
        }

        [Fact]
        public void Validar_NomeVazio_DeveRetornarErro()
        {
            
            var dto = new PessoaDto
            {
                IdPessoa = Guid.NewGuid(),
                Nome = "",
                Email = "joao@email.com",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            
            var erros = dto.Validar();

            
            erros.Should().NotBeEmpty();
            erros.Should().Contain("O nome é obrigatório.");
        }

        [Fact]
        public void Validar_NomeMuitoLongo_DeveRetornarErro()
        {
            
            var dto = new PessoaDto
            {
                IdPessoa = Guid.NewGuid(),
                Nome = new string('A', 151), // 151 caracteres
                Email = "joao@email.com",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            
            var erros = dto.Validar();

            
            erros.Should().NotBeEmpty();
            erros.Should().Contain("O nome não pode ultrapassar 150 caracteres.");
        }

        [Fact]
        public void Validar_EmailVazio_DeveRetornarErro()
        {
            
            var dto = new PessoaDto
            {
                IdPessoa = Guid.NewGuid(),
                Nome = "João Silva",
                Email = "",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            
            var erros = dto.Validar();

            
            erros.Should().NotBeEmpty();
            erros.Should().Contain("O e-mail é obrigatório.");
        }

        [Fact]
        public void Validar_EmailInvalido_DeveRetornarErro()
        {
            
            var dto = new PessoaDto
            {
                IdPessoa = Guid.NewGuid(),
                Nome = "João Silva",
                Email = "email_invalido",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            
            var erros = dto.Validar();

            
            erros.Should().NotBeEmpty();
            erros.Should().Contain("O e-mail informado não é válido.");
        }

        [Fact]
        public void Validar_EmailMuitoLongo_DeveRetornarErro()
        {
            
            var dto = new PessoaDto
            {
                IdPessoa = Guid.NewGuid(),
                Nome = "João Silva",
                Email = new string('A', 145) + "@email.com", // 145 + 10 = 155 caracteres (acima de 150)
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            
            var erros = dto.Validar();

            
            erros.Should().NotBeEmpty();
            erros.Should().Contain("O e-mail não pode ultrapassar 150 caracteres.");
        }

        [Fact]
        public void Validar_TelefoneVazio_DeveRetornarErro()
        {
            
            var dto = new PessoaDto
            {
                IdPessoa = Guid.NewGuid(),
                Nome = "João Silva",
                Email = "joao@email.com",
                Telefone = "",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            
            var erros = dto.Validar();

            
            erros.Should().NotBeEmpty();
            erros.Should().Contain("O telefone é obrigatório.");
        }

        [Fact]
        public void Validar_TelefoneInvalido_DeveRetornarErro()
        {
            
            var dto = new PessoaDto
            {
                IdPessoa = Guid.NewGuid(),
                Nome = "João Silva",
                Email = "joao@email.com",
                Telefone = "abc123",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            
            var erros = dto.Validar();

            
            erros.Should().NotBeEmpty();
            erros.Should().Contain("O telefone deve conter apenas números e, opcionalmente, o código do país.");
        }

        [Fact]
        public void Validar_DataNascimentoFutura_DeveRetornarErro()
        {
            
            var dto = new PessoaDto
            {
                IdPessoa = Guid.NewGuid(),
                Nome = "João Silva",
                Email = "joao@email.com",
                Telefone = "11999999999",
                DataNascimento = DateTime.Today.AddDays(1)
            };

            
            var erros = dto.Validar();

            
            erros.Should().NotBeEmpty();
            erros.Should().Contain("A data de nascimento deve ser no passado.");
        }

        [Fact]
        public void Validar_DataNascimentoMuitoAntiga_DeveRetornarErro()
        {
            
            var dto = new PessoaDto
            {
                IdPessoa = Guid.NewGuid(),
                Nome = "João Silva",
                Email = "joao@email.com",
                Telefone = "11999999999",
                DataNascimento = DateTime.Today.AddYears(-121)
            };

            
            var erros = dto.Validar();

            
            erros.Should().NotBeEmpty();
            erros.Should().Contain("A data de nascimento não pode indicar mais de 120 anos de idade.");
        }

        [Fact]
        public void Validar_TodosCamposInvalidos_DeveRetornarMultiplosErros()
        {
            
            var dto = new PessoaDto
            {
                IdPessoa = Guid.NewGuid(),
                Nome = "",
                Email = "email_invalido",
                Telefone = "abc123",
                DataNascimento = DateTime.Today.AddDays(1)
            };

            
            var erros = dto.Validar();

            
            erros.Should().NotBeEmpty();
            erros.Count.Should().BeGreaterThan(3);
        }
    }
}
