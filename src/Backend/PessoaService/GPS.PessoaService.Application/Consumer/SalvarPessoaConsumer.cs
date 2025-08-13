using AutoMapper;
using GPS.PessoaService.Application.DTOs;
using GPS.PessoaService.Domain.Entidades;
using GPS.PessoaService.Domain.Interfaces;
using MassTransit;

namespace GPS.PessoaService.Application.Consumers
{
    public class SalvarPessoaConsumer(IPessoaRepository pessoaRepository, IMapper mapper) : IConsumer<PessoaDto>
    {
        private readonly IPessoaRepository _pessoaRepository = pessoaRepository;
        private readonly IMapper _mapper = mapper;

        public async Task Consume(ConsumeContext<PessoaDto> context)
        {
            var pessoa = _mapper.Map<Pessoa>(context.Message);
            await _pessoaRepository.SalvarAsync(pessoa, context.CancellationToken);
        }
    }
}
