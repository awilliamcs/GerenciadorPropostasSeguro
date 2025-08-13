using AutoMapper;
using GPS.PropostaService.Application.DTOs;
using GPS.PropostaService.Domain.Entidades;
using GPS.PropostaService.Domain.Interfaces;
using GPS.CrossCutting.Messaging;
using GPS.CrossCutting.Enums;
using NSubstitute;
using FluentAssertions;

namespace GPS.PropostaService.Tests.Services
{
    public class PropostaServiceTests
    {
        private readonly IPropostaRepository _propostaRepository;
        private readonly IMapper _mapper;
        private readonly IClienteMQ _clientMQ;
        private readonly GPS.PropostaService.Application.Services.PropostaService _propostaService;

        public PropostaServiceTests()
        {
            _propostaRepository = Substitute.For<IPropostaRepository>();
            _mapper = Substitute.For<IMapper>();
            _clientMQ = Substitute.For<IClienteMQ>();
            _propostaService = new GPS.PropostaService.Application.Services.PropostaService(
                _propostaRepository, _mapper, _clientMQ);
        }

        [Fact]
        public async Task SalvarAsync_DevePublicarMensagemNaFila()
        {
            
            var propostaDto = new PropostaDto
            {
                Id = Guid.NewGuid(),
                IdPessoa = Guid.NewGuid(),
                Tipo = TipoProposta.Vida,
                Valor = 50000m,
                Status = StatusProposta.EmAnalise,
                DataSolicitacao = DateTime.UtcNow
            };

            
            var act = () => _propostaService.SalvarAsync(propostaDto);

            
            await act.Should().NotThrowAsync();
            await _clientMQ.Received(1).PublicarMensagemParaFila<PropostaDto>(propostaDto);
        }

        [Fact]
        public async Task ObterAsync_DeveRetornarPropostasPaginadas()
        {
            
            var pagina = 1;
            var quantidadeItens = 10;
            var propostas = new List<Proposta>
            {
                new Proposta(Guid.NewGuid(), TipoProposta.Vida, 50000m),
                new Proposta(Guid.NewGuid(), TipoProposta.Veicular, 30000m)
            };
            var total = 2;

            var propostaDtos = new List<PropostaDto>
            {
                new PropostaDto { Id = propostas[0].IdProposta, IdPessoa = propostas[0].IdPessoa, Tipo = propostas[0].Tipo, Valor = propostas[0].Valor },
                new PropostaDto { Id = propostas[1].IdProposta, IdPessoa = propostas[1].IdPessoa, Tipo = propostas[1].Tipo, Valor = propostas[1].Valor }
            };

            _propostaRepository.ObterAsync(pagina, quantidadeItens, Arg.Any<CancellationToken>())
                .Returns((propostas, total));
            _mapper.Map<List<PropostaDto>>(propostas).Returns(propostaDtos);

            
            var result = await _propostaService.ObterAsync(pagina, quantidadeItens);

            
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.Total.Should().Be(total);
            result.Pagina.Should().Be(pagina);
            result.QuantidadeItens.Should().Be(quantidadeItens);
            result.Items.Should().BeEquivalentTo(propostaDtos);
        }

        [Fact]
        public async Task ObterPorIdAsync_PropostaExiste_DeveRetornarPropostaDto()
        {
            
            var idProposta = Guid.NewGuid();
            var idPessoa = Guid.NewGuid();
            var proposta = new Proposta(idPessoa, TipoProposta.Vida, 50000m);
            var propostaDto = new PropostaDto
            {
                Id = idProposta,
                IdPessoa = idPessoa,
                Tipo = TipoProposta.Vida,
                Valor = 50000m,
                Status = StatusProposta.EmAnalise,
                DataSolicitacao = DateTime.UtcNow
            };

            _propostaRepository.ObterPorIdAsync(idProposta, Arg.Any<CancellationToken>())
                .Returns(proposta);
            _mapper.Map<PropostaDto>(proposta).Returns(propostaDto);

            
            var result = await _propostaService.ObterPorIdAsync(idProposta);

            
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(propostaDto);
        }

        [Fact]
        public async Task ObterPorIdAsync_PropostaNaoExiste_DeveRetornarNull()
        {
            
            var idProposta = Guid.NewGuid();
            _propostaRepository.ObterPorIdAsync(idProposta, Arg.Any<CancellationToken>())
                .Returns((Proposta?)null);

            
            var result = await _propostaService.ObterPorIdAsync(idProposta);

            
            result.Should().BeNull();
        }
    }
}
