using GPS.ContratacaoService.Domain.Entidades;
using GPS.ContratacaoService.Infrastructure;
using GPS.ContratacaoService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace GPS.ContratacaoService.Tests.Repositories
{
    public class ContratacaoRepositoryTests : IDisposable
    {
        private readonly ContratacaoDbContext _context;
        private readonly ContratacaoRepository _repository;

        public ContratacaoRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ContratacaoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ContratacaoDbContext(options);
            _repository = new ContratacaoRepository(_context);
        }

        [Fact]
        public async Task SalvarAsync_NovaContratacao_DeveAdicionarContratacao()
        {
            
            var idProposta = Guid.NewGuid();
            var contratacao = new Contratacao(idProposta);

            
            var result = await _repository.SalvarAsync(contratacao);

            
            result.Should().NotBeNull();
            result.IdContratacao.Should().Be(contratacao.IdContratacao);
            result.IdProposta.Should().Be(idProposta);

            var savedContratacao = await _context.Contratacoes.FirstOrDefaultAsync(c => c.IdContratacao == contratacao.IdContratacao);
            savedContratacao.Should().NotBeNull();
        }

        [Fact]
        public async Task SalvarAsync_ContratacaoExistente_DeveAtualizarContratacao()
        {
            
            var idProposta = Guid.NewGuid();
            var contratacao = new Contratacao(idProposta);
            
            // Primeiro salva a contratação
            await _context.Contratacoes.AddAsync(contratacao);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var result = await _repository.SalvarAsync(contratacao);
            
            result.Should().NotBeNull();
            result.IdContratacao.Should().Be(contratacao.IdContratacao);

            var count = await _context.Contratacoes.CountAsync(c => c.IdContratacao == contratacao.IdContratacao);
            count.Should().Be(1); // Deve haver apenas uma contratação com este ID
        }

        [Fact]
        public async Task ObterPorIdAsync_ContratacaoExiste_DeveRetornarContratacao()
        {
            
            var idProposta = Guid.NewGuid();
            var contratacao = new Contratacao(idProposta);
            await _context.Contratacoes.AddAsync(contratacao);
            await _context.SaveChangesAsync();

            
            var result = await _repository.ObterPorIdAsync(contratacao.IdContratacao);

            
            result.Should().NotBeNull();
            result!.IdContratacao.Should().Be(contratacao.IdContratacao);
            result.IdProposta.Should().Be(idProposta);
        }

        [Fact]
        public async Task ObterPorIdAsync_ContratacaoNaoExiste_DeveRetornarNull()
        {
            
            var idContratacao = Guid.NewGuid();

            
            var result = await _repository.ObterPorIdAsync(idContratacao);

            
            result.Should().BeNull();
        }

        [Fact]
        public async Task ObterAsync_DeveRetornarContratacoesPaginadas()
        {
            
            var contratacoes = new List<Contratacao>
            {
                new Contratacao(Guid.NewGuid()),
                new Contratacao(Guid.NewGuid()),
                new Contratacao(Guid.NewGuid())
            };

            await _context.Contratacoes.AddRangeAsync(contratacoes);
            await _context.SaveChangesAsync();

            
            var (items, total) = await _repository.ObterAsync(1, 2);

            
            items.Should().HaveCount(2);
            total.Should().Be(3);
            items.Should().BeInDescendingOrder(c => c.DataContratacao);
        }

        [Fact]
        public async Task ObterAsync_PaginaInvalida_DeveUsarPagina1()
        {
            
            var contratacao = new Contratacao(Guid.NewGuid());
            await _context.Contratacoes.AddAsync(contratacao);
            await _context.SaveChangesAsync();

            
            var (items, total) = await _repository.ObterAsync(0, 10); // Página inválida

            
            items.Should().HaveCount(1);
            total.Should().Be(1);
        }

        [Fact]
        public async Task ObterAsync_QuantidadeItensInvalida_DeveUsar10Items()
        {
            
            var contratacoes = Enumerable.Range(1, 15)
                .Select(_ => new Contratacao(Guid.NewGuid()))
                .ToList();

            await _context.Contratacoes.AddRangeAsync(contratacoes);
            await _context.SaveChangesAsync();

            
            var (items, total) = await _repository.ObterAsync(1, 0); // Quantidade inválida

            
            items.Should().HaveCount(10); // Deve usar o padrão de 10 itens
            total.Should().Be(15);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
            }
        }
    }
}
