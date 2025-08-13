using AutoMapper;
using GPS.CrossCutting.Messaging;
using GPS.ContratacaoService.Application.DTOs;
using GPS.ContratacaoService.Application.Interfaces;
using GPS.ContratacaoService.Domain.Interfaces;

namespace GPS.ContratacaoService.Application.Services
{
    public class ContratacaoService(IContratacaoRepository contratacaoRepository, IMapper mapper, IClienteMQ clientMQ) : IContratacaoService
    {
        private readonly IContratacaoRepository _contratacaoRepository = contratacaoRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IClienteMQ _clientMQ = clientMQ;

        public async Task SalvarAsync(ContratacaoDto contratacaoDto, CancellationToken ct = default)
        {
            await _clientMQ.PublicarMensagemParaFila<ContratacaoDto>(contratacaoDto);
        }

        public async Task<ContratacaoListResponseDto> ObterAsync(int pagina, int quantidadeItens, CancellationToken ct = default)
        {
            var (items, total) = await _contratacaoRepository.ObterAsync(pagina, quantidadeItens, ct);

            return new ContratacaoListResponseDto
            {
                Items = _mapper.Map<List<ContratacaoDto>>(items),
                Total = total,
                Pagina = pagina,
                QuantidadeItens = quantidadeItens
            };
        }

        public async Task<ContratacaoDto?> ObterPorIdAsync(Guid idContratacao, CancellationToken ct = default)
        {
            var Contratacao = await _contratacaoRepository.ObterPorIdAsync(idContratacao, ct);
            return Contratacao != null ? _mapper.Map<ContratacaoDto>(Contratacao) : null;
        }
    }
}
