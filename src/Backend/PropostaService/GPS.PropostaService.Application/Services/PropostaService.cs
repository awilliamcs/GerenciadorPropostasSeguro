using AutoMapper;
using GPS.CrossCutting.Messaging;
using GPS.PropostaService.Application.DTOs;
using GPS.PropostaService.Application.Interfaces;
using GPS.PropostaService.Domain.Interfaces;

namespace GPS.PropostaService.Application.Services
{
    public class PropostaService(IPropostaRepository propostaRepository, IMapper mapper, IClienteMQ clientMQ) : IPropostaService
    {
        private readonly IPropostaRepository _propostaRepository = propostaRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IClienteMQ _clientMQ = clientMQ;

        public async Task SalvarAsync(PropostaDto propostaDto, CancellationToken ct = default)
        {
            await _clientMQ.PublicarMensagemParaFila<PropostaDto>(propostaDto);
        }

        public async Task<PropostaListResponseDto> ObterAsync(int pagina, int quantidadeItens, CancellationToken ct = default)
        {
            var (items, total) = await _propostaRepository.ObterAsync(pagina, quantidadeItens, ct);

            return new PropostaListResponseDto
            {
                Items = _mapper.Map<List<PropostaDto>>(items),
                Total = total,
                Pagina = pagina,
                QuantidadeItens = quantidadeItens
            };
        }

        public async Task<PropostaDto?> ObterPorIdAsync(Guid idProposta, CancellationToken ct = default)
        {
            var proposta = await _propostaRepository.ObterPorIdAsync(idProposta, ct);
            return proposta != null ? _mapper.Map<PropostaDto>(proposta) : null;
        }
    }
}
