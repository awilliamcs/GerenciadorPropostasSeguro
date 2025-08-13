using AutoMapper;
using GPS.PropostaService.Application.DTOs;
using GPS.PropostaService.Domain.Entidades;
using GPS.PropostaService.Domain.Interfaces;
using MassTransit;

namespace GPS.PropostaService.Application.Consumers
{
    public class SalvarPropostaConsumer(IPropostaRepository propostaRepository, IMapper mapper) : IConsumer<PropostaDto>
    {
        private readonly IPropostaRepository _propostaRepository = propostaRepository;
        private readonly IMapper _mapper = mapper;

        public async Task Consume(ConsumeContext<PropostaDto> context)
        {
            var proposta = _mapper.Map<Proposta>(context.Message);
            await _propostaRepository.SalvarAsync(proposta, context.CancellationToken);
        }
    }
}
