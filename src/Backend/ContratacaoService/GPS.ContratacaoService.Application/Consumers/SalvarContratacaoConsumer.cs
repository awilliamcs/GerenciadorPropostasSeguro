using AutoMapper;
using GPS.ContratacaoService.Application.DTOs;
using GPS.ContratacaoService.Application.Interfaces;
using GPS.ContratacaoService.Domain.Entidades;
using GPS.ContratacaoService.Domain.Interfaces;
using GPS.CrossCutting.Enums;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GPS.ContratacaoService.Application.Consumers
{
    public class SalvarContratacaoConsumer(IContratacaoRepository ContratacaoRepository, IMapper mapper, 
        IPropostaClient propostaClient, ILogger<SalvarContratacaoConsumer> logger) : IConsumer<ContratacaoDto>
    {
        private readonly IContratacaoRepository _contratacaoRepository = ContratacaoRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IPropostaClient _propostaClient = propostaClient;

        public async Task Consume(ConsumeContext<ContratacaoDto> context)
        {
            var proposta = await _propostaClient.ObterPorIdAsync(context.Message.IdProposta, context.CancellationToken);
            
            if (proposta == null)
            {
                logger.Log(LogLevel.Warning, $"Não existe a proposta ID {context.Message.IdProposta}.");
                return;
            }

            if (proposta.Status != StatusProposta.Aprovada)
            {
                logger.Log(LogLevel.Warning, $"A proposta proposta ID {context.Message.IdProposta} não está aprovada.");
                return;
            }

            var contratacao = _mapper.Map<Contratacao>(context.Message);
            await _contratacaoRepository.SalvarAsync(contratacao, context.CancellationToken);
        }
    }
}
