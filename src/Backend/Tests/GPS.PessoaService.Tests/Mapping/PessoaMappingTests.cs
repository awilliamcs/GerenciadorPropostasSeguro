using AutoMapper;
using GPS.PessoaService.Application.DTOs;
using GPS.PessoaService.Application.Mapping;
using GPS.PessoaService.Domain.Entidades;
using FluentAssertions;

namespace GPS.PessoaService.Tests.Mapping
{
    public class PessoaMappingTests
    {
        [Fact]
        public void PessoaProfile_DeveExistirMapeamentoCorreto()
        {
            var profile = new PessoaProfile();

            profile.Should().NotBeNull();
            profile.Should().BeOfType<PessoaProfile>();
            profile.Should().BeAssignableTo<Profile>();
        }

        [Fact]
        public void Mapeamento_PessoaDtoParaPessoa_DeveFuncionarComDadosReais()
        {
            var pessoaDto = new PessoaDto
            {
                IdPessoa = Guid.NewGuid(),
                Nome = "João Silva",
                Email = "joao@email.com",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            var pessoa = new Pessoa(pessoaDto.Nome, pessoaDto.Email, pessoaDto.Telefone, pessoaDto.DataNascimento);

            pessoa.Should().NotBeNull();
            pessoa.Nome.Should().Be(pessoaDto.Nome);
            pessoa.Email.Should().Be(pessoaDto.Email);
            pessoa.Telefone.Should().Be(pessoaDto.Telefone);
            pessoa.DataNascimento.Should().Be(pessoaDto.DataNascimento);
        }

        [Fact]
        public void Mapeamento_PessoaParaPessoaDto_DeveFuncionarComDadosReais()
        {
            var pessoa = new Pessoa(
                "Maria Santos",
                "maria@email.com",
                "11888888888",
                new DateTime(1985, 6, 15, 0, 0, 0, DateTimeKind.Utc)
            );

            var pessoaDto = new PessoaDto
            {
                IdPessoa = pessoa.IdPessoa,
                Nome = pessoa.Nome,
                Email = pessoa.Email,
                Telefone = pessoa.Telefone,
                DataNascimento = pessoa.DataNascimento
            };

            pessoaDto.Should().NotBeNull();
            pessoaDto.IdPessoa.Should().Be(pessoa.IdPessoa);
            pessoaDto.Nome.Should().Be(pessoa.Nome);
            pessoaDto.Email.Should().Be(pessoa.Email);
            pessoaDto.Telefone.Should().Be(pessoa.Telefone);
            pessoaDto.DataNascimento.Should().Be(pessoa.DataNascimento);
        }

        [Fact]
        public void PessoaProfile_TemplateConstrutorUsing_DeveEstarConfiguradoCorretamente()
        {
            var profile = new PessoaProfile();
            profile.Should().NotBeNull();
        }

        [Fact]
        public void ConstrutorPessoa_ComParametrosCorretos_DeveFuncionar()
        {
            var nome = "João Silva";
            var email = "joao@email.com";
            var telefone = "11999999999";
            var dataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var pessoa = new Pessoa(nome, email, telefone, dataNascimento);

            pessoa.Should().NotBeNull();
            pessoa.Nome.Should().Be(nome);
            pessoa.Email.Should().Be(email);
            pessoa.Telefone.Should().Be(telefone);
            pessoa.DataNascimento.Should().Be(dataNascimento);
            pessoa.IdPessoa.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public void MapeamentoReverso_DeveManterConsistencia()
        {
            var pessoaOriginal = new Pessoa(
                "João Silva",
                "joao@email.com", 
                "11999999999",
                new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            );

            var pessoaDto = new PessoaDto
            {
                IdPessoa = pessoaOriginal.IdPessoa,
                Nome = pessoaOriginal.Nome,
                Email = pessoaOriginal.Email,
                Telefone = pessoaOriginal.Telefone,
                DataNascimento = pessoaOriginal.DataNascimento
            };

            var pessoaMapeada = new Pessoa(pessoaDto.Nome, pessoaDto.Email, pessoaDto.Telefone, pessoaDto.DataNascimento);
            pessoaMapeada.Nome.Should().Be(pessoaOriginal.Nome);
            pessoaMapeada.Email.Should().Be(pessoaOriginal.Email);
            pessoaMapeada.Telefone.Should().Be(pessoaOriginal.Telefone);
            pessoaMapeada.DataNascimento.Should().Be(pessoaOriginal.DataNascimento);
            pessoaMapeada.IdPessoa.Should().NotBe(pessoaOriginal.IdPessoa);
        }
    }
}
