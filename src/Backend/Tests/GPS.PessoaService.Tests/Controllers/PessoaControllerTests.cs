using GPS.PessoaService.Api.Controllers;
using GPS.PessoaService.Application.DTOs;
using GPS.PessoaService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using FluentAssertions;

namespace GPS.PessoaService.Tests.Controllers
{
    public class PessoaControllerTests
    {
        private readonly IPessoaService _pessoaService;
        private readonly PessoaController _controller;

        public PessoaControllerTests()
        {
            _pessoaService = Substitute.For<IPessoaService>();
            _controller = new PessoaController(_pessoaService);
        }

        [Fact]
        public async Task SalvarAsync_PessoaValida_DeveRetornarOk()
        {
            
            var pessoaDto = new PessoaDto
            {
                IdPessoa = Guid.NewGuid(),
                Nome = "João Silva",
                Email = "joao@email.com",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            
            var result = await _controller.SalvarAsync(pessoaDto);

            
            result.Should().BeOfType<OkResult>();
            await _pessoaService.Received(1).SalvarAsync(pessoaDto);
        }

        [Fact]
        public async Task SalvarAsync_ExcecaoLancada_DeveRetornarBadRequest()
        {
            
            var pessoaDto = new PessoaDto
            {
                IdPessoa = Guid.NewGuid(),
                Nome = "João Silva",
                Email = "joao@email.com",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            _pessoaService.When(x => x.SalvarAsync(pessoaDto))
                .Do(x => throw new Exception("Erro teste"));

            
            var result = await _controller.SalvarAsync(pessoaDto);

            
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ObterPorIdAsync_PessoaExiste_DeveRetornarOkComPessoa()
        {
            
            var idPessoa = Guid.NewGuid();
            var pessoaDto = new PessoaDto
            {
                IdPessoa = idPessoa,
                Nome = "João Silva",
                Email = "joao@email.com",
                Telefone = "11999999999",
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            _pessoaService.ObterPorIdAsync(idPessoa).Returns(pessoaDto);

            
            var result = await _controller.ObterPorIdAsync(idPessoa);

            
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult?.Value.Should().Be(pessoaDto);
        }

        [Fact]
        public async Task ObterPorIdAsync_PessoaNaoExiste_DeveRetornarNotFound()
        {
            
            var idPessoa = Guid.NewGuid();
            _pessoaService.ObterPorIdAsync(idPessoa).Returns((PessoaDto?)null);

            
            var result = await _controller.ObterPorIdAsync(idPessoa);

            
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult?.Value.Should().Be("Pessoa não encontrada");
        }

        [Fact]
        public async Task ObterPorIdAsync_ExcecaoLancada_DeveRetornarBadRequest()
        {
            
            var idPessoa = Guid.NewGuid();
            _pessoaService.When(x => x.ObterPorIdAsync(idPessoa))
                .Do(x => throw new Exception("Erro teste"));

            
            var result = await _controller.ObterPorIdAsync(idPessoa);

            
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
