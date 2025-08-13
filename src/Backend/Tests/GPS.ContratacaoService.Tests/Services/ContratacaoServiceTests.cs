using AutoMapper;
using GPS.ContratacaoService.Application.DTOs;
using GPS.ContratacaoService.Domain.Entidades;
using GPS.ContratacaoService.Domain.Interfaces;
using GPS.CrossCutting.Messaging;
using NSubstitute;
using FluentAssertions;

namespace GPS.ContratacaoService.Tests.Services
{
    public class ContratacaoServiceTests
    {
        private readonly IContratacaoRepository _contratacaoRepository;
        private readonly IMapper _mapper;
        private readonly IClienteMQ _clientMQ;
        private readonly GPS.ContratacaoService.Application.Services.ContratacaoService _contratacaoService;

        public ContratacaoServiceTests()
        {
            _contratacaoRepository = Substitute.For<IContratacaoRepository>();
            _mapper = Substitute.For<IMapper>();
            _clientMQ = Substitute.For<IClienteMQ>();
            _contratacaoService = new GPS.ContratacaoService.Application.Services.ContratacaoService(
                _contratacaoRepository, _mapper, _clientMQ);
        }

        [Fact]
        public async Task SalvarAsync_DevePublicarMensagemNaFila()
        {
            
            var contratacaoDto = new ContratacaoDto
            {
                IdContratacao = Guid.NewGuid(),
                IdProposta = Guid.NewGuid(),
                DataContratacao = DateTime.UtcNow
            };

            
            var act = () => _contratacaoService.SalvarAsync(contratacaoDto);

            
            await act.Should().NotThrowAsync();
            await _clientMQ.Received(1).PublicarMensagemParaFila<ContratacaoDto>(contratacaoDto);
        }

        [Fact]
        public async Task ObterAsync_DeveRetornarContratacoesPaginadas()
        {
            
            var pagina = 1;
            var quantidadeItens = 10;
            var contratacoes = new List<Contratacao>
            {
                new Contratacao(Guid.NewGuid()),
                new Contratacao(Guid.NewGuid())
            };
            var total = 2;

            var contratacaoDtos = new List<ContratacaoDto>
            {
                new ContratacaoDto { IdContratacao = contratacoes[0].IdContratacao, IdProposta = contratacoes[0].IdProposta },
                new ContratacaoDto { IdContratacao = contratacoes[1].IdContratacao, IdProposta = contratacoes[1].IdProposta }
            };

            _contratacaoRepository.ObterAsync(pagina, quantidadeItens, Arg.Any<CancellationToken>())
                .Returns((contratacoes, total));
            _mapper.Map<List<ContratacaoDto>>(contratacoes).Returns(contratacaoDtos);

            
            var result = await _contratacaoService.ObterAsync(pagina, quantidadeItens);

            
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.Total.Should().Be(total);
            result.Pagina.Should().Be(pagina);
            result.QuantidadeItens.Should().Be(quantidadeItens);
            result.Items.Should().BeEquivalentTo(contratacaoDtos);
        }

        [Fact]
        public async Task ObterPorIdAsync_ContratacaoExiste_DeveRetornarContratacaoDto()
        {
            
            var idContratacao = Guid.NewGuid();
            var idProposta = Guid.NewGuid();
            var contratacao = new Contratacao(idProposta);
            var contratacaoDto = new ContratacaoDto
            {
                IdContratacao = idContratacao,
                IdProposta = idProposta,
                DataContratacao = DateTime.UtcNow
            };

            _contratacaoRepository.ObterPorIdAsync(idContratacao, Arg.Any<CancellationToken>())
                .Returns(contratacao);
            _mapper.Map<ContratacaoDto>(contratacao).Returns(contratacaoDto);

            
            var result = await _contratacaoService.ObterPorIdAsync(idContratacao);

            
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(contratacaoDto);
        }

        [Fact]
        public async Task ObterPorIdAsync_ContratacaoNaoExiste_DeveRetornarNull()
        {
            
            var idContratacao = Guid.NewGuid();
            _contratacaoRepository.ObterPorIdAsync(idContratacao, Arg.Any<CancellationToken>())
                .Returns((Contratacao?)null);

            
            var result = await _contratacaoService.ObterPorIdAsync(idContratacao);

            
            result.Should().BeNull();
        }
    }
}
