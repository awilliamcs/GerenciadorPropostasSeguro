using GPS.ContratacaoService.Api.Controllers;
using GPS.ContratacaoService.Application.DTOs;
using GPS.ContratacaoService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using FluentAssertions;

namespace GPS.ContratacaoService.Tests.Controllers
{
    public class ContratacaoControllerTests
    {
        private readonly IContratacaoService _contratacaoService;
        private readonly ContratacaoController _controller;

        public ContratacaoControllerTests()
        {
            _contratacaoService = Substitute.For<IContratacaoService>();
            _controller = new ContratacaoController(_contratacaoService);
        }

        [Fact]
        public async Task SalvarAsync_ContratacaoValida_DeveRetornarOk()
        {
            
            var contratacaoDto = new ContratacaoDto
            {
                IdContratacao = Guid.NewGuid(),
                IdProposta = Guid.NewGuid(),
                DataContratacao = DateTime.UtcNow
            };

            
            var result = await _controller.SalvarAsync(contratacaoDto);

            
            result.Should().BeOfType<OkResult>();
            await _contratacaoService.Received(1).SalvarAsync(contratacaoDto);
        }

        [Fact]
        public async Task SalvarAsync_ExcecaoLancada_DeveRetornarBadRequest()
        {
            
            var contratacaoDto = new ContratacaoDto
            {
                IdContratacao = Guid.NewGuid(),
                IdProposta = Guid.NewGuid(),
                DataContratacao = DateTime.UtcNow
            };

            _contratacaoService.When(x => x.SalvarAsync(contratacaoDto))
                .Do(x => throw new Exception("Erro teste"));

            
            var result = await _controller.SalvarAsync(contratacaoDto);

            
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ObterAsync_DeveRetornarOkComDados()
        {
            
            var response = new ContratacaoListResponseDto
            {
                Items = new List<ContratacaoDto>
                {
                    new ContratacaoDto { IdContratacao = Guid.NewGuid(), IdProposta = Guid.NewGuid() }
                },
                Total = 1,
                Pagina = 1,
                QuantidadeItens = 10
            };

            _contratacaoService.ObterAsync(1, 10).Returns(response);

            
            var result = await _controller.ObterAsync(1, 10);

            
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult?.Value.Should().Be(response);
        }

        [Fact]
        public async Task ObterAsync_ExcecaoLancada_DeveRetornarBadRequest()
        {
            
            _contratacaoService.When(x => x.ObterAsync(1, 10))
                .Do(x => throw new Exception("Erro teste"));

            
            var result = await _controller.ObterAsync(1, 10);

            
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ObterPorIdAsync_ContratacaoExiste_DeveRetornarOkComContratacao()
        {
            
            var idContratacao = Guid.NewGuid();
            var contratacaoDto = new ContratacaoDto
            {
                IdContratacao = idContratacao,
                IdProposta = Guid.NewGuid(),
                DataContratacao = DateTime.UtcNow
            };

            _contratacaoService.ObterPorIdAsync(idContratacao).Returns(contratacaoDto);

            
            var result = await _controller.ObterPorIdAsync(idContratacao);

            
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult?.Value.Should().Be(contratacaoDto);
        }

        [Fact]
        public async Task ObterPorIdAsync_ContratacaoNaoExiste_DeveRetornarNotFound()
        {
            
            var idContratacao = Guid.NewGuid();
            _contratacaoService.ObterPorIdAsync(idContratacao).Returns((ContratacaoDto?)null);

            
            var result = await _controller.ObterPorIdAsync(idContratacao);

            
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult?.Value.Should().Be("Contratacao não encontrada");
        }

        [Fact]
        public async Task ObterPorIdAsync_ExcecaoLancada_DeveRetornarBadRequest()
        {
            
            var idContratacao = Guid.NewGuid();
            _contratacaoService.When(x => x.ObterPorIdAsync(idContratacao))
                .Do(x => throw new Exception("Erro teste"));

            
            var result = await _controller.ObterPorIdAsync(idContratacao);

            
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
