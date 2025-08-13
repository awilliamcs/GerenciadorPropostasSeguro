using GPS.PropostaService.Domain.Entidades;
using GPS.PropostaService.Infrastructure;
using GPS.PropostaService.Infrastructure.Repositories;
using GPS.CrossCutting.Enums;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace GPS.PropostaService.Tests.Repositories
{
    public class PropostaRepositoryTests : IDisposable
    {
        private readonly PropostaDbContext _context;
        private readonly PropostaRepository _repository;

        public PropostaRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<PropostaDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new PropostaDbContext(options);
            _repository = new PropostaRepository(_context);
        }

        [Fact]
        public async Task SalvarAsync_NovaProposta_DeveAdicionarProposta()
        {
            
            var idPessoa = Guid.NewGuid();
            var proposta = new Proposta(idPessoa, TipoProposta.Vida, 50000m);

            
            var result = await _repository.SalvarAsync(proposta);

            
            result.Should().NotBeNull();
            result.IdProposta.Should().Be(proposta.IdProposta);
            result.IdPessoa.Should().Be(idPessoa);
            result.Tipo.Should().Be(TipoProposta.Vida);
            result.Valor.Should().Be(50000m);

            var savedProposta = await _context.Propostas.FirstOrDefaultAsync(p => p.IdProposta == proposta.IdProposta);
            savedProposta.Should().NotBeNull();
        }

        [Fact]
        public async Task SalvarAsync_PropostaExistente_DeveAtualizarProposta()
        {
            
            var idPessoa = Guid.NewGuid();
            var proposta = new Proposta(idPessoa, TipoProposta.Vida, 50000m);
            
            // Primeiro salva a proposta
            await _context.Propostas.AddAsync(proposta);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var result = await _repository.SalvarAsync(proposta);
            result.Should().NotBeNull();
            result.IdProposta.Should().Be(proposta.IdProposta);

            var count = await _context.Propostas.CountAsync(p => p.IdProposta == proposta.IdProposta);
            count.Should().Be(1); // Deve haver apenas uma proposta com este ID
        }

        [Fact]
        public async Task ObterPorIdAsync_PropostaExiste_DeveRetornarProposta()
        {
            
            var idPessoa = Guid.NewGuid();
            var proposta = new Proposta(idPessoa, TipoProposta.Veicular, 30000m);
            await _context.Propostas.AddAsync(proposta);
            await _context.SaveChangesAsync();

            
            var result = await _repository.ObterPorIdAsync(proposta.IdProposta);

            
            result.Should().NotBeNull();
            result!.IdProposta.Should().Be(proposta.IdProposta);
            result.IdPessoa.Should().Be(idPessoa);
            result.Tipo.Should().Be(TipoProposta.Veicular);
            result.Valor.Should().Be(30000m);
            result.Status.Should().Be(StatusProposta.EmAnalise);
        }

        [Fact]
        public async Task ObterPorIdAsync_PropostaNaoExiste_DeveRetornarNull()
        {
            
            var idProposta = Guid.NewGuid();

            
            var result = await _repository.ObterPorIdAsync(idProposta);

            
            result.Should().BeNull();
        }

        [Fact]
        public async Task ObterAsync_DeveRetornarPropostasPaginadas()
        {
            
            var propostas = new List<Proposta>
            {
                new Proposta(Guid.NewGuid(), TipoProposta.Vida, 50000m),
                new Proposta(Guid.NewGuid(), TipoProposta.Veicular, 30000m),
                new Proposta(Guid.NewGuid(), TipoProposta.Imobiliario, 80000m)
            };

            await _context.Propostas.AddRangeAsync(propostas);
            await _context.SaveChangesAsync();

            
            var (items, total) = await _repository.ObterAsync(1, 2);

            
            items.Should().HaveCount(2);
            total.Should().Be(3);
            items.Should().BeInDescendingOrder(p => p.DataSolicitacao);
        }

        [Fact]
        public async Task ObterAsync_PaginaInvalida_DeveUsarPagina1()
        {
            
            var proposta = new Proposta(Guid.NewGuid(), TipoProposta.Vida, 50000m);
            await _context.Propostas.AddAsync(proposta);
            await _context.SaveChangesAsync();

            
            var (items, total) = await _repository.ObterAsync(0, 10); // Página inválida

            
            items.Should().HaveCount(1);
            total.Should().Be(1);
        }

        [Fact]
        public async Task ObterAsync_QuantidadeItensInvalida_DeveUsar10Items()
        {
            
            var propostas = Enumerable.Range(1, 15)
                .Select(_ => new Proposta(Guid.NewGuid(), TipoProposta.Vida, 50000m))
                .ToList();

            await _context.Propostas.AddRangeAsync(propostas);
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
