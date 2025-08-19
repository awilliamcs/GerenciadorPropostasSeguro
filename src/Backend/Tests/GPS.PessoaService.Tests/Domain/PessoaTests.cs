using GPS.PessoaService.Domain.Entidades;
using FluentAssertions;

namespace GPS.PessoaService.Tests.Domain
{
    public class PessoaTests
    {
        [Fact]
        public void Pessoa_CriadaComParametros_DeveArmazenarDadosCorretamente()
        {
            
            var nome = "João Silva";
            var email = "joao@email.com";
            var telefone = "11999999999";
            var dataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            
            var pessoa = new Pessoa(nome, email, telefone, dataNascimento);

            
            pessoa.Should().NotBeNull();
            pessoa.IdPessoa.Should().NotBe(Guid.Empty);
            pessoa.Nome.Should().Be(nome);
            pessoa.Email.Should().Be(email);
            pessoa.Telefone.Should().Be(telefone);
            pessoa.DataNascimento.Should().Be(dataNascimento);
        }

        [Fact]
        public void Pessoa_GeraIdUnico_CadaInstanciaDeveTermIdDiferente()
        {
            
            var nome = "João Silva";
            var email = "joao@email.com";
            var telefone = "11999999999";
            var dataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            
            var pessoa1 = new Pessoa(nome, email, telefone, dataNascimento);
            var pessoa2 = new Pessoa(nome, email, telefone, dataNascimento);

            
            pessoa1.IdPessoa.Should().NotBe(pessoa2.IdPessoa);
        }

        [Fact]
        public void Pessoa_PropriedadesSaoPrivateSet_NaoDevemSerAlteradasAposInstanciacao()
        {
            
            var nome = "João Silva";
            var email = "joao@email.com";
            var telefone = "11999999999";
            var dataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            
            var pessoa = new Pessoa(nome, email, telefone, dataNascimento);

            
            // Verifica se as propriedades têm setter privado
            var nomeProperty = typeof(Pessoa).GetProperty(nameof(Pessoa.Nome));
            var emailProperty = typeof(Pessoa).GetProperty(nameof(Pessoa.Email));
            var telefoneProperty = typeof(Pessoa).GetProperty(nameof(Pessoa.Telefone));
            var dataNascimentoProperty = typeof(Pessoa).GetProperty(nameof(Pessoa.DataNascimento));
            var idProperty = typeof(Pessoa).GetProperty(nameof(Pessoa.IdPessoa));

            nomeProperty?.SetMethod?.IsPrivate.Should().BeTrue();
            emailProperty?.SetMethod?.IsPrivate.Should().BeTrue();
            telefoneProperty?.SetMethod?.IsPrivate.Should().BeTrue();
            dataNascimentoProperty?.SetMethod?.IsPrivate.Should().BeTrue();
            idProperty?.SetMethod?.IsPrivate.Should().BeTrue();
        }

        [Fact]
        public void Pessoa_ComDadosMinimos_DeveSerCriadaCorretamente()
        {
            
            var nome = "A";
            var email = "a@b.co";
            var telefone = "12345678";
            var dataNascimento = DateTime.Today.AddYears(-18);

            
            var pessoa = new Pessoa(nome, email, telefone, dataNascimento);

            
            pessoa.Should().NotBeNull();
            pessoa.Nome.Should().Be(nome);
            pessoa.Email.Should().Be(email);
            pessoa.Telefone.Should().Be(telefone);
            pessoa.DataNascimento.Should().Be(dataNascimento);
        }

        [Fact]
        public void Pessoa_ComDadosMaximos_DeveSerCriadaCorretamente()
        {
            
            var nome = new string('A', 150);
            var email = new string('B', 140) + "@test.com";
            var telefone = "+5511999999999";
            var dataNascimento = DateTime.Today.AddYears(-25);

            
            var pessoa = new Pessoa(nome, email, telefone, dataNascimento);

            
            pessoa.Should().NotBeNull();
            pessoa.Nome.Should().Be(nome);
            pessoa.Email.Should().Be(email);
            pessoa.Telefone.Should().Be(telefone);
            pessoa.DataNascimento.Should().Be(dataNascimento);
        }
    }
}
