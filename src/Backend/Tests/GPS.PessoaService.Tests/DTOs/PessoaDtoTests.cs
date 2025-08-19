using GPS.PessoaService.Application.DTOs;
using FluentAssertions;

namespace GPS.PessoaService.Tests.DTOs
{
    public class PessoaDtoTests
    {
        [Fact]
        public void PessoaDto_PropriedadesDefinidas_DeveArmazenarCorretamente()
        {
            var idPessoa = Guid.NewGuid();
            var nome = "João Silva";
            var email = "joao@email.com";
            var telefone = "11999999999";
            var dataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            
            var dto = new PessoaDto
            {
                IdPessoa = idPessoa,
                Nome = nome,
                Email = email,
                Telefone = telefone,
                DataNascimento = dataNascimento
            };

            
            dto.IdPessoa.Should().Be(idPessoa);
            dto.Nome.Should().Be(nome);
            dto.Email.Should().Be(email);
            dto.Telefone.Should().Be(telefone);
            dto.DataNascimento.Should().Be(dataNascimento);
        }

        [Fact]
        public void PessoaDto_InstanciaPadrao_DeveTerValoresPadrao()
        {
            var dto = new PessoaDto();

            dto.IdPessoa.Should().Be(Guid.Empty);
            dto.Nome.Should().BeNull();
            dto.Email.Should().BeNull();
            dto.Telefone.Should().BeNull();
            dto.DataNascimento.Should().Be(default(DateTime));
        }

        [Fact]
        public void PessoaDto_ComDadosCompletos_DeveRepresentarUmaPessoaValida()
        {
            var dto = new PessoaDto
            {
                IdPessoa = Guid.NewGuid(),
                Nome = "Maria Santos",
                Email = "maria@empresa.com.br",
                Telefone = "+5511987654321",
                DataNascimento = new DateTime(1985, 6, 15, 0, 0, 0, DateTimeKind.Utc)
            };

            dto.Should().NotBeNull();
            dto.IdPessoa.Should().NotBe(Guid.Empty);
            dto.Nome.Should().NotBeNullOrWhiteSpace();
            dto.Email.Should().NotBeNullOrWhiteSpace();
            dto.Telefone.Should().NotBeNullOrWhiteSpace();
            dto.DataNascimento.Should().BeBefore(DateTime.Today);
        }

        [Fact]
        public void PessoaDto_PropriedadesPublicas_DevemTerGettersESetters()
        {
            var dtoType = typeof(PessoaDto);

            var idPessoaProperty = dtoType.GetProperty(nameof(PessoaDto.IdPessoa));
            var nomeProperty = dtoType.GetProperty(nameof(PessoaDto.Nome));
            var emailProperty = dtoType.GetProperty(nameof(PessoaDto.Email));
            var telefoneProperty = dtoType.GetProperty(nameof(PessoaDto.Telefone));
            var dataNascimentoProperty = dtoType.GetProperty(nameof(PessoaDto.DataNascimento));

            idPessoaProperty.Should().NotBeNull();
            idPessoaProperty!.CanRead.Should().BeTrue();
            idPessoaProperty.CanWrite.Should().BeTrue();

            nomeProperty.Should().NotBeNull();
            nomeProperty!.CanRead.Should().BeTrue();
            nomeProperty.CanWrite.Should().BeTrue();

            emailProperty.Should().NotBeNull();
            emailProperty!.CanRead.Should().BeTrue();
            emailProperty.CanWrite.Should().BeTrue();

            telefoneProperty.Should().NotBeNull();
            telefoneProperty!.CanRead.Should().BeTrue();
            telefoneProperty.CanWrite.Should().BeTrue();

            dataNascimentoProperty.Should().NotBeNull();
            dataNascimentoProperty!.CanRead.Should().BeTrue();
            dataNascimentoProperty.CanWrite.Should().BeTrue();
        }
    }
}
