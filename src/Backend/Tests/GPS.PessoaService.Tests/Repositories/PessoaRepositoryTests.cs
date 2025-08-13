using GPS.PessoaService.Domain.Entidades;
using GPS.PessoaService.Infrastructure;
using GPS.PessoaService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace GPS.PessoaService.Tests.Repositories
{
    public class PessoaRepositoryTests : IDisposable
    {
        private readonly PessoaDbContext _context;
        private readonly PessoaRepository _repository;

        public PessoaRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<PessoaDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new PessoaDbContext(options);
            _repository = new PessoaRepository(_context);
        }

        [Fact]
        public async Task SalvarAsync_NovaPessoa_DeveAdicionarPessoa()
        {
            
            var pessoa = new Pessoa("João Silva", "joao@email.com", "11999999999", 
                new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            
            var result = await _repository.SalvarAsync(pessoa);

            
            result.Should().NotBeNull();
            result.IdPessoa.Should().Be(pessoa.IdPessoa);
            result.Nome.Should().Be("João Silva");
            result.Email.Should().Be("joao@email.com");

            var savedPessoa = await _context.Pessoas.FirstOrDefaultAsync(p => p.IdPessoa == pessoa.IdPessoa);
            savedPessoa.Should().NotBeNull();
        }

        [Fact]
        public async Task SalvarAsync_PessoaExistente_DeveAtualizarPessoa()
        {
            
            var pessoa = new Pessoa("João Silva", "joao@email.com", "11999999999", 
                new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            
            // Primeiro salva a pessoa
            await _context.Pessoas.AddAsync(pessoa);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var result = await _repository.SalvarAsync(pessoa);
            
            result.Should().NotBeNull();
            result.IdPessoa.Should().Be(pessoa.IdPessoa);

            var count = await _context.Pessoas.CountAsync(p => p.IdPessoa == pessoa.IdPessoa);
            count.Should().Be(1); // Deve haver apenas uma pessoa com este ID
        }

        [Fact]
        public async Task ObterPorIdAsync_PessoaExiste_DeveRetornarPessoa()
        {
            
            var pessoa = new Pessoa("João Silva", "joao@email.com", "11999999999", 
                new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            await _context.Pessoas.AddAsync(pessoa);
            await _context.SaveChangesAsync();

            
            var result = await _repository.ObterPorIdAsync(pessoa.IdPessoa);

            
            result.Should().NotBeNull();
            result!.IdPessoa.Should().Be(pessoa.IdPessoa);
            result.Nome.Should().Be("João Silva");
            result.Email.Should().Be("joao@email.com");
            result.Telefone.Should().Be("11999999999");
        }

        [Fact]
        public async Task ObterPorIdAsync_PessoaNaoExiste_DeveRetornarNull()
        {
            
            var idPessoa = Guid.NewGuid();
            var result = await _repository.ObterPorIdAsync(idPessoa);
            result.Should().BeNull();
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
