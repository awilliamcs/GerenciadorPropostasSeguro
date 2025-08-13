using GPS.PropostaService.Api.Controllers;
using GPS.PropostaService.Application.DTOs;
using GPS.PropostaService.Application.Interfaces;
using GPS.CrossCutting.Enums;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using FluentAssertions;

namespace GPS.PropostaService.Tests.Controllers
{
    public class PropostaControllerTests
    {
        private readonly IPropostaService _propostaService;
        private readonly PropostaController _controller;

        public PropostaControllerTests()
        {
            _propostaService = Substitute.For<IPropostaService>();
            _controller = new PropostaController(_propostaService);
        }

        [Fact]
        public async Task SalvarAsync_PropostaValida_DeveRetornarOk()
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

            
            var result = await _controller.SalvarAsync(propostaDto);

            
            result.Should().BeOfType<OkResult>();
            await _propostaService.Received(1).SalvarAsync(propostaDto);
        }

        [Fact]
        public async Task SalvarAsync_ExcecaoLancada_DeveRetornarBadRequest()
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

            _propostaService.When(x => x.SalvarAsync(propostaDto))
                .Do(x => throw new Exception("Erro teste"));

            
            var result = await _controller.SalvarAsync(propostaDto);

            
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ObterAsync_DeveRetornarOkComDados()
        {
            
            var response = new PropostaListResponseDto
            {
                Items = new List<PropostaDto>
                {
                    new PropostaDto { Id = Guid.NewGuid(), IdPessoa = Guid.NewGuid(), Tipo = TipoProposta.Vida, Valor = 50000m }
                },
                Total = 1,
                Pagina = 1,
                QuantidadeItens = 10
            };

            _propostaService.ObterAsync(1, 10).Returns(response);

            
            var result = await _controller.ObterAsync(1, 10);

            
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult?.Value.Should().Be(response);
        }

        [Fact]
        public async Task ObterAsync_ExcecaoLancada_DeveRetornarBadRequest()
        {
            
            _propostaService.When(x => x.ObterAsync(1, 10))
                .Do(x => throw new Exception("Erro teste"));

            
            var result = await _controller.ObterAsync(1, 10);

            
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ObterPorIdAsync_PropostaExiste_DeveRetornarOkComProposta()
        {
            
            var idProposta = Guid.NewGuid();
            var propostaDto = new PropostaDto
            {
                Id = idProposta,
                IdPessoa = Guid.NewGuid(),
                Tipo = TipoProposta.Vida,
                Valor = 50000m,
                Status = StatusProposta.EmAnalise,
                DataSolicitacao = DateTime.UtcNow
            };

            _propostaService.ObterPorIdAsync(idProposta).Returns(propostaDto);

            
            var result = await _controller.ObterPorIdAsync(idProposta);

            
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult?.Value.Should().Be(propostaDto);
        }

        [Fact]
        public async Task ObterPorIdAsync_PropostaNaoExiste_DeveRetornarNotFound()
        {
            
            var idProposta = Guid.NewGuid();
            _propostaService.ObterPorIdAsync(idProposta).Returns((PropostaDto?)null);

            
            var result = await _controller.ObterPorIdAsync(idProposta);

            
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult?.Value.Should().Be("Proposta não encontrada");
        }

        [Fact]
        public async Task ObterPorIdAsync_ExcecaoLancada_DeveRetornarBadRequest()
        {
            
            var idProposta = Guid.NewGuid();
            _propostaService.When(x => x.ObterPorIdAsync(idProposta))
                .Do(x => throw new Exception("Erro teste"));

            
            var result = await _controller.ObterPorIdAsync(idProposta);

            
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
