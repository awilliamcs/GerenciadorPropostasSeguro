using AutoMapper;
using GPS.PessoaService.Application.DTOs;
using GPS.PessoaService.Domain.Entidades;
using GPS.PessoaService.Domain.Interfaces;
using GPS.CrossCutting.Messaging;
using NSubstitute;
using FluentAssertions;

namespace GPS.PessoaService.Tests.Services
{
    public class PessoaServiceTests
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IMapper _mapper;
        private readonly IClienteMQ _clientMQ;
        private readonly GPS.PessoaService.Application.Services.PessoaService _pessoaService;

        public PessoaServiceTests()
        {
            _pessoaRepository = Substitute.For<IPessoaRepository>();
            _mapper = Substitute.For<IMapper>();
            _clientMQ = Substitute.For<IClienteMQ>();
            _pessoaService = new GPS.PessoaService.Application.Services.PessoaService(
                _pessoaRepository, _mapper, _clientMQ);
        }

        [Fact]
        public async Task SalvarAsync_DevePublicarMensagemNaFila()
        {
            
            var pessoaDto = new PessoaDto
            {
                IdPessoa = Guid.NewGuid(),
                Nome = "João Silva",
                Email = "joao@email.com",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            
            var act = () => _pessoaService.SalvarAsync(pessoaDto);

            
            await act.Should().NotThrowAsync();
            await _clientMQ.Received(1).PublicarMensagemParaFila<PessoaDto>(pessoaDto);
        }

        [Fact]
        public async Task ObterPorIdAsync_PessoaExiste_DeveRetornarPessoaDto()
        {
            
            var idPessoa = Guid.NewGuid();
            var pessoa = new Pessoa("João Silva", "joao@email.com", "11999999999", 
                new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            var pessoaDto = new PessoaDto
            {
                IdPessoa = idPessoa,
                Nome = "João Silva",
                Email = "joao@email.com",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            _pessoaRepository.ObterPorIdAsync(idPessoa, Arg.Any<CancellationToken>())
                .Returns(pessoa);
            _mapper.Map<PessoaDto>(pessoa).Returns(pessoaDto);

            
            var result = await _pessoaService.ObterPorIdAsync(idPessoa);

            
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(pessoaDto);
        }

        [Fact]
        public async Task ObterPorIdAsync_PessoaNaoExiste_DeveRetornarNull()
        {
            
            var idPessoa = Guid.NewGuid();
            _pessoaRepository.ObterPorIdAsync(idPessoa, Arg.Any<CancellationToken>())
                .Returns((Pessoa?)null);

            
            var result = await _pessoaService.ObterPorIdAsync(idPessoa);

            
            result.Should().BeNull();
        }
    }
}
